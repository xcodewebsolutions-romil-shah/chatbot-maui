using Chatbot.MSAL.Helpers;
using Chatbot.MSAL.Interfaces;
using Chatbot.MSAL.MSALClient;

namespace Chatbot.MSAL.Services
{
    public class MSALService : IMSALService
    {
        /// <summary>
        /// SIGN IN AND ACQUIRE TOKEN
        /// </summary>
        /// <returns></returns>
        public async Task<string> SignInAndAcquireAccessToken()
        {
            return await this.SignInAndAcquireAccessToken(AppSettings.MSALScopes).ConfigureAwait(false);
        }

        /// <summary>
        /// SIGN IN AND ACQUIRE TOKEN
        /// </summary>
        /// <param name="scopes"></param>
        /// <returns></returns>
        public async Task<string> SignInAndAcquireAccessToken(string[] scopes)
        {
            return await MSALPublicClientManager.Instance.SignInAndAcquireAccessToken(scopes).ConfigureAwait(false);
        }

        /// <summary>
        /// SIGN OUT
        /// </summary>
        /// <returns></returns>
        public async Task SignOutAsync()
        {
            await MSALPublicClientManager.Instance.SignOutAsync().ConfigureAwait(false);
        }
    }
}