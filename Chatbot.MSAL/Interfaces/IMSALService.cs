namespace Chatbot.MSAL.Interfaces
{
    public interface IMSALService
    {
        Task<string> SignInAndAcquireAccessToken();
        Task<string> SignInAndAcquireAccessToken(string[] scopes);
        Task SignOutAsync();
    }
}