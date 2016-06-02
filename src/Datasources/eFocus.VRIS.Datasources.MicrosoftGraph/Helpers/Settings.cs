using System.Configuration;

namespace eFocus.VRIS.Datasources.MicrosoftGraph.Helpers
{
    public static class Settings
    {
        public static string ClientId => ConfigurationManager.AppSettings["ClientID"];
        public static string ClientSecret => ConfigurationManager.AppSettings["ClientSecret"];

        public static string AzureADAuthority = @"https://login.microsoftonline.com/eea0e992-9e6d-46ea-a497-9fcf6258cd1c/common";
        public static string LogoutAuthority = @"https://login.microsoftonline.com/common/oauth2/logout?post_logout_redirect_uri=";
        public static string O365UnifiedAPIResource = @"https://graph.microsoft.com/";

        public static string SendMessageUrl = @"https://graph.microsoft.com/v1.0/me/microsoft.graph.sendmail";
        public static string GetMeUrl = @"https://graph.microsoft.com/v1.0/me";
        public static string MessageBody => ConfigurationManager.AppSettings["MessageBody"];
        public static string MessageSubject => ConfigurationManager.AppSettings["MessageSubject"];
    }
}
