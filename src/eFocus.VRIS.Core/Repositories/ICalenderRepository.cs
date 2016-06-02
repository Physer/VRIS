using System;
using System.Threading.Tasks;
using eFocus.VRIS.Core.Models;

namespace eFocus.VRIS.Core.Repositories
{
    public interface ICalenderRepository
    {
        Task<AuthToken> Authorize(string authorizationCode, Uri loginRedirectUri);
        Task<Uri> Login(Uri loginUri);
        Task<UserInfo> GetUserInfoAsync(string accessToken);
    }
}
