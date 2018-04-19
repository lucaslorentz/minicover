using MiniCover.Commands.Options;
using MiniCover.Commands.Options.Reports;
using MiniCover.Reports;
using System.Threading.Tasks;
using MiniCover.Utils;

namespace MiniCover.Commands.Reports
{
    internal class OpenCoverReportCommand : ParameterizationCommand
    {
        private const string CommandDescription = "Write an OpenCover-formatted XML report to file";
        private const string CommandName = "opencoverreport";

        internal OpenCoverReportCommand()
        : base(CommandName, CommandDescription, new WorkingDirectoryOption(), new CoverageLoadedFileOption(), new OpenCoverOutputOption(), new ThresholdOption())
        {
        }

        protected override Task<int> Execute()
        {
            OpenCoverReport.Execute(Parametrization.InstrumentationResult, Parametrization.OpenCoverFile, Parametrization.Threshold);
            var result = CalcUtils.IsHigherThanThreshold(Parametrization.InstrumentationResult, Parametrization.Threshold);
            return Task.FromResult(result);
        }
    }
}
