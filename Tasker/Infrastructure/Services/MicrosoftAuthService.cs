using Tasker.Seedwork;
using CSharpFunctionalExtensions;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using System.Threading.Tasks;

namespace Tasker.Infrastructure.Services
{
    public interface MicrosoftAuthService
    {
        Task<Result<TokenResponse, CommandErrorResponse>> RefreshAccessToken(RefreshTokenRequest authRequest);
    }
}
