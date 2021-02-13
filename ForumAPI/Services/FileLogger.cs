using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace ForumAPI.Services
{
    internal sealed class FileLogger : ILogger
    {
        private readonly string filepath;

        public FileLogger(string fileName, string filepath = null)
        {
            if (filepath != default)
            {
                this.filepath = filepath;
                return;
            }

            var baseDirectory = @$"{Directory.GetCurrentDirectory()}\static-files\log";

            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }

            this.filepath = string.Concat(baseDirectory, @$"\{fileName}");
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!this.IsEnabled(logLevel))
            {
                return;
            }

            using (var file = File.AppendText(this.filepath))
            {
                file.WriteLine(
                    $"{logLevel} {eventId} Exception: {exception?.GetType().ToString() ?? "Custom Exception"} {exception?.Message ?? formatter.Invoke(state, exception)}{Environment.NewLine}Stacktrace: {exception?.StackTrace}");
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (logLevel == LogLevel.Trace || logLevel == LogLevel.Information)
            {
                return default;
            }

            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }

    internal sealed class FileLoggerProvider : ILoggerProvider
    {
        private readonly string filepath;
        private readonly string fileName;

        public FileLoggerProvider(string fileName, string filepath = null)
        {
            this.fileName = fileName;
            this.filepath = filepath;
        }

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(this.fileName, this.filepath);
        }
    }
}