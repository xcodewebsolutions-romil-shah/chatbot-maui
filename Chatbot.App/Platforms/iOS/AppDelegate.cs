using Foundation;
using Chatbot.MSAL.MSALClient;

namespace Chatbot.App
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool FinishedLaunching(UIKit.UIApplication application, NSDictionary launchOptions)
        {
            Microsoft.Identity.Client.IAccount existinguser = Task.Run(async () => await MSALPublicClientManager.Instance.MSALClientHelper.InitializePublicClientAppAsync()).Result;
            return base.FinishedLaunching(application, launchOptions);
        }
    }
}
