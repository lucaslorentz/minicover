using MiniCover.Core.Model;

namespace MiniCover.Core.Instrumentation
{
    public interface IUninstrumenter
    {
        void Uninstrument(InstrumentationResult result);
    }
}