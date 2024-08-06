namespace Chatbot.MSAL.MSALClient
{
    public class PlatformConfigurations
    {
        public static PlatformConfigurations Instance { get; } = new PlatformConfigurations();
        public object ParentWindow { get; set; }

        private PlatformConfigurations()
        {
        }
    }
}