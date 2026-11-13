using System.Text;

namespace MQTester
{
    /// <summary>
    /// Shows messages read off a queue: a grid at the top, the selected message's body
    /// and header fields below. Built in code (no designer file).
    /// </summary>
    public class MessageViewerForm : Form
    {
        private readonly List<MqMessageInfo> messages;
        private readonly Action<List<string>> loadIntoSendBox;

        private readonly DataGridView grid = new();
        private readonly TextBox txtBody = new();
        private readonly TextBox txtDetails = new();
        private readonly CheckBox chkWrap = new();
        private readonly CheckBox chkHex = new();

        public MessageViewerForm(List<MqMessageInfo> messages, string title, Action<List<string>> loadIntoSendBox)
        {
            this.messages = messages;
            this.loadIntoSendBox = loadIntoSendBox;

            Text = title;
            Size = new Size(1200, 800);
            MinimumSize = new Size(800, 500);
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = true;

            BuildLayout();
            FillGrid();
        }

        private void BuildLayout()
        {
            // ---- toolbar
            var toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(6, 6, 6, 0),
                WrapContents = false
            };
            toolbar.Controls.Add(MakeButton("Copy Body", (s, e) => CopyBody()));
            toolbar.Controls.Add(MakeButton("Copy MsgId", (s, e) => CopyId(m => m.MessageId, "MsgId")));
            toolbar.Controls.Add(MakeButton("Copy CorrelId", (s, e) => CopyId(m => m.CorrelationId, "CorrelId")));
            toolbar.Controls.Add(MakeButton("Load into Send Box", (s, e) => LoadSelected(), 140));
            toolbar.Controls.Add(MakeButton("Save Bodies...", (s, e) => SaveBodies(), 110));
            toolbar.Controls.Add(MakeButton("Save Report...", (s, e) => SaveReport(), 110));

            chkWrap.Text = "Wrap body";
            chkWrap.AutoSize = true;
            chkWrap.Margin = new Padding(16, 6, 3, 3);
            chkWrap.CheckedChanged += (s, e) => txtBody.WordWrap = chkWrap.Checked;
            toolbar.Controls.Add(chkWrap);

            chkHex.Text = "Show body as hex";
            chkHex.AutoSize = true;
            chkHex.Margin = new Padding(8, 6, 3, 3);
            chkHex.CheckedChanged += (s, e) => ShowSelected();
            toolbar.Controls.Add(chkHex);

            var lblCount = new Label
            {
                AutoSize = true,
                Margin = new Padding(16, 9, 3, 3),
                Text = "Ctrl/Shift-click to select several"
            };
            toolbar.Controls.Add(lblCount);

            // ---- grid
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = true;
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            grid.BackgroundColor = SystemColors.Window;
            grid.DefaultCellStyle.Font = new Font("Consolas", 9F);
            grid.SelectionChanged += (s, e) => ShowSelected();
            grid.CurrentCellChanged += (s, e) => ShowSelected();

            AddColumn("Pos", 50);
            AddColumn("Put date/time (UTC)", 160);
            AddColumn("MsgId", 220);
            AddColumn("CorrelId", 220);
            AddColumn("Type", 75);
            AddColumn("Format", 75);
            AddColumn("CCSID", 55);
            AddColumn("Bytes", 70);
            var preview = AddColumn("Preview", 400);
            preview.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // ---- body + details
            txtBody.Multiline = true;
            txtBody.ReadOnly = true;
            txtBody.ScrollBars = ScrollBars.Both;
            txtBody.WordWrap = false;
            txtBody.Font = new Font("Consolas", 9.75F);
            txtBody.Dock = DockStyle.Fill;
            txtBody.BackColor = SystemColors.Window;

            txtDetails.Multiline = true;
            txtDetails.ReadOnly = true;
            txtDetails.ScrollBars = ScrollBars.Both;
            txtDetails.WordWrap = false;
            txtDetails.Font = new Font("Consolas", 9F);
            txtDetails.Dock = DockStyle.Fill;
            txtDetails.BackColor = SystemColors.Window;

            var bottomSplit = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical };
            bottomSplit.Panel1.Controls.Add(WithCaption(txtBody, "Body"));
            bottomSplit.Panel2.Controls.Add(WithCaption(txtDetails, "Header fields / properties"));

