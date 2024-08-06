using Chatbot.App.Pages;

namespace Chatbot.App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("MainPage", typeof(MainPage));
            Routing.RegisterRoute("ChatPage", typeof(ChatPage));
        }

        #region EVENTS
        /// <summary>
        /// ON NAVIGATING
        /// </summary>
        /// <param name="args"></param>
        protected override async void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);
            if (args.Target.Location.OriginalString.Contains("MoreOptionsTemp"))
            {
                args.Cancel();
                await Navigation.PushModalAsync(new MoreOptionsPage(), false);
            }
        }
        #endregion
    }
}