namespace Chatbot.MSAL.Helpers
{
    public static class AppSettings
    {
        //MSAL
        public const string MSALAzureClientId = "MSAL_CLIENT_ID";
        public const string MSALRedirectURI = $"msal{MSALAzureClientId}://auth";
        public static readonly string[] MSALScopes = { "offline_access", $"{MSALAzureClientId}/.default" };
        public const string MSALNativeRedirectURI = "https://login.microsoftonline.com/common/oauth2/nativeclient";
    }
}