namespace Chatbot.App.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        #region EVENTS
        /// <summary>
        /// ON CLICK OF FRAME TO CAPTURE TIME SHEETS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void frameCaptureTimeSheets_Tapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("ChatPage");
        }

        /// <summary>
        /// ON CLICK OF FRAMES THAT ARE NOT IMPLEMENTED
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void frameNotImplemented_Tapped(object sender, TappedEventArgs e)
        {
            await DisplayAlert("Alert", "Feature coming soon! Thanks for your understanding!", "OK");
        }
        #endregion
    }
}