using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;

namespace PerformanceMeter.Settings
{
    public sealed class ApplicationSettings
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ApplicationSettings));
        const int loggerSectionPad = 29;
        const int columnPosition = 100;

        public static readonly string LogConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings", "Log.config");
        public static readonly string SettingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings", "PerformanceMeter.config");

        public static Configuration Configuration { get; private set; }

        public static bool AutRedirectStandardInput { get; private set; }

        public static bool AutRedirectStandardOutput { get; private set; }

        public static bool AutRedirectStandardError { get; private set; }

        public static bool AutCreateWindow { get; private set; }

        public ApplicationSettings()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings", "Logger.config")));
        }

        public void LoadConfig()
        {
            log.Debug("Loading application settings.");
            Configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap() { ExeConfigFilename = SettingsFilePath }, ConfigurationUserLevel.None);

            if (!Configuration.HasFile)
            {
                log.Fatal($"Failed to find Application Settings configuration file in: {SettingsFilePath}");
                throw new FileNotFoundException("PerformanceMeter.config file with application settings not found");
            }
            
            var appSettings = Configuration.AppSettings.Settings;
            string settingName = string.Empty;
            string settingValue = string.Empty;

            AutRedirectStandardInput = (appSettings["AutRedirectStandardInput"] != null) ? bool.Parse(appSettings["AutRedirectStandardInput"].Value) : false;
            settingName = $"Setting: {nameof(AutRedirectStandardInput)}";
            settingValue = $"Value: {AutRedirectStandardInput}".PadLeft(columnPosition - (loggerSectionPad + settingName.Length));
            log.Debug(settingName + settingValue);

            AutRedirectStandardOutput = (appSettings["AutRedirectStandardOutput"] != null) ? bool.Parse(appSettings["AutRedirectStandardOutput"].Value) : false;
            settingName = $"Setting: {nameof(AutRedirectStandardOutput)}";
            settingValue = $"Value: {AutRedirectStandardOutput}".PadLeft(columnPosition - (loggerSectionPad + settingName.Length));
            log.Debug(settingName + settingValue);

            AutRedirectStandardError = (appSettings["AutRedirectStandardError"] != null) ? bool.Parse(appSettings["AutRedirectStandardError"].Value) : false;
            settingName = $"Setting: {nameof(AutRedirectStandardError)}";
            settingValue = $"Value: {AutRedirectStandardError}".PadLeft(columnPosition - (loggerSectionPad + settingName.Length));
            log.Debug(settingName + settingValue);

            AutCreateWindow = (appSettings["AutCreateWindow"] != null) ? bool.Parse(appSettings["AutRedirectStandardError"].Value) : false;
            settingName = $"Setting: {nameof(AutCreateWindow)}";
            settingValue = $"Value: {AutCreateWindow}".PadLeft(columnPosition - (loggerSectionPad + settingName.Length));
            log.Debug(settingName + settingValue);
        }
    }
}
