using MiniCover.Model;

namespace MiniCover.Instrumentation
{
    public interface IUninstrumenter
    {
        void Execute(InstrumentationResult result);
    }
}