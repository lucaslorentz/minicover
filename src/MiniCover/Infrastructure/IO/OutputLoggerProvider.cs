using Microsoft.Extensions.Logging;

namespace MiniCover.Infrastructure.Console
{
    public class OutputLoggerProvider : ILoggerProvider
    {
        private readonly IOutput _output;

        public OutputLoggerProvider(IOutput output)
        {
            _output = output;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new OutputLogger(categoryName, _output);
        }

        public void Dispose()
        {
        }
    }
}
