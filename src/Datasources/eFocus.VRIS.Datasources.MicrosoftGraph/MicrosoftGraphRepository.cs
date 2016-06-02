using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eFocus.VRIS.Core.Models;
using eFocus.VRIS.Core.Repositories;
using eFocus.VRIS.Datasources.MicrosoftGraph.Helpers;
using eFocus.VRIS.Datasources.MicrosoftGraph.Providers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace eFocus.VRIS.Datasources.MicrosoftGraph
{
    public class MicrosoftGraphRepository : ICalenderRepository
    {
        private readonly MicrosoftGraphTokenProvider _tokenProvider;

        public MicrosoftGraphRepository(MicrosoftGraphTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        public async Task<AuthToken> Authorize()
        {
            var token = await _tokenProvider.GetToken();

            return new AuthToken
            {
                AccessToken = token.AccessToken,
                AccessTokenType = token.AccessTokenType,
                ExpiresOn = token.ExpiresOn,
                IdToken = token.IdToken,
                TenantId = token.TenantId
            };
        }
    }
}
