using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eFocus.VRIS.Datasources.MicrosoftGraph.Helpers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace eFocus.VRIS.Datasources.MicrosoftGraph.Providers
{
    public class MicrosoftGraphTokenProvider
    {
        private AuthenticationResult _currentToken;

        public async Task<AuthenticationResult> GetToken()
        {
            if (string.IsNullOrWhiteSpace(_currentToken?.AccessToken) || DateTime.UtcNow >= _currentToken.ExpiresOn)
                return _currentToken = await Authorize();

            return _currentToken;
        }

        private async Task<AuthenticationResult> Authorize()
        {
            var authContext = new AuthenticationContext(Settings.AzureADAuthority);
            var credentials = new ClientCredential(Settings.ClientId, Settings.ClientSecret);
            return await authContext.AcquireTokenAsync(Settings.O365UnifiedAPIResource, credentials);
        }
    }
}
