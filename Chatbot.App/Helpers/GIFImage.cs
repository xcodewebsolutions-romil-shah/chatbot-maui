namespace Chatbot.App.Helpers
{
    public class GIFImage : Image
    {
        public string ImageName { get; set; }

        public static readonly BindableProperty IsAnimationPlayingProperty =
            BindableProperty.Create(
                nameof(IsAnimationPlaying),
                typeof(bool),
                typeof(GIFImage),
                false,
                propertyChanged: OnIsAnimationPlayingChanged);

        public bool IsPlaying
        {
            get => (bool)GetValue(IsAnimationPlayingProperty);
            set => SetValue(IsAnimationPlayingProperty, value);
        }
        /// <summary>
        /// PROPERTY CHANGED EVENT
        /// </summary>
        /// <param name="bindable"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        private static void OnIsAnimationPlayingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var gifImage = (GIFImage)bindable;
            gifImage.OnIsAnimationPlayingChanged((bool)newValue);
        }

        /// <summary>
        /// PROPERTY CHANGED EVENT
        /// </summary>
        /// <param name="isPlaying"></param>
        protected virtual void OnIsAnimationPlayingChanged(bool isPlaying)
        {
            IsAnimationPlayingChanged?.Invoke(this, isPlaying);
        }

        public event EventHandler<bool> IsAnimationPlayingChanged;
    }
}