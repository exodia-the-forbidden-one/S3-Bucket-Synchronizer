using System;
using System.IO;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace S3Uploader.Helpers
{
    public class LogHelper
    {
        private static readonly Lazy<LogHelper> instance = new Lazy<LogHelper>(() => new LogHelper());

        public string LogDirectory { get; private set; }
        private Logger Logger { get; set; }

        private LogHelper()
        {
            LogDirectory = @"C:\S3FileUploader\Logs";

            Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(LogDirectory, DateTime.Now.ToString("yyyy-MM"), "log.txt"),
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
                .WriteTo.File(Path.Combine(LogDirectory, DateTime.Now.ToString("yyyy-MM"), "warning.txt"),
                    restrictedToMinimumLevel: LogEventLevel.Warning,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
                .WriteTo.File(Path.Combine(LogDirectory, DateTime.Now.ToString("yyyy-MM"), "error.txt"),
                    restrictedToMinimumLevel: LogEventLevel.Error,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
                .CreateLogger();
        }

        public static LogHelper Instance => instance.Value;

        public void Information(string message)
        {
            Logger.Information(message);
        }

        public void Error(string message)
        {
            Logger.Error(message);
        }

        public void Warning(string message)
        {
            Logger.Warning(message);
        }
    }
}