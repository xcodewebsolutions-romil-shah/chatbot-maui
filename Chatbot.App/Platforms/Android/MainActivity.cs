using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Chatbot.MSAL.MSALClient;
using Microsoft.Identity.Client;

namespace Chatbot.App
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            PlatformConfigurations.Instance.ParentWindow = this;
            _ = Task.Run(async () => await MSALPublicClientManager.Instance.MSALClientHelper.InitializePublicClientAppAsync()).Result;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }
    }
}