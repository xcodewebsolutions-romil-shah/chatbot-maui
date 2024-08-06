using Chatbot.MSAL.Helpers;
using Microsoft.Identity.Client;

namespace Chatbot.MSAL.MSALClient
{
    public class MSALClientHelper
    {
        public AuthenticationResult AuthResult { get; private set; }
        public IPublicClientApplication PublicClientApplication { get; private set; }
        private PublicClientApplicationBuilder PublicClientApplicationBuilder;

        public static readonly string KeyChainServiceName = "Contoso.MyProduct";
        public static readonly string KeyChainAccountName = "MSALCache";
        public static readonly KeyValuePair<string, string> LinuxKeyRingAttr2 = new KeyValuePair<string, string>("ProductGroup", "Contoso");

        public MSALClientHelper()
        {
            this.InitializePublicClientApplicationBuilder();
        }

        /// <summary>
        /// INITIALIZE PUBLIC CLIENT APPLICATION BUILDER
        /// </summary>
        private void InitializePublicClientApplicationBuilder()
        {
            this.PublicClientApplicationBuilder = PublicClientApplicationBuilder.Create(AppSettings.MSALAzureClientId)
                .WithExperimentalFeatures()
                .WithIosKeychainSecurityGroup("com.microsoft.adalcache");
        }

        /// <summary>
        /// INITIALIZE THE PUBLIC CLIENT APP
        /// </summary>
        /// <returns></returns>
        public async Task<IAccount> InitializePublicClientAppAsync()
        {
            this.PublicClientApplication = this.PublicClientApplicationBuilder
                .WithRedirectUri(AppSettings.MSALRedirectURI)
                .Build();
            return await GetSignedInUserAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// SIGN IN AND GET ACCESS TOKEN
        /// </summary>
        /// <param name="scopes"></param>
        /// <returns></returns>
        public async Task<string> SignInAndAcquireAccessToken(string[] scopes)
        {
            var alreadySignedInUser = await GetSignedInUserAsync().ConfigureAwait(false);
            try
            {
                if (alreadySignedInUser != null)
                {
                    this.AuthResult = await this.PublicClientApplication
                        .AcquireTokenSilent(scopes, alreadySignedInUser)
                        .ExecuteAsync()
                        .ConfigureAwait(false);
                }
                else
                {
                    this.AuthResult = await SignInInteractivelyAsync(scopes);
                }
            }
            catch (MsalUiRequiredException msalUIReqex)
            {
                this.AuthResult = await this.PublicClientApplication
                    .AcquireTokenInteractive(scopes)
                    .ExecuteAsync()
                    .ConfigureAwait(false);
            }
            catch (MsalException msalEx)
            {
                //ERROR ACQUIRING TOKEN INTERACTIVELY
            }
            return this.AuthResult?.AccessToken ?? "";
        }

        /// <summary>
        /// SIGN IN INTERACTIVELY
        /// </summary>
        /// <param name="scopes"></param>
        /// <param name="existingAccount"></param>
        /// <returns></returns>
        public async Task<AuthenticationResult> SignInInteractivelyAsync(string[] scopes, IAccount existingAccount = null)
        {
            AuthenticationResult? authenticationResult = null;
            try
            {
                if (this.PublicClientApplication != null)
                {
                    if (this.PublicClientApplication.IsUserInteractive())
                    {
                        authenticationResult = await this.PublicClientApplication.AcquireTokenInteractive(scopes)
                            .WithUseEmbeddedWebView(true)
                            .WithParentActivityOrWindow(PlatformConfigurations.Instance.ParentWindow)
                            .ExecuteAsync()
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        authenticationResult = await this.PublicClientApplication.AcquireTokenWithDeviceCode(scopes, (dcr) =>
                        {
                            return Task.CompletedTask;
                        }).ExecuteAsync().ConfigureAwait(false);
                    }
                }
            }
            catch
            {
            }
            return authenticationResult;
        }

        /// <summary>
        /// SIGN OUT ASYNC
        /// </summary>
        /// <returns></returns>
        public async Task SignOutAsync()
        {
            try
            {
                var existingUser = await GetSignedInUserAsync().ConfigureAwait(false);
                await this.SignOutAsync(existingUser).ConfigureAwait(false);
            }
            catch
            {
            }
        }

        /// <summary>
        /// SIGN OUT ASYNC
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task SignOutAsync(IAccount account)
        {
            try
            {
                if (this.PublicClientApplication == null) return;
                await this.PublicClientApplication.RemoveAsync(account).ConfigureAwait(false);
            }
            catch
            {
            }
        }

        /// <summary>
        /// GET SIGNED IN USER FROM CACHE
        /// </summary>
        /// <returns></returns>
        public async Task<IAccount> GetSignedInUserAsync()
        {
            IEnumerable<IAccount> allAccounts = await this.PublicClientApplication.GetAccountsAsync();
            try
            {
                if (allAccounts != null && allAccounts.Count() > 1)
                {
                    foreach (IAccount account in allAccounts)
                    {
                        await this.PublicClientApplication.RemoveAsync(account);
                    }
                    return null;
                }
            }
            catch
            {
            }
            return allAccounts.SingleOrDefault();
        }
    }
}