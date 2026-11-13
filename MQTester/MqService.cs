using IBM.WMQ;
using NLog;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace MQTester
{
    /// <summary>Connection details taken from the screen.</summary>
    public record MqConnectionInfo(string Host, int Port, string Channel, string QueueManager, string Username, string Password);

    /// <summary>Options for sending messages.</summary>
    public class MqSendOptions
    {
        public int Persistence { get; set; } = MQC.MQPER_PERSISTENCE_AS_Q_DEF;
        public int ExpirySeconds { get; set; }                 // 0 = never expires
        public string ReplyToQueue { get; set; } = "";
        public byte[]? CorrelationId { get; set; }             // null = none
        public byte[]? MessageId { get; set; }                 // null = queue manager makes one
        public bool NewCorrelationIdPerMessage { get; set; }
        public int DelayMs { get; set; }
        public int RepeatCount { get; set; } = 1;
        public bool IsRequest { get; set; }
        public bool PassCorrelationId { get; set; }            // request/reply: reply should carry our CorrelId
    }

    /// <summary>Options for reading messages.</summary>
    public class MqReadOptions
    {
        public bool Destructive { get; set; }                  // false = browse (leave on queue)
        public int MaxCount { get; set; } = 10;                // 0 = all
        public byte[]? MatchCorrelationId { get; set; }
        public byte[]? MatchMessageId { get; set; }
        public int WaitSeconds { get; set; }                   // how long to wait for the first message
    }

    /// <summary>A message that was sent (for the log).</summary>
    public record MqSentInfo(int Number, byte[] MessageId, byte[] CorrelationId, string Preview);

    /// <summary>A message read off the queue, with its header fields decoded for display.</summary>
    public class MqMessageInfo
    {
        public int Position { get; set; }
        public byte[] MessageId { get; set; } = Array.Empty<byte>();
        public byte[] CorrelationId { get; set; } = Array.Empty<byte>();
        public DateTime PutDateTime { get; set; }
        public int MessageType { get; set; }
        public string Format { get; set; } = "";
        public int CharacterSet { get; set; }
        public int Encoding { get; set; }
        public int Persistence { get; set; }
        public int Priority { get; set; }
        public int Expiry { get; set; }
        public int Report { get; set; }
        public int BackoutCount { get; set; }
        public string ReplyToQueue { get; set; } = "";
        public string ReplyToQueueManager { get; set; } = "";
        public string PutApplicationName { get; set; } = "";
        public string UserId { get; set; } = "";
        public int DataLength { get; set; }
        public byte[] Data { get; set; } = Array.Empty<byte>();
        public string Body { get; set; } = "";
        public List<KeyValuePair<string, string>> Properties { get; set; } = new();

        public string MessageTypeText => MessageType switch
        {
            MQC.MQMT_DATAGRAM => "Datagram",
            MQC.MQMT_REQUEST => "Request",
            MQC.MQMT_REPLY => "Reply",
            MQC.MQMT_REPORT => "Report",
            _ => MessageType.ToString()
        };

        public string PersistenceText => Persistence switch
        {
            MQC.MQPER_PERSISTENT => "Persistent",
            MQC.MQPER_NOT_PERSISTENT => "Not persistent",
            _ => Persistence.ToString()
        };

        /// <summary>Full header listing for the details pane / saved report.</summary>
        public string DescribeHeaders()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Position        : {Position}");
            sb.AppendLine($"Put date/time   : {PutDateTime:yyyy-MM-dd HH:mm:ss.fff} (UTC)");
            sb.AppendLine($"MsgId (hex)     : {MqIds.ToHex(MessageId)}");
            sb.AppendLine($"MsgId (text)    : {MqIds.ToText(MessageId)}");
            sb.AppendLine($"CorrelId (hex)  : {MqIds.ToHex(CorrelationId)}");
            sb.AppendLine($"CorrelId (text) : {MqIds.ToText(CorrelationId)}");
            sb.AppendLine($"Message type    : {MessageTypeText}");
            sb.AppendLine($"Format          : {Format}");
            sb.AppendLine($"CCSID           : {CharacterSet}");
            sb.AppendLine($"Encoding        : {Encoding}");
            sb.AppendLine($"Persistence     : {PersistenceText}");
            sb.AppendLine($"Priority        : {Priority}");
            sb.AppendLine($"Expiry          : {(Expiry < 0 ? "Unlimited" : $"{Expiry / 10.0:0.#} sec left")}");
            sb.AppendLine($"Report options  : {Report}");
            sb.AppendLine($"Backout count   : {BackoutCount}");
            sb.AppendLine($"Reply-to queue  : {ReplyToQueue}");
            sb.AppendLine($"Reply-to QMgr   : {ReplyToQueueManager}");
            sb.AppendLine($"Put application : {PutApplicationName}");
            sb.AppendLine($"User id         : {UserId}");
            sb.AppendLine($"Data length     : {DataLength} bytes");
            if (Properties.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Message properties:");
                foreach (var p in Properties)
                    sb.AppendLine($"  {p.Key} = {p.Value}");
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Helpers for MQ message ids / correlation ids (24 bytes).
    /// Input: exactly 48 hex characters = hex bytes; anything else = text, padded with zero bytes.
    /// </summary>
    public static class MqIds
    {
        public const int Length = 24;

        public static byte[]? Parse(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            var s = input.Trim();

            if (s.Length == Length * 2 && s.All(Uri.IsHexDigit))
                return Convert.FromHexString(s);

            var bytes = System.Text.Encoding.UTF8.GetBytes(s);
            if (bytes.Length > Length)
                throw new ArgumentException($"\"{s}\" is too long for an MQ id ({bytes.Length} bytes, max {Length}). " +
                                            $"Use up to {Length} characters of text or exactly {Length * 2} hex characters.");
            var id = new byte[Length];
            Array.Copy(bytes, id, bytes.Length);
            return id;
        }

        public static byte[] NewId() => RandomNumberGenerator.GetBytes(Length);

        public static bool IsEmpty(byte[]? id) => id == null || id.All(b => b == 0);

        public static string ToHex(byte[]? id) => id == null ? "" : Convert.ToHexString(id);

        /// <summary>Shows the id as text if it is readable text, otherwise blank.</summary>
        public static string ToText(byte[]? id)
        {
            if (IsEmpty(id)) return "";
            var trimmed = id!.Reverse().SkipWhile(b => b == 0 || b == 0x20).Reverse().ToArray();
            return trimmed.All(b => b >= 0x20 && b < 0x7F) ? System.Text.Encoding.ASCII.GetString(trimmed) : "";
        }

        /// <summary>Text if readable, otherwise hex - for grids and the log.</summary>
        public static string ToDisplay(byte[]? id)
        {
            if (IsEmpty(id)) return "";
            var text = ToText(id);
            return text.Length > 0 ? text : ToHex(id);
        }
    }

    /// <summary>
    /// All IBM MQ calls live here. Each operation opens its own connection and closes it afterwards.
    /// The methods are blocking - the form runs them on a background thread.
    /// </summary>
    public class MqService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public MQQueueManager Connect(MqConnectionInfo info)
        {
            var properties = new Hashtable
            {
                // use the managed MQ client
                { MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED },

                // reconnect automatically after network interruptions; allow use from more than one thread
                { MQC.CONNECT_OPTIONS_PROPERTY, MQC.MQCNO_RECONNECT | MQC.MQCNO_HANDLE_SHARE_BLOCK },

                { MQC.HOST_NAME_PROPERTY, info.Host },
                { MQC.PORT_PROPERTY, info.Port },
                { MQC.CHANNEL_PROPERTY, info.Channel },
            };

            if (!string.IsNullOrEmpty(info.Username))
            {
                properties.Add(MQC.USER_ID_PROPERTY, info.Username);
                properties.Add(MQC.PASSWORD_PROPERTY, info.Password);
            }

            return new MQQueueManager(info.QueueManager, properties);
        }

        public void TestConnection(MqConnectionInfo info)
        {
            var qm = Connect(info);
            try { logger.Info($"Connection test successful: {info.QueueManager} on {info.Host}:{info.Port}"); }
            finally { SafeDisconnect(qm, null); }
        }

        public int GetDepth(MqConnectionInfo info, string queueName)
        {
            var qm = Connect(info);
            MQQueue? queue = null;
            try
            {
                queue = qm.AccessQueue(queueName, MQC.MQOO_INQUIRE | MQC.MQOO_FAIL_IF_QUIESCING);
                return queue.CurrentDepth;
            }
            finally { SafeDisconnect(qm, queue); }
        }

        /// <summary>
        /// Sends each body as one message (repeated RepeatCount times). Reports each sent message through onSent.
        /// Returns how many messages were sent.
        /// </summary>
        public int Send(MqConnectionInfo info, string queueName, IReadOnlyList<string> bodies, MqSendOptions options,
                        Action<MqSentInfo> onSent, CancellationToken token)
        {
            var qm = Connect(info);
            MQQueue? queue = null;
            int sent = 0;
            try
            {
                queue = qm.AccessQueue(queueName, MQC.MQOO_OUTPUT | MQC.MQOO_FAIL_IF_QUIESCING);
                int total = bodies.Count * Math.Max(1, options.RepeatCount);

                for (int r = 0; r < Math.Max(1, options.RepeatCount); r++)
                {
                    foreach (var body in bodies)
                    {
                        token.ThrowIfCancellationRequested();

                        var correlId = options.NewCorrelationIdPerMessage ? MqIds.NewId() : options.CorrelationId;
                        var message = BuildMessage(body, options, correlId, options.MessageId);
                        queue.Put(message, NewPutOptions());
                        sent++;

                        logger.Info($"Sent message {sent} to {queueName} MsgId={MqIds.ToHex(message.MessageId)} " +
                                    $"CorrelId={MqIds.ToHex(message.CorrelationId)}: {body}");
                        onSent(new MqSentInfo(sent, message.MessageId, message.CorrelationId, Preview(body)));

                        if (options.DelayMs > 0 && sent < total)
                            Task.Delay(options.DelayMs, token).Wait(token);
                    }
                }
                return sent;
            }
            finally { SafeDisconnect(qm, queue); }
        }

        /// <summary>Browses or gets messages, optionally only those matching a CorrelId / MsgId.</summary>
        public List<MqMessageInfo> Read(MqConnectionInfo info, string queueName, MqReadOptions options, CancellationToken token)
        {
            var qm = Connect(info);
            MQQueue? queue = null;
            var results = new List<MqMessageInfo>();
            try
            {
                int openOptions = (options.Destructive ? MQC.MQOO_INPUT_AS_Q_DEF : MQC.MQOO_BROWSE) | MQC.MQOO_FAIL_IF_QUIESCING;
                queue = qm.AccessQueue(queueName, openOptions);

                while (options.MaxCount <= 0 || results.Count < options.MaxCount)
                {
                    token.ThrowIfCancellationRequested();

                    var message = new MQMessage();
                    var gmo = new MQGetMessageOptions { MatchOptions = MQC.MQMO_NONE };
                    gmo.Options = MQC.MQGMO_FAIL_IF_QUIESCING | MQC.MQGMO_NO_SYNCPOINT;
                    if (!options.Destructive)
                        gmo.Options |= results.Count == 0 ? MQC.MQGMO_BROWSE_FIRST : MQC.MQGMO_BROWSE_NEXT;

                    if (options.MatchCorrelationId != null)
                    {
                        gmo.MatchOptions |= MQC.MQMO_MATCH_CORREL_ID;
                        message.CorrelationId = options.MatchCorrelationId;
                    }
                    if (options.MatchMessageId != null)
                    {
                        gmo.MatchOptions |= MQC.MQMO_MATCH_MSG_ID;
                        message.MessageId = options.MatchMessageId;
                    }

                    // Only the first read waits; after that we stop as soon as the queue is empty.
                    int waitMs = results.Count == 0 ? options.WaitSeconds * 1000 : 0;
                    if (!GetWithWait(queue, message, gmo, waitMs, token))
                        break;

                    results.Add(ToInfo(message, results.Count + 1));
                }

                logger.Info($"{(options.Destructive ? "Got" : "Browsed")} {results.Count} message(s) from {queueName}");
                return results;
            }
            finally { SafeDisconnect(qm, queue); }
        }

        /// <summary>Removes every message from the queue. Returns how many were removed.</summary>
        public int Purge(MqConnectionInfo info, string queueName, CancellationToken token)
        {
            var qm = Connect(info);
            MQQueue? queue = null;
            int removed = 0;
            try
            {
                queue = qm.AccessQueue(queueName, MQC.MQOO_INPUT_AS_Q_DEF | MQC.MQOO_FAIL_IF_QUIESCING);
                var gmo = new MQGetMessageOptions
                {
                    Options = MQC.MQGMO_NO_WAIT | MQC.MQGMO_FAIL_IF_QUIESCING | MQC.MQGMO_NO_SYNCPOINT |
                              MQC.MQGMO_ACCEPT_TRUNCATED_MSG
                };
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    var message = new MQMessage();
                    try
                    {
                        queue.Get(message, gmo, 0);   // 0-byte buffer + accept truncated = fast removal
                    }
                    catch (MQException ex) when (ex.ReasonCode == MQC.MQRC_NO_MSG_AVAILABLE)
                    {
                        break;
                    }
                    catch (MQException ex) when (ex.ReasonCode == MQC.MQRC_TRUNCATED_MSG_ACCEPTED)
                    {
                        // expected - message was removed
                    }
                    removed++;
                }
                logger.Warn($"Purged {removed} message(s) from {queueName}");
                return removed;
            }
            finally { SafeDisconnect(qm, queue); }
        }

        /// <summary>
        /// Sends one request message with a reply-to queue, then waits for the reply whose CorrelId matches
        /// either the request's MsgId (MQ default) or the request's CorrelId (if PassCorrelationId).
        /// Returns the reply, or null if none arrived in time.
        /// </summary>
        public (MqSentInfo Sent, MqMessageInfo? Reply) RequestReply(MqConnectionInfo info, string queueName, string body,
                                                                   MqSendOptions options, int waitSeconds, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(options.ReplyToQueue))
                throw new ArgumentException("Pick a Reply-to queue for Send & Wait for Reply.");

            var qm = Connect(info);
            MQQueue? requestQueue = null;
            MQQueue? replyQueue = null;
            try
            {
                options.IsRequest = true;
                var correlId = options.NewCorrelationIdPerMessage ? MqIds.NewId() : options.CorrelationId;
                if (options.PassCorrelationId && MqIds.IsEmpty(correlId))
                    correlId = MqIds.NewId();   // need a CorrelId to match on

                // Open the reply queue first so a fast reply can't be missed.
                replyQueue = qm.AccessQueue(options.ReplyToQueue, MQC.MQOO_INPUT_AS_Q_DEF | MQC.MQOO_FAIL_IF_QUIESCING);
                requestQueue = qm.AccessQueue(queueName, MQC.MQOO_OUTPUT | MQC.MQOO_FAIL_IF_QUIESCING);

                var request = BuildMessage(body, options, correlId, options.MessageId);
                requestQueue.Put(request, NewPutOptions());
                var sent = new MqSentInfo(1, request.MessageId, request.CorrelationId, Preview(body));
                logger.Info($"Sent request to {queueName} MsgId={MqIds.ToHex(request.MessageId)} " +
                            $"CorrelId={MqIds.ToHex(request.CorrelationId)} ReplyTo={options.ReplyToQueue}: {body}");

                var matchId = options.PassCorrelationId ? request.CorrelationId : request.MessageId;
                var reply = new MQMessage { CorrelationId = matchId };
                var gmo = new MQGetMessageOptions
                {
                    Options = MQC.MQGMO_FAIL_IF_QUIESCING | MQC.MQGMO_NO_SYNCPOINT,
                    MatchOptions = MQC.MQMO_MATCH_CORREL_ID
                };

                if (!GetWithWait(replyQueue, reply, gmo, waitSeconds * 1000, token))
                {
                    logger.Info($"No reply on {options.ReplyToQueue} within {waitSeconds} sec (CorrelId={MqIds.ToHex(matchId)})");
                    return (sent, null);
                }

                var info2 = ToInfo(reply, 1);
                logger.Info($"Reply received on {options.ReplyToQueue} MsgId={MqIds.ToHex(info2.MessageId)}: {info2.Body}");
                return (sent, info2);
            }
            finally
            {
                try { requestQueue?.Close(); } catch (Exception ex) { logger.Error(ex, "Error closing request queue"); }
                SafeDisconnect(qm, replyQueue);
            }
        }

        // ---------------------------------------------------------------- helpers

        private static MQMessage BuildMessage(string body, MqSendOptions options, byte[]? correlId, byte[]? msgId)
        {
            var message = new MQMessage();
            message.WriteString(body);
            message.Persistence = options.Persistence;
            message.Expiry = options.ExpirySeconds > 0 ? options.ExpirySeconds * 10 : MQC.MQEI_UNLIMITED;

            if (correlId != null) message.CorrelationId = correlId;
            if (msgId != null) message.MessageId = msgId;

            if (!string.IsNullOrWhiteSpace(options.ReplyToQueue))
            {
                message.ReplyToQueueName = options.ReplyToQueue.Trim();
                message.MessageType = MQC.MQMT_REQUEST;
                message.Report = options.PassCorrelationId ? MQC.MQRO_PASS_CORREL_ID : MQC.MQRO_COPY_MSG_ID_TO_CORREL_ID;
            }
            else if (options.IsRequest)
            {
                message.MessageType = MQC.MQMT_REQUEST;
            }
            return message;
        }

        private static MQPutMessageOptions NewPutOptions() => new()
        {
            Options = MQC.MQPMO_NO_SYNCPOINT | MQC.MQPMO_FAIL_IF_QUIESCING
        };

        /// <summary>
        /// Gets one message, waiting up to waitMs in 1-second slices so Cancel still works.
        /// Returns false if no message was available.
        /// </summary>
        private static bool GetWithWait(MQQueue queue, MQMessage message, MQGetMessageOptions gmo, int waitMs, CancellationToken token)
        {
            var correl = message.CorrelationId;
            var msgId = message.MessageId;
            var deadline = DateTime.UtcNow.AddMilliseconds(waitMs);

            while (true)
            {
                token.ThrowIfCancellationRequested();
                int slice = (int)Math.Min(1000, Math.Max(0, (deadline - DateTime.UtcNow).TotalMilliseconds));
                gmo.Options &= ~(MQC.MQGMO_WAIT | MQC.MQGMO_NO_WAIT);
                gmo.Options |= slice > 0 ? MQC.MQGMO_WAIT : MQC.MQGMO_NO_WAIT;
                gmo.WaitInterval = slice;

                // ids are overwritten by a failed get on some clients - put the match values back
                message.CorrelationId = correl;
                message.MessageId = msgId;
                try
                {
                    queue.Get(message, gmo);
                    return true;
                }
                catch (MQException ex) when (ex.ReasonCode == MQC.MQRC_NO_MSG_AVAILABLE)
                {
                    if (DateTime.UtcNow >= deadline) return false;
                }
            }
        }

        private static MqMessageInfo ToInfo(MQMessage m, int position)
        {
            var data = m.DataLength > 0 ? m.ReadBytes(m.DataLength) : Array.Empty<byte>();
            var info = new MqMessageInfo
            {
                Position = position,
                MessageId = m.MessageId,
                CorrelationId = m.CorrelationId,
                PutDateTime = m.PutDateTime,
                MessageType = m.MessageType,
                Format = (m.Format ?? "").Trim(),
                CharacterSet = m.CharacterSet,
                Encoding = m.Encoding,
                Persistence = m.Persistence,
                Priority = m.Priority,
                Expiry = m.Expiry,
                Report = m.Report,
                BackoutCount = m.BackoutCount,
                ReplyToQueue = (m.ReplyToQueueName ?? "").Trim(),
                ReplyToQueueManager = (m.ReplyToQueueManagerName ?? "").Trim(),
                PutApplicationName = (m.PutApplicationName ?? "").Trim(),
                UserId = (m.UserId ?? "").Trim(),
                DataLength = data.Length,
                Data = data,
                Body = Decode(data, m.CharacterSet, m.Encoding)
            };

            try
            {
                var names = m.GetPropertyNames("%");
                while (names.MoveNext())
                {
                    var name = names.Current?.ToString();
                    if (string.IsNullOrEmpty(name)) continue;
                    object? value = null;
                    try { value = m.GetObjectProperty(name); } catch { /* ignore unreadable property */ }
                    info.Properties.Add(new(name, value?.ToString() ?? ""));
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Could not read message properties");
            }

            return info;
        }

        /// <summary>Turns message bytes into text using the message's CCSID.</summary>
        public static string Decode(byte[] data, int ccsid, int mqEncoding)
        {
            if (data.Length == 0) return "";
            try
            {
                System.Text.Encoding enc = ccsid switch
                {
                    1200 or 13488 or 17584 => (mqEncoding & MQC.MQENC_INTEGER_REVERSED) != 0
                        ? System.Text.Encoding.Unicode : System.Text.Encoding.BigEndianUnicode,
                    1208 or 0 => System.Text.Encoding.UTF8,
                    819 => System.Text.Encoding.Latin1,
                    _ => System.Text.Encoding.GetEncoding(ccsid)   // 37, 500, 437, 850, 1252, ... (code page = CCSID)
                };
                return enc.GetString(data);
            }
            catch
            {
                return System.Text.Encoding.UTF8.GetString(data);
            }
        }

        private static string Preview(string body)
        {
            var oneLine = body.Replace("\r", " ").Replace("\n", " ");
            return oneLine.Length > 120 ? oneLine[..120] + "..." : oneLine;
        }

        private static void SafeDisconnect(MQQueueManager? qm, MQQueue? queue)
        {
            try { queue?.Close(); } catch (Exception ex) { logger.Error(ex, "Error closing queue"); }
            try { qm?.Disconnect(); } catch (Exception ex) { logger.Error(ex, "Error disconnecting"); }
        }
    }
}
