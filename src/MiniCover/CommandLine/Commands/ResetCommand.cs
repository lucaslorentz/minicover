using System;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MiniCover.CommandLine;
using MiniCover.CommandLine.Options;
using MiniCover.Core.Hits;
using MiniCover.IO;

namespace MiniCover.Commands
{
    public class ResetCommand : ICommand
    {
        private readonly HitsDirectoryOption _hitsDirectoryOption;

        private readonly IHitsResetter _hitResetService;
        private readonly VerbosityOption _verbosityOption;
        private readonly WorkingDirectoryOption _workingDirectoryOption;

        public ResetCommand(
            IHitsResetter hitResetService,
            VerbosityOption verbosityOption,
            WorkingDirectoryOption workingDirectoryOption,
            HitsDirectoryOption hitsDirectoryOption)
        {
            _hitResetService = hitResetService;
            _verbosityOption = verbosityOption;
            _workingDirectoryOption = workingDirectoryOption;
            _hitsDirectoryOption = hitsDirectoryOption;
        }

        public string CommandName => "reset";
        public string CommandDescription => "Reset hits count";

        public IOption[] Options => new IOption[]
        {
            _verbosityOption,
            _workingDirectoryOption,
            _hitsDirectoryOption
        };

        public Task<int> Execute()
        {
            var hitsDirectory = _hitsDirectoryOption.DirectoryInfo;

            if (!_hitResetService.ResetHits(hitsDirectory))
                return Task.FromResult(1);

            return Task.FromResult(0);
        }
    }
}