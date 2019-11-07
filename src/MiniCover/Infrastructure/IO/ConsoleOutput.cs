using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace MiniCover.Infrastructure.Console
{
    public class ConsoleOutput : IOutput
    {
        public LogLevel MinimumLevel { get; set; } = LogLevel.Information;

        public int Identation { get; set; } = 0;

        public void WriteLine(
            string message,
            LogLevel level)
        {
            if (level < MinimumLevel) return;

            var identation = new string(' ', Identation * 2);

            var identedMessage = identation + message.Replace(Environment.NewLine, Environment.NewLine + identation);

            var levelColor = GetLevelColor(level);

            if (levelColor != null)
                System.Console.ForegroundColor = levelColor.Value;

            GetLevelWriter(level).WriteLine(identedMessage);

            if (levelColor != null)
                System.Console.ResetColor();
        }

        private ConsoleColor? GetLevelColor(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Critical:
                case LogLevel.Error:
                    return ConsoleColor.Red;
                case LogLevel.Warning:
                    return ConsoleColor.Yellow;
                case LogLevel.Information:
                    return ConsoleColor.Blue;
                default:
                    return null;
            }
        }

        private TextWriter GetLevelWriter(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Critical:
                case LogLevel.Error:
                    return System.Console.Error;
                default:
                    return System.Console.Out;
            }
        }
    }
}
