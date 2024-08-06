using Android.App;
using Android.Content;
using Chatbot.MSAL.Helpers;
using Microsoft.Identity.Client;

namespace Chatbot.App.Platforms.Android
{
    [Activity(Exported = true)]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
        DataHost = "auth",
        DataScheme = $"msal{AppSettings.MSALAzureClientId}")]
    public class MsalActivity : BrowserTabActivity
    {
    }
}