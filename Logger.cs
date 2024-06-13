using System.Reflection;
using NLog;
using NLog.Layouts;
using NLog.Targets;

namespace Coaction.KickAssCardBot
{
    public class Logger
    {
        public static ILogger Log = LogManager.GetLogger($"{Assembly.GetExecutingAssembly().GetName().Name}Logger", Assembly.GetExecutingAssembly().GetType());

        public static void SetupLogger()
        {
            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new FileTarget("logfile")
            {
                FileName = "logger.txt", 
                ArchiveEvery = FileArchivePeriod.Day, 
                ArchiveNumbering = ArchiveNumberingMode.Date, 
                MaxArchiveDays = 30, 
                ArchiveDateFormat = "yyyyMMddHHmm", 
                ArchiveFileName = Layout.FromString("logs/MTG.{#}.log"), 
                Layout = Layout.FromString(@"${longdate} ${callsite} ${level} ${message}")
            };

            var logconsole = new ConsoleTarget("logconsole");

            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);


            // Apply config           
            LogManager.Configuration = config;
        }
    }
}
