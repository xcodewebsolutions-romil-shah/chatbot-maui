namespace Chatbot.App.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        #region EVENTS
        /// <summary>
        /// NAVIGATE TO SIGN IN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private void btnSignInNavigation_Clicked(object sender, EventArgs e)
        {
            if(App.Current != null)
            {
                App.Current.MainPage = new LoaderPage(true);
            }
        }
        #endregion
    }
}