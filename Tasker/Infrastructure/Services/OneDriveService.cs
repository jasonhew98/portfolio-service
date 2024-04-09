using Tasker.Seedwork;
using CSharpFunctionalExtensions;
using System.IO;
using System.Threading.Tasks;

namespace Tasker.Infrastructure.Services
{
    public interface OneDriveService
    {
        Task<Result<byte[], CommandErrorResponse>> DriveDownloadFileFromAccessToken(string accessToken, string encodedUrl);
    }
}
