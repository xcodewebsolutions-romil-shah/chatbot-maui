using Chatbot.App.Common;

namespace Chatbot.App.Pages
{
    public partial class LoaderPage : ContentPage
    {
        [Obsolete]
        public LoaderPage(bool showSignInPage)
        {
            InitializeComponent();
            AnimateLoader();

            if (showSignInPage)
            {
                _ = Dispatcher.DispatchAsync(async () =>
                {
                    await Users.SignInUser();
                });
            }
        }

        #region METHODS
        /// <summary>
        /// ANIMATE LOADER
        /// </summary>
        private void AnimateLoader()
        {
            Task.Delay(500);
            imgLoader.IsAnimationPlaying = true;
            imgLoader.IsPlaying = true;
        }
        #endregion
    }
}