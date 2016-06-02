using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using eFocus.VRIS.Core.Models;
using eFocus.VRIS.Core.Repositories;
using eFocus.VRIS.Datasources.MicrosoftGraph.Helpers;
using eFocus.VRIS.Datasources.MicrosoftGraph.Providers;
using Newtonsoft.Json.Linq;
using UserInfo = eFocus.VRIS.Core.Models.UserInfo;

namespace eFocus.VRIS.Datasources.MicrosoftGraph
{
    public class MicrosoftGraphRepository : ICalenderRepository
    {
        private readonly MicrosoftGraphTokenProvider _tokenProvider;
        private readonly MediaTypeWithQualityHeaderValue Json = new MediaTypeWithQualityHeaderValue("application/json");

        public MicrosoftGraphRepository(MicrosoftGraphTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        public async Task<AuthToken> Authorize(string authorizationCode, Uri loginRedirectUri)
        {
            var token = await _tokenProvider.GetToken(authorizationCode, loginRedirectUri);

            return new AuthToken
            {
                AccessToken = token.AccessToken,
                AccessTokenType = token.AccessTokenType,
                ExpiresOn = token.ExpiresOn,
                IdToken = token.IdToken,
                TenantId = token.TenantId
            };
        }

        public async Task<Uri> Login(Uri loginUri)
        {
            return await _tokenProvider.GetRedirectUrl(loginUri);
        }

        public async Task<UserInfo> GetUserInfoAsync(string accessToken)
        {
            var myInfo = new UserInfo();

            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, Settings.GetMeUrl))
                {
                    request.Headers.Accept.Add(Json);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    using (var response = await client.SendAsync(request))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                            myInfo.Name = json?["displayName"]?.ToString();
                            myInfo.Address = json?["mail"]?.ToString().Trim().Replace(" ", string.Empty);

                        }
                        else
                        {
                            var x = JObject.Parse(await response.Content.ReadAsStringAsync());
                            myInfo.Name = x.ToString();
                        }
                    }
                }
            }

            return myInfo;
        }
    }
}
