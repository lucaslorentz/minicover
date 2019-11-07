using System;
using Microsoft.Extensions.Logging;
using MiniCover.Infrastructure;

namespace MiniCover.CommandLine.Options
{
    class VerbosityOption : SingleValueOption<LogLevel>
    {
        private const string _template = "-v | --verbosity";

        private readonly IOutput _output;

        public VerbosityOption(IOutput output)
            : base(_template, $"Change verbosity level ({GetPossibleValues()}) [default: {output.MinimumLevel}]")
        {
            _output = output;
        }

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

        protected override LogLevel PrepareValue(string value)
        {
            if (value != null && Enum.TryParse<LogLevel>(value, out var logLevel))
            {
                _output.MinimumLevel = logLevel;
            }

            return _output.MinimumLevel;
        }
    }
}