using IBM.WMQ;
using NLog;

namespace MQTester
{
    public partial class MQTester : Form
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly MqService mq = new();
        private readonly List<MqEnvironment> environments;
        private readonly UserPrefs prefs;
        private CancellationTokenSource? cts;

        public MQTester()
        {
            InitializeComponent();
            SetupToolTips();

            environments = Program.LoadEnvironments();
            prefs = UserPrefs.Load();

            cboEnvironment.Items.AddRange(environments.Cast<object>().ToArray());
            RestorePrefs();
        }

        // ================================================================ setup / preferences

        private void SetupToolTips()
        {
            const string idHelp = "Up to 24 characters of text, or exactly 48 hex characters. Leave blank for none.";
            toolTip.SetToolTip(cboEnvironment, "Environments come from MQEnvironments in appsettings.json");
            toolTip.SetToolTip(cboQueue, "Pick from the list or type any queue name");
            toolTip.SetToolTip(txtCorrelId, "Correlation id to put on sent messages. " + idHelp);
            toolTip.SetToolTip(btnNewCorrelId, "Generate a new random correlation id");
            toolTip.SetToolTip(chkNewCorrelPerMsg, "Give every message its own new random correlation id");
            toolTip.SetToolTip(txtMsgId, "Message id to use. Blank = queue manager generates one (normal). " + idHelp);
            toolTip.SetToolTip(nudRepeat, "Send the whole set this many times (load testing)");
            toolTip.SetToolTip(nudExpiry, "Message expires after this many seconds. 0 = never expires");
            toolTip.SetToolTip(cboReplyTo, "Reply-to queue put on sent messages (makes them Request messages). Blank = none");
            toolTip.SetToolTip(cboReplyMatch,
                "How the responding app marks its reply:\n" +
                "  my MsgId    = reply CorrelId is the request's MsgId (MQ default)\n" +
                "  my CorrelId = reply CorrelId is the request's CorrelId (pass-through)");
            toolTip.SetToolTip(btnSendAndWait, "Send one request message, then wait on the Reply-to queue for the matching reply");
            toolTip.SetToolTip(txtMatchCorrelId, "Only read messages with this correlation id. " + idHelp);
            toolTip.SetToolTip(txtMatchMsgId, "Only read the message with this message id. " + idHelp);
            toolTip.SetToolTip(btnPurge, "Remove ALL messages from the queue (asks first)");
        }

        private void RestorePrefs()
        {
            int envIndex = environments.FindIndex(e => e.ToString() == prefs.Environment);
            if (environments.Count > 0)
                cboEnvironment.SelectedIndex = envIndex >= 0 ? envIndex : 0;

            if (!string.IsNullOrWhiteSpace(prefs.Queue)) cboQueue.Text = prefs.Queue;
            if (prefs.ReplyToQueue != null) cboReplyTo.Text = prefs.ReplyToQueue;

            rbWholeText.Checked = prefs.WholeTextMode;
            rbEachLine.Checked = !prefs.WholeTextMode;
            nudDelay.Value = Clamp(nudDelay, prefs.DelayMs);
            nudRepeat.Value = Clamp(nudRepeat, prefs.RepeatCount);
            cboPersistence.SelectedIndex = prefs.PersistenceIndex is >= 0 and <= 2 ? prefs.PersistenceIndex : 0;
            nudExpiry.Value = Clamp(nudExpiry, prefs.ExpirySeconds);
            nudReplyWait.Value = Clamp(nudReplyWait, prefs.ReplyWaitSeconds);
            cboReplyMatch.SelectedIndex = prefs.ReplyMatchIndex is 0 or 1 ? prefs.ReplyMatchIndex : 0;
            rbBrowse.Checked = prefs.BrowseMode;
            rbGet.Checked = !prefs.BrowseMode;
            nudReadCount.Value = Clamp(nudReadCount, prefs.ReadCount);
            nudReadWait.Value = Clamp(nudReadWait, prefs.ReadWaitSeconds);
        }

        private static decimal Clamp(NumericUpDown nud, int value) => Math.Min(nud.Maximum, Math.Max(nud.Minimum, value));

