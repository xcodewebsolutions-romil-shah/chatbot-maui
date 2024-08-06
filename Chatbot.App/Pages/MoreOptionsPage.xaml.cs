using Chatbot.MSAL.Interfaces;
using Chatbot.MSAL.Services;

namespace Chatbot.App.Pages
{
    public partial class MoreOptionsPage : ContentPage
    {
        IMSALService msalService = new MSALService();

        public MoreOptionsPage()
        {
            InitializeComponent();
        }

        #region EVENTS
        /// <summary>
        /// CLOSE POPUP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnCloseMoreOptions_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(animated: false);
        }

        /// <summary>
        /// SIGN OUT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private async void btnSignOut_Clicked(object sender, TappedEventArgs e)
        {
            bool confirmSignOut = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure to sign out?", "Yes", "No");
            if (confirmSignOut)
            {
                await Navigation.PopModalAsync(animated: true);
                await msalService.SignOutAsync().ContinueWith((t) =>
                {
                    return Task.CompletedTask;
                });

                if (App.Current != null)
                {
                    App.Current.MainPage = new LoaderPage(true);
                }
            }
        }
        #endregion
    }
}