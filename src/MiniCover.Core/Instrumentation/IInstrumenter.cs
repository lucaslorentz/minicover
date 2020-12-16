using MiniCover.Core.Model;

namespace MiniCover.Core.Instrumentation
{
    public interface IInstrumenter
    {
        InstrumentationResult Instrument(InstrumentationContext context);
    }
}