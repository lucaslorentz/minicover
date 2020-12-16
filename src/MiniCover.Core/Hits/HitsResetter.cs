using System;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace MiniCover.Core.Hits
{
    public class HitsResetter : IHitsResetter
    {
        private readonly ILogger<HitsResetter> _logger;

        public HitsResetter(
            ILogger<HitsResetter> logger)
        {
            _logger = logger;
        }

        public bool ResetHits(IDirectoryInfo hitsDirectory)
        {
            _logger.LogInformation("Resetting hits directory '{directory}'", hitsDirectory.FullName);

            var hitsFiles = hitsDirectory.Exists
                ? hitsDirectory.GetFiles("*.hits")
                : new IFileInfo[0];

            if (!hitsFiles.Any())
            {
                _logger.LogInformation("Directory is already cleared");
                return true;
            }

            _logger.LogInformation("Found {count} files to clean", hitsFiles.Length);

            var errorsCount = 0;
            foreach (var hitsFile in hitsFiles)
            {
                try
                {
                    hitsFile.Delete();
                    _logger.LogTrace("{fileName} - removed", hitsFile.FullName);
                }
                catch (Exception e)
                {
                    errorsCount++;
                    _logger.LogError("{fileName} - error: {error}", hitsFile.FullName, e.Message);
                }
            }

            if (errorsCount != 0)
            {
                _logger.LogError("Reset operation completed with {errorsCount} errors", errorsCount);
                return false;
            }

            _logger.LogInformation("Reset operation completed without errors");
            return true;
        }
    }
}
