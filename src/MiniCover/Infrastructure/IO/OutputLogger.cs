using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MiniCover.Infrastructure.Console
{
    public class OutputLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly IOutput _output;

        public OutputLogger(
            string categoryName,
            IOutput output)
        {
            _categoryName = categoryName;
            _output = output;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            _output.WriteLine(FormatMessage(state), GetLogLevel(state));
            return new Scope(_output);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var message = FormatMessage(state);
            _output.WriteLine(message, logLevel);
        }

        public LogLevel GetLogLevel(object state)
        {
            if (state is IReadOnlyList<KeyValuePair<string, object>> logValues)
            {
                foreach (var kv in logValues)
                {
                    if (kv.Value is LogLevel logLevel)
                        return logLevel;
                }
            }

            return LogLevel.Information;
        }

        public string FormatMessage(object state)
        {
            if (state is string s)
                return s;

            if (state is IReadOnlyList<KeyValuePair<string, object>> logValues)
            {
                var originalFormat = logValues
                    .Where(kv => kv.Key == "{OriginalFormat}")
                    .Select(kv => kv.Value)
                    .OfType<string>()
                    .First();

                if (!string.IsNullOrEmpty(originalFormat))
                {
                    var message = originalFormat;
                    foreach (var logValue in logValues.Where(kv => kv.Key != "{OriginalFormat}"))
                    {
                        message = message.Replace($"{{{logValue.Key}}}", JsonConvert.SerializeObject(logValue.Value, Formatting.Indented));
                    }
                    return message;
                }
            }

            return state.ToString();
        }

        class Scope : IDisposable
        {
            private readonly IOutput _output;

            public Scope(IOutput output)
            {
                _output = output;
                _output.Identation++;
            }

            public void Dispose()
            {
                _output.Identation--;
            }
        }
    }
}
