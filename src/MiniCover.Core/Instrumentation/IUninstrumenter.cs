using MiniCover.Core.Model;

namespace MiniCover.Core.Instrumentation
{
    public interface IUninstrumenter
    {
        void Execute(InstrumentationResult result);
    }
}