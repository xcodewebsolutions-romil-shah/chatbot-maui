using Chatbot.MSAL.Helpers;
using System.Runtime.CompilerServices;

namespace Chatbot.MSAL.MSALClient
{
    public class MSALPublicClientManager
    {
        public static MSALPublicClientManager Instance { get; private set; } = new MSALPublicClientManager();
        public MSALClientHelper MSALClientHelper { get; }


        [MethodImpl(MethodImplOptions.NoInlining)]
        private MSALPublicClientManager()
        {
            this.MSALClientHelper = new MSALClientHelper();
        }

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
            return await this.MSALClientHelper.SignInAndAcquireAccessToken(scopes).ConfigureAwait(false);
        }

        /// <summary>
        /// SIGN OUT
        /// </summary>
        /// <returns></returns>
        public async Task SignOutAsync()
        {
            await this.MSALClientHelper.SignOutAsync().ConfigureAwait(false);
        }
    }
}