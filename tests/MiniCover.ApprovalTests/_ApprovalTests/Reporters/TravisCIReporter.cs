using System;
using ApprovalTests.Core;

namespace ApprovalTests.Reporters
{
    public class TravisCIReporter : IEnvironmentAwareReporter
    {
        public static readonly TravisCIReporter INSTANCE = new TravisCIReporter();

        public void Report(string approved, string received)
        {
            ContinousDeliveryUtils.ReportOnServer(approved,received);
        }

        public bool IsWorkingInThisEnvironment(string forFile)
        {
            var flag = Environment.GetEnvironmentVariable("TRAVIS");
            return "true".Equals(flag, StringComparison.OrdinalIgnoreCase);
        }
    }
}