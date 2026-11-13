using System.Text.Json;

namespace MQTester
{
    /// <summary>
    /// One connection profile from the "MQEnvironments" section of appsettings.json.
    /// Picking it in the Environment dropdown fills in the connection fields.
    /// </summary>
    public class MqEnvironment
    {
        public string Name { get; set; } = "";
        public string Host { get; set; } = "";
        public int Port { get; set; } = 1414;
        public string Channel { get; set; } = "";
        public string QueueManager { get; set; } = "";
        public List<string> Queues { get; set; } = new();
        public bool UseDefaultCredentials { get; set; } = true;
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";

        public override string ToString() => string.IsNullOrWhiteSpace(Name) ? $"{QueueManager} ({Host}:{Port})" : Name;
    }

    /// <summary>
    /// "MQDefaultCredentials" section - used by environments with UseDefaultCredentials = true.
    /// </summary>
    public class MqCredentials
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    /// <summary>
    /// Old single-connection "MQSettings" section. Still read if "MQEnvironments" is missing.
    /// </summary>
    public class MqSettings
    {
        public string Host { get; set; } = "";
        public int Port { get; set; }
        public string Channel { get; set; } = "";
        public string QueueManager { get; set; } = "";
        public string QueueName { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";

        public MqEnvironment ToEnvironment() => new()
        {
            Name = QueueManager,
            Host = Host,
            Port = Port,
            Channel = Channel,
            QueueManager = QueueManager,
            Queues = string.IsNullOrWhiteSpace(QueueName) ? new() : new() { QueueName },
            UseDefaultCredentials = false,
            Username = Username,
            Password = Password
        };
    }

    /// <summary>
    /// What was last used on screen, remembered between runs in
    /// %LOCALAPPDATA%\MQTester\userprefs.json (per Windows user, not shared).
    /// </summary>
    public class UserPrefs
    {
        public string? Environment { get; set; }
        public string? Queue { get; set; }
        public string? ReplyToQueue { get; set; }
        public bool WholeTextMode { get; set; }
        public int DelayMs { get; set; }
        public int RepeatCount { get; set; } = 1;
        public int PersistenceIndex { get; set; }
        public int ExpirySeconds { get; set; }
        public int ReplyWaitSeconds { get; set; } = 30;
        public int ReplyMatchIndex { get; set; }
        public bool BrowseMode { get; set; } = true;
        public int ReadCount { get; set; } = 10;
        public int ReadWaitSeconds { get; set; }

        private static string FilePath => Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "MQTester", "userprefs.json");

        public static UserPrefs Load()
        {
            try
            {
                if (File.Exists(FilePath))
                    return JsonSerializer.Deserialize<UserPrefs>(File.ReadAllText(FilePath)) ?? new UserPrefs();
            }
            catch (Exception ex)
            {
                Program.Logger.Warn(ex, "Could not read user preferences - using defaults");
            }
            return new UserPrefs();
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
                File.WriteAllText(FilePath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (Exception ex)
            {
                Program.Logger.Warn(ex, "Could not save user preferences");
            }
        }
    }
}