            var mainSplit = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal };
            mainSplit.Panel1.Controls.Add(grid);
            mainSplit.Panel2.Controls.Add(bottomSplit);

            Controls.Add(mainSplit);
            Controls.Add(toolbar);

            Load += (s, e) =>
            {
                mainSplit.SplitterDistance = Math.Max(120, ClientSize.Height / 3);
                bottomSplit.SplitterDistance = Math.Max(300, bottomSplit.Width * 62 / 100);
            };
        }

        private static Control WithCaption(Control inner, string caption)
        {
            var panel = new Panel { Dock = DockStyle.Fill };
            var label = new Label { Text = caption, Dock = DockStyle.Top, Height = 20, TextAlign = ContentAlignment.BottomLeft };
            panel.Controls.Add(inner);
            panel.Controls.Add(label);
            return panel;
        }

        private static Button MakeButton(string text, EventHandler onClick, int width = 100)
        {
            var b = new Button { Text = text, Width = width, Height = 27, UseVisualStyleBackColor = true };
            b.Click += onClick;
            return b;
        }

        private DataGridViewTextBoxColumn AddColumn(string header, int width)
        {
            var col = new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                Width = width,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            grid.Columns.Add(col);
            return col;
        }

        private void FillGrid()
        {
            foreach (var m in messages)
            {
                var preview = m.Body.Replace("\r", " ").Replace("\n", " ");
                if (preview.Length > 300) preview = preview[..300] + "...";
                int row = grid.Rows.Add(
                    m.Position,
                    m.PutDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    MqIds.ToDisplay(m.MessageId),
                    MqIds.ToDisplay(m.CorrelationId),
                    m.MessageTypeText,
                    m.Format,
                    m.CharacterSet,
                    m.DataLength,
                    preview);
                grid.Rows[row].Tag = m;
            }
            if (grid.Rows.Count > 0) grid.Rows[0].Selected = true;
            ShowSelected();
        }

        /// <summary>Selected messages in queue order.</summary>
        private List<MqMessageInfo> Selected() => grid.SelectedRows.Cast<DataGridViewRow>()
            .Select(r => (MqMessageInfo)r.Tag!)
            .OrderBy(m => m.Position)
            .ToList();

        private MqMessageInfo? Current() => grid.CurrentRow?.Tag as MqMessageInfo ?? Selected().FirstOrDefault();

        private void ShowSelected()
        {
            var m = Current();
            if (m == null)
            {
                txtBody.Text = "";
                txtDetails.Text = "";
                return;
            }
            txtBody.Text = chkHex.Checked ? HexDump(m.Data) : m.Body;
            txtDetails.Text = m.DescribeHeaders();
        }

        private void Explain(string title, string why) =>
            MessageBox.Show(this, why, title, MessageBoxButtons.OK, MessageBoxIcon.Information);

        private const string SelectFirst = "Click a message in the list first (Ctrl/Shift-click to pick several).";

        private void CopyBody()
        {
            var sel = Selected();
            if (sel.Count == 0) { Explain("Copy Body", SelectFirst); return; }

            var text = string.Join(Environment.NewLine, sel.Select(m => m.Body));
            if (text.Length == 0) { Explain("Copy Body", "The selected message(s) have an empty body - nothing to copy."); return; }
            Clipboard.SetText(text);
        }

        private void CopyId(Func<MqMessageInfo, byte[]> pick, string idName)
        {
            var m = Current();
            if (m == null) { Explain($"Copy {idName}", SelectFirst); return; }

            var id = pick(m);
            if (MqIds.IsEmpty(id))
            {
                Explain($"Copy {idName}", $"Message {m.Position} has no {idName} (it is all zeros), so there is nothing to copy.");
                return;
            }
            Clipboard.SetText(MqIds.ToHex(id));
        }

        private void LoadSelected()
        {
            var sel = Selected();
            if (sel.Count == 0) { Explain("Load into Send Box", SelectFirst); return; }
            loadIntoSendBox(sel.Select(m => m.Body).ToList());
        }

        private void SaveBodies()
        {
            var sel = Selected();
            if (sel.Count == 0) sel = messages;
            using var dlg = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                FileName = "messages.txt",
                Title = $"Save {sel.Count} message body(s) - one per line"
            };
            if (dlg.ShowDialog(this) != DialogResult.OK) return;
            File.WriteAllText(dlg.FileName, string.Join(Environment.NewLine, sel.Select(m => m.Body)) + Environment.NewLine);
        }

        private void SaveReport()
        {
            var sel = Selected();
            if (sel.Count == 0) sel = messages;
            using var dlg = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                FileName = "message-report.txt",
                Title = $"Save full details of {sel.Count} message(s)"
            };
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            var sb = new StringBuilder();
            sb.AppendLine(Text);
            sb.AppendLine($"Saved {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            foreach (var m in sel)
            {
                sb.AppendLine();
                sb.AppendLine(new string('=', 80));
                sb.Append(m.DescribeHeaders());
                sb.AppendLine("--- Body ---");
                sb.AppendLine(m.Body);
            }
            File.WriteAllText(dlg.FileName, sb.ToString());
        }

        private static string HexDump(byte[] data)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < data.Length; i += 16)
            {
                var chunk = data.Skip(i).Take(16).ToArray();
                sb.Append($"{i:X8}  ");
                sb.Append(string.Join(" ", chunk.Select(b => b.ToString("X2"))).PadRight(48));
                sb.Append("  ");
                sb.AppendLine(new string(chunk.Select(b => b >= 0x20 && b < 0x7F ? (char)b : '.').ToArray()));
            }
            return sb.ToString();
        }
    }
}