        private void MQTester_FormClosing(object? sender, FormClosingEventArgs e)
        {
            cts?.Cancel();
            prefs.Environment = cboEnvironment.SelectedItem?.ToString();
            prefs.Queue = cboQueue.Text;
            prefs.ReplyToQueue = cboReplyTo.Text;
            prefs.WholeTextMode = rbWholeText.Checked;
            prefs.DelayMs = (int)nudDelay.Value;
            prefs.RepeatCount = (int)nudRepeat.Value;
            prefs.PersistenceIndex = cboPersistence.SelectedIndex;
            prefs.ExpirySeconds = (int)nudExpiry.Value;
            prefs.ReplyWaitSeconds = (int)nudReplyWait.Value;
            prefs.ReplyMatchIndex = cboReplyMatch.SelectedIndex;
            prefs.BrowseMode = rbBrowse.Checked;
            prefs.ReadCount = (int)nudReadCount.Value;
            prefs.ReadWaitSeconds = (int)nudReadWait.Value;
            prefs.Save();
        }

        private void cboEnvironment_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cboEnvironment.SelectedItem is not MqEnvironment env) return;

            txtHost.Text = env.Host;
            txtPort.Text = env.Port.ToString();
            txtChannel.Text = env.Channel;
            txtQueueManager.Text = env.QueueManager;
            txtUsername.Text = env.Username;
            txtPassword.Text = env.Password;

            cboQueue.Items.Clear();
            cboQueue.Items.AddRange(env.Queues.Cast<object>().ToArray());
            cboQueue.Text = env.Queues.FirstOrDefault() ?? "";

            var replyText = cboReplyTo.Text;
            cboReplyTo.Items.Clear();
            cboReplyTo.Items.Add("");
            cboReplyTo.Items.AddRange(env.Queues.Cast<object>().ToArray());
            cboReplyTo.Text = env.Queues.Contains(replyText) ? replyText : "";

            lblDepth.Text = "Depth: -";
        }

        // ================================================================ connection buttons

        private async void btnTestConnection_Click(object? sender, EventArgs e)
        {
            if (!Ready("Test Connection", needQueue: false)) return;
            var conn = GetConnection();

            await RunBusy($"Connecting to {conn.QueueManager}...", token =>
            {
                mq.TestConnection(conn);
                Log($"Connection OK: {conn.QueueManager} on {conn.Host}:{conn.Port} channel {conn.Channel}");
                SetStatus("Connection successful");
            });
        }

        private async void btnQueueDepth_Click(object? sender, EventArgs e)
        {
            if (!Ready("Queue Depth", needQueue: true)) return;
            var conn = GetConnection();
            var queue = GetQueue();

            await RunBusy($"Checking depth of {queue}...", token =>
            {
                int depth = mq.GetDepth(conn, queue);
                UI(() => lblDepth.Text = $"Depth: {depth:N0}");
                Log($"Queue depth of {queue}: {depth:N0}");
                SetStatus($"{queue} has {depth:N0} message(s)");
            });
        }

        // ================================================================ send

        private void btnLoadFile_Click(object? sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) != DialogResult.OK) return;
            try
            {
                txtMessage.Text = File.ReadAllText(openFileDialog.FileName);
                Log($"Loaded file {openFileDialog.FileName} ({txtMessage.Lines.Length:N0} line(s))");
            }
            catch (Exception ex)
            {
                ShowError($"Could not read the file: {ex.Message}", ex);
            }
        }

        private void btnClear_Click(object? sender, EventArgs e) => txtMessage.Clear();

        private void btnNewCorrelId_Click(object? sender, EventArgs e) => txtCorrelId.Text = MqIds.ToHex(MqIds.NewId());

        /// <summary>Message bodies from the text box, depending on the send mode.</summary>
        private List<string> GetBodies()
        {
            if (rbWholeText.Checked)
                return string.IsNullOrWhiteSpace(txtMessage.Text) ? new() : new() { txtMessage.Text };

            return txtMessage.Lines
                .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("--"))
                .ToList();
        }

        private MqSendOptions? GetSendOptions()
        {
            try
            {
                return new MqSendOptions
                {
                    Persistence = cboPersistence.SelectedIndex switch
                    {
                        1 => MQC.MQPER_PERSISTENT,
                        2 => MQC.MQPER_NOT_PERSISTENT,
                        _ => MQC.MQPER_PERSISTENCE_AS_Q_DEF
                    },
                    ExpirySeconds = (int)nudExpiry.Value,
                    ReplyToQueue = cboReplyTo.Text.Trim(),
                    CorrelationId = MqIds.Parse(txtCorrelId.Text),
                    MessageId = MqIds.Parse(txtMsgId.Text),
                    NewCorrelationIdPerMessage = chkNewCorrelPerMsg.Checked,
                    DelayMs = (int)nudDelay.Value,
                    RepeatCount = (int)nudRepeat.Value,
                    PassCorrelationId = cboReplyMatch.SelectedIndex == 1
                };
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(this, ex.Message, "Invalid id", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
        }

        private async void btnSend_Click(object? sender, EventArgs e)
        {
            var bodies = GetBodies();
            if (!Ready("Send Messages", needQueue: true,
                    bodies.Count == 0 ? NothingToSend() : null,
                    SendIdProblems()))
                return;
            if (!ConfirmCorrelIdIgnored()) return;

            var conn = GetConnection();
            var queue = GetQueue();
            var options = GetSendOptions();
            if (options == null) return;

            if (options.MessageId != null && bodies.Count * options.RepeatCount > 1)
            {
                var answer = MessageBox.Show(this,
                    "A fixed MsgId is set, so every message will get the same MsgId. Continue?",
                    "Same MsgId on every message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (answer != DialogResult.Yes) return;
            }

            int total = bodies.Count * options.RepeatCount;
            bool logEach = total <= 200;
            UI(() => { progressBar.Style = ProgressBarStyle.Continuous; progressBar.Maximum = total; progressBar.Value = 0; });

            await RunBusy($"Sending {total:N0} message(s) to {queue}...", token =>
            {
                int sent = mq.Send(conn, queue, bodies, options, info =>
                {
                    if (logEach || info.Number % 100 == 0 || info.Number == total)
                        Log($"Sent #{info.Number}  MsgId={MqIds.ToHex(info.MessageId)}  " +
                            $"CorrelId={MqIds.ToDisplay(info.CorrelationId)}  {info.Preview}");
                    UI(() =>
                    {
                        progressBar.Value = Math.Min(info.Number, progressBar.Maximum);
                        lblStatus.Text = $"Sending to {queue}... {info.Number:N0} of {total:N0}";
                    });
                }, token);
                Log($"Done - {sent:N0} message(s) sent to {queue}");
                SetStatus($"{sent:N0} message(s) sent to {queue}");
            }, showMarquee: false);
        }

        private async void btnSendAndWait_Click(object? sender, EventArgs e)
        {
            var bodies = GetBodies();
            if (!Ready("Send & Wait for Reply", needQueue: true,
                    bodies.Count == 0 ? NothingToSend() : null,
                    bodies.Count > 1
                        ? $"Send & Wait sends ONE request message, but there are {bodies.Count} lines to send. " +
                          "Choose \"Whole text = one message\", or leave just one line in the message box."
                        : null,
                    string.IsNullOrWhiteSpace(cboReplyTo.Text)
                        ? "Pick or type a Reply-to queue - that's where the reply is expected to arrive."
                        : null,
                    SendIdProblems()))
                return;
            if (!ConfirmCorrelIdIgnored()) return;

            var conn = GetConnection();
            var queue = GetQueue();
            var options = GetSendOptions();
            if (options == null) return;

            int wait = (int)nudReplyWait.Value;
            await RunBusy($"Sent request - waiting up to {wait} sec for reply on {options.ReplyToQueue}...", token =>
            {
                var (sent, reply) = mq.RequestReply(conn, queue, bodies[0], options, wait, token);
                Log($"Request sent to {queue}  MsgId={MqIds.ToHex(sent.MessageId)}  " +
                    $"CorrelId={MqIds.ToDisplay(sent.CorrelationId)}  ReplyTo={options.ReplyToQueue}");

                if (reply == null)
                {
                    Log($"No reply within {wait} sec on {options.ReplyToQueue}");
                    SetStatus("No reply received (timed out)");
                    return;
                }

                Log($"Reply received  MsgId={MqIds.ToHex(reply.MessageId)}  {Preview(reply.Body)}");
                SetStatus("Reply received");
                UI(() => ShowViewer(new List<MqMessageInfo> { reply }, $"Reply from {options.ReplyToQueue}"));
            });
        }

        // ================================================================ read / purge

        private async void btnRead_Click(object? sender, EventArgs e)
        {
            if (!Ready("Read Messages", needQueue: true,
                    IdProblem(txtMatchCorrelId, "Match CorrelId"),
                    IdProblem(txtMatchMsgId, "Match MsgId")))
                return;
            var conn = GetConnection();
            var queue = GetQueue();

            MqReadOptions options;
            try
            {
                options = new MqReadOptions
                {
                    Destructive = rbGet.Checked,
                    MaxCount = (int)nudReadCount.Value,
                    MatchCorrelationId = MqIds.Parse(txtMatchCorrelId.Text),
                    MatchMessageId = MqIds.Parse(txtMatchMsgId.Text),
                    WaitSeconds = (int)nudReadWait.Value
                };
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(this, ex.Message, "Invalid id", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (options.Destructive && options.MaxCount == 0)
            {
                var answer = MessageBox.Show(this,
                    $"Get with Max = 0 removes ALL matching messages from {queue} on {conn.QueueManager}.\n\nContinue?",
                    "Remove all messages?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (answer != DialogResult.Yes) return;
            }

            string verb = options.Destructive ? "Getting" : "Browsing";
            await RunBusy($"{verb} messages from {queue}...", token =>
            {
                var messages = mq.Read(conn, queue, options, token);
                string done = options.Destructive ? "Got (removed)" : "Browsed";
                Log($"{done} {messages.Count:N0} message(s) from {queue}" + MatchText(options));
                SetStatus($"{done} {messages.Count:N0} message(s) from {queue}");

                if (messages.Count > 0)
                    UI(() => ShowViewer(messages, $"{done} {messages.Count:N0} message(s) - {queue} on {conn.QueueManager}"));
            });
        }

        private static string MatchText(MqReadOptions o)
        {
            var parts = new List<string>();
            if (o.MatchCorrelationId != null) parts.Add($"CorrelId={MqIds.ToDisplay(o.MatchCorrelationId)}");
            if (o.MatchMessageId != null) parts.Add($"MsgId={MqIds.ToDisplay(o.MatchMessageId)}");
            return parts.Count == 0 ? "" : " matching " + string.Join(", ", parts);
        }

        private async void btnPurge_Click(object? sender, EventArgs e)
        {
            if (!Ready("Purge Queue", needQueue: true)) return;
            var conn = GetConnection();
            var queue = GetQueue();

            var answer = MessageBox.Show(this,
                $"This permanently removes ALL messages from:\n\n    Queue: {queue}\n    Queue manager: {conn.QueueManager} ({conn.Host}:{conn.Port})\n\n" +
                "This cannot be undone. Continue?",
                "Purge queue?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (answer != DialogResult.Yes) return;

            await RunBusy($"Purging {queue}...", token =>
            {
                int removed = mq.Purge(conn, queue, token);
                Log($"Purged {removed:N0} message(s) from {queue} on {conn.QueueManager}");
                SetStatus($"Purged {removed:N0} message(s) from {queue}");
                UI(() => lblDepth.Text = "Depth: 0");
            });
        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {
            cts?.Cancel();
            SetStatus("Cancelling...");
        }

        // ================================================================ message viewer

        private void ShowViewer(List<MqMessageInfo> messages, string title)
        {
            var viewer = new MessageViewerForm(messages, title, LoadIntoSendBox);
            viewer.Show(this);
        }

        /// <summary>Called from the viewer: puts message bodies into the send box for re-sending.</summary>
        private void LoadIntoSendBox(List<string> bodies)
        {
            if (bodies.Count == 0) return;

            if (bodies.Count == 1)
            {
                txtMessage.Text = bodies[0];
                bool multiLine = bodies[0].Contains('\n');
                rbWholeText.Checked = multiLine;
                rbEachLine.Checked = !multiLine;
            }
            else
            {
                if (bodies.Any(b => b.Contains('\n')))
                {
                    MessageBox.Show(this,
                        "Some of the selected messages have more than one line, so they can't be loaded as one-per-line.\n\n" +
                        "Load them one at a time instead.", "Load into send box", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                txtMessage.Text = string.Join(Environment.NewLine, bodies);
                rbEachLine.Checked = true;
            }
            Log($"Loaded {bodies.Count} message(s) into the send box");
            Activate();
        }

        // ================================================================ helpers

        // ---------------------------------------------------------------- "why can't I click that" checks

        /// <summary>
        /// Checks everything an action needs before it runs. If anything is missing or wrong,
        /// shows ONE popup listing every reason (instead of one popup per problem) and returns false.
        /// </summary>
        /// <param name="action">Button name, shown in the popup title and first line.</param>
        /// <param name="needQueue">True if the action works on a queue.</param>
        /// <param name="extraProblems">Action-specific problems; nulls are ignored.</param>
        private bool Ready(string action, bool needQueue, params string?[] extraProblems)
        {
            var problems = new List<string>();
            problems.AddRange(extraProblems.Where(p => p != null)!);

            if (string.IsNullOrWhiteSpace(txtHost.Text))
                problems.Add("Fill in Host (the MQ server name or IP address).");

            var portText = txtPort.Text.Trim();
            if (portText.Length == 0)
                problems.Add("Fill in Port (the queue manager's listener port, e.g. 1414).");
            else if (!int.TryParse(portText, out int port) || port <= 0 || port > 65535)
                problems.Add($"Port must be a number from 1 to 65535 - \"{portText}\" isn't.");

            if (string.IsNullOrWhiteSpace(txtChannel.Text))
                problems.Add("Fill in Channel (the server-connection channel, e.g. DEV.APP.SVRCONN).");
            if (string.IsNullOrWhiteSpace(txtQueueManager.Text))
                problems.Add("Fill in Queue Manager (e.g. QM1).");
            if (needQueue && string.IsNullOrWhiteSpace(cboQueue.Text))
                problems.Add("Pick or type a Queue.");

            if (problems.Count == 0) return true;

            string tip = cboEnvironment.Items.Count > 0 && problems.Any(p => p.StartsWith("Fill in"))
                ? "\n\nTip: picking an Environment fills in the connection fields for you."
                : "";
            MessageBox.Show(this,
                $"{action} can't run yet:\n\n" + string.Join("\n", problems.Select(p => "  • " + p)) + tip,
                action, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        private static string NothingToSend() =>
            "The message box is empty - type, paste or click Load File... first. " +
            "(In \"each line\" mode, blank lines and lines starting with -- don't count.)";

        /// <summary>Checks an id field; returns the reason if it isn't a valid MQ id, otherwise null.</summary>
        private static string? IdProblem(TextBox box, string fieldName)
        {
            try
            {
                MqIds.Parse(box.Text);
                return null;
            }
            catch (ArgumentException)
            {
                return $"{fieldName} is too long. Use up to {MqIds.Length} characters of text, " +
                       $"or exactly {MqIds.Length * 2} hex characters.";
            }
        }

        /// <summary>CorrelId / MsgId problems for the send buttons (CorrelId isn't checked when it will be ignored).</summary>
        private string? SendIdProblems()
        {
            var problems = new[]
            {
                chkNewCorrelPerMsg.Checked ? null : IdProblem(txtCorrelId, "CorrelId"),
                IdProblem(txtMsgId, "MsgId")
            }.Where(p => p != null).ToList();
            return problems.Count == 0 ? null : string.Join("\n  • ", problems);
        }

        /// <summary>
        /// If "New per msg" is ticked AND a CorrelId is typed in, the typed one is ignored - say so and let the user choose.
        /// </summary>
        private bool ConfirmCorrelIdIgnored()
        {
            if (!chkNewCorrelPerMsg.Checked || string.IsNullOrWhiteSpace(txtCorrelId.Text)) return true;

            var answer = MessageBox.Show(this,
                "\"New per msg\" is ticked, so the CorrelId you typed will be IGNORED - " +
                "every message gets its own new random CorrelId instead.\n\n" +
                "Send anyway?\n\n(To use your CorrelId on every message, untick \"New per msg\".)",
                "CorrelId will be ignored", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return answer == DialogResult.Yes;
        }

        /// <summary>Connection details from the screen. Call Ready(...) first - this assumes the fields are valid.</summary>
        private MqConnectionInfo GetConnection() =>
            new(txtHost.Text.Trim(), int.Parse(txtPort.Text.Trim()), txtChannel.Text.Trim(), txtQueueManager.Text.Trim(),
                txtUsername.Text.Trim(), txtPassword.Text);

        private string GetQueue() => cboQueue.Text.Trim();

        /// <summary>
        /// Runs MQ work on a background thread so the window stays responsive.
        /// Disables the action buttons, enables Cancel, and reports errors in the log.
        /// </summary>
        private async Task RunBusy(string status, Action<CancellationToken> work, bool showMarquee = true)
        {
            cts = new CancellationTokenSource();
            var token = cts.Token;
            SetBusy(true, showMarquee);
            SetStatus(status);
            try
            {
                await Task.Run(() => work(token), token);
            }
            catch (OperationCanceledException)
            {
                Log("Cancelled.");
                SetStatus("Cancelled");
            }
            catch (AggregateException ae) when (ae.InnerException is OperationCanceledException)
            {
                Log("Cancelled.");
                SetStatus("Cancelled");
            }
            catch (MQException mqe)
            {
                ShowError($"MQ error {mqe.ReasonCode} ({ReasonName(mqe.ReasonCode)}): {mqe.Message}", mqe);
            }
            catch (Exception ex)
            {
                ShowError($"Error: {ex.Message}", ex);
            }
            finally
            {
                SetBusy(false, showMarquee);
                cts.Dispose();
                cts = null;
            }
        }

        private void SetBusy(bool busy, bool marquee)
        {
            foreach (var b in new Control[] { btnTestConnection, btnQueueDepth, btnSend, btnSendAndWait, btnRead, btnPurge, cboEnvironment })
                b.Enabled = !busy;
            btnCancel.Enabled = busy;
            progressBar.Visible = busy;
            if (busy && marquee) progressBar.Style = ProgressBarStyle.Marquee;
            UseWaitCursor = false;
        }

        private void ShowError(string message, Exception ex)
        {
            logger.Error(ex, message);
            Log("ERROR: " + message);
            SetStatus("Error - see log");
            MessageBox.Show(this, message, "MQ Tester", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>Friendly names for the MQ reason codes you are most likely to hit.</summary>
        private static string ReasonName(int code) => code switch
        {
            2009 => "connection broken",
            2035 => "not authorized - check user / password / channel auth",
            2058 => "queue manager name wrong for this host/port",
            2059 => "queue manager not available - check host / port / channel",
            2085 => "unknown queue name",
            2033 => "no message available",
            2053 => "queue full",
            2016 => "gets inhibited on queue",
            2051 => "puts inhibited on queue",
            2538 => "host not available - check host / port",
            2540 => "unknown channel name",
            _ => "see IBM MQ reason codes"
        };

        private void Log(string text)
        {
            var line = $"{DateTime.Now:HH:mm:ss}  {text}{Environment.NewLine}";
            UI(() =>
            {
                // keep the on-screen log from growing forever
                if (txtLog.TextLength > 500_000)
                    txtLog.Text = txtLog.Text[^250_000..];
                txtLog.AppendText(line);
            });
        }

        private void SetStatus(string text) => UI(() => lblStatus.Text = text);

        /// <summary>Runs an action on the UI thread (safe to call from background work).</summary>
        private void UI(Action action)
        {
            if (IsDisposed) return;
            if (InvokeRequired) BeginInvoke(action);
            else action();
        }

        private static string Preview(string body)
        {
            var oneLine = body.Replace("\r", " ").Replace("\n", " ");
            return oneLine.Length > 120 ? oneLine[..120] + "..." : oneLine;
        }
    }
}
