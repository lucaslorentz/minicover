using System.Threading.Tasks;
using MiniCover.Model;

namespace MiniCover.Reports.Coveralls
{
    public interface ICoverallsReport
    {
        Task<int> Execute(InstrumentationResult result,
               string output,
               string repoToken,
               string serviceJobId,
               string serviceName,
               string commitMessage,
               string rootfolder,
               string commit,
               string commitAuthorName,
               string commitAuthorEmail,
               string commitCommitterName,
               string commitCommitterEmail,
               string branch,
               string remoteName,
               string remoteUrl);
    }
}