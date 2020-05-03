using System;
using Microsoft.Extensions.Logging;
using MiniCover.Exceptions;
using MiniCover.Infrastructure;

namespace MiniCover.CommandLine.Options
{
    class VerbosityOption : ISingleValueOption
    {
        private readonly IOutput _output;

        public VerbosityOption(IOutput output)
        {
            _output = output;
        }

        public string Template => "-v | --verbosity";
        public string Description => $"Change verbosity level ({GetPossibleValues()}) [default: {_output.MinimumLevel}]";

        private static string GetPossibleValues()
        {
            return string.Join(", ", new[] {
                LogLevel.Trace,
                LogLevel.Debug,
                LogLevel.Information,
                LogLevel.Warning,
                LogLevel.Error,
                LogLevel.Critical
            });
        }

        public void ReceiveValue(string value)
        {
            if (value != null)
            {
                if (!Enum.TryParse<LogLevel>(value, true, out var logLevel))
                    throw new ValidationException($"Invalid verbosity '{value}'");

                _output.MinimumLevel = logLevel;
            }
        }
    }
}