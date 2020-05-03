using System.Threading.Tasks;

namespace MiniCover.CommandLine
{
    interface ICommand
    {
        string CommandName { get; }
        string CommandDescription { get; }
        IOption[] Options { get; }
        Task<int> Execute();
    }
}
