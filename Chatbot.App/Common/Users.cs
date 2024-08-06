using Chatbot.App.Pages;
using Chatbot.MSAL.Interfaces;
using Chatbot.MSAL.Services;

namespace Chatbot.App.Common
{
    public static class Users
    {
        static IMSALService msalService = new MSALService();

        /// <summary>
        /// SIGN IN USER
        /// </summary>
        /// <returns></returns>
        public static async Task SignInUser()
        {
            try
            {
                string accessToken = await msalService.SignInAndAcquireAccessToken();
                if (!string.IsNullOrEmpty(accessToken))
                {
                    App.Current.MainPage = new AppShell();
                }
                else
                {
                    App.Current.MainPage = new LoginPage();
                }
            }
            catch
            {
            }
        }
    }
}