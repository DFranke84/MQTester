using Microsoft.Extensions.Configuration;
using System.Text;
using NLog;
using NLog.Extensions.Logging;

namespace MQTester
{
    internal static class Program
    {
        public static IConfiguration Configuration { get; private set; } = null!;
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Lets us decode messages in EBCDIC / Windows code pages (CCSID 37, 500, 1252, ...)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // First run: no appsettings.json yet -> start from the example file.
            var settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            var examplePath = Path.Combine(AppContext.BaseDirectory, "appsettings.example.json");
            bool createdFromExample = false;
            if (!File.Exists(settingsPath) && File.Exists(examplePath))
            {
                File.Copy(examplePath, settingsPath);
                createdFromExample = true;
            }

            // Read appsettings.json from the program folder, so it works no matter
            // which folder the app was started from (shortcut, command line, etc.)
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            LogManager.Configuration = new NLogLoggingConfiguration(Configuration.GetSection("NLog"));

            ApplicationConfiguration.Initialize();

            if (createdFromExample)
                MessageBox.Show(
                    "No appsettings.json was found, so one was created from appsettings.example.json:\n\n" +
                    settingsPath + "\n\n" +
                    "Edit it to add your own queue managers, queues and credentials, then restart MQTester.",
                    "MQTester - first run", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Application.Run(new MQTester());
        }

        /// <summary>
        /// Reads the environment list from appsettings.json (falls back to the old "MQSettings" section).
        /// </summary>
        public static List<MqEnvironment> LoadEnvironments()
        {
            var envs = Configuration.GetSection("MQEnvironments").Get<List<MqEnvironment>>() ?? new();
            if (envs.Count == 0)
            {
                var old = Configuration.GetSection("MQSettings").Get<MqSettings>();
                if (old != null) envs.Add(old.ToEnvironment());
            }

            var defaults = Configuration.GetSection("MQDefaultCredentials").Get<MqCredentials>() ?? new MqCredentials();
            foreach (var env in envs.Where(e => e.UseDefaultCredentials))
            {
                env.Username = defaults.Username;
                env.Password = defaults.Password;
            }
            return envs;
        }
    }
}
