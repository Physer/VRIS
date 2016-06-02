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

        public async Task<AuthenticationResult> GetToken(string authorizationCode, Uri loginRedirectUri)
        {
            if (string.IsNullOrWhiteSpace(_currentToken?.AccessToken) || DateTime.UtcNow >= _currentToken.ExpiresOn)
                return _currentToken = await Authorize(authorizationCode, loginRedirectUri);

            return _currentToken;
        }

        private async Task<AuthenticationResult> Authorize(string authorizationCode, Uri loginRedirectUri)
        {
            //var authContext = new AuthenticationContext(Settings.AzureADAuthority);
            //var credentials = new ClientCredential(Settings.ClientId, Settings.ClientSecret);
            //return await authContext.AcquireTokenAsync(Settings.O365UnifiedAPIResource, credentials);

            var authContext = new AuthenticationContext(Settings.AzureADAuthority);


            // Get the token.
            var authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(
                authorizationCode,                                         // the auth 'code' parameter from the Azure redirect.
                loginRedirectUri,                                               // same redirectUri as used before in Login method.
                new ClientCredential(Settings.ClientId, Settings.ClientSecret), // use the client ID and secret to establish app identity.
                Settings.O365UnifiedAPIResource);

            return authResult;

        }

        public async Task<Uri> GetRedirectUrl(Uri loginUri)
        {
            var authContext = new AuthenticationContext(Settings.AzureADAuthority);
            var credentials = new ClientCredential(Settings.ClientId, Settings.ClientSecret);
            var authUri = authContext.GetAuthorizationRequestURL(
                Settings.O365UnifiedAPIResource,
                Settings.ClientId,
                loginUri,
                UserIdentifier.AnyUser,
                "prompt=login");

            return authUri;
        }
    }
}
