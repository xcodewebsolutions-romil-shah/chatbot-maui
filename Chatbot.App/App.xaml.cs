using Chatbot.App.Common;
using Chatbot.App.Pages;

namespace Chatbot.App
{
    public partial class App : Application
    {
        [Obsolete]
        public App()
        {
            InitializeComponent();
            MainPage = new LoaderPage(false);
        }

        #region EVENTS
        /// <summary>
        /// ON START
        /// </summary>
        protected override void OnStart()
        {
            base.OnStart();

            _ = Dispatcher.DispatchAsync(async () =>
            {
                await Users.SignInUser();
            });
        }
        #endregion
    }
}