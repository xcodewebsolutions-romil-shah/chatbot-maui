using Microsoft.Extensions.Logging;

#if IOS
using Chatbot.App.Helpers;
using Foundation;
#endif

namespace Chatbot.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("avenir-regular.ttf", "AvenirRegular");
                    fonts.AddFont("avenir-heavy.ttf", "AvenirBold");
                    fonts.AddFont("fa-regular-400.ttf", "FontAwesomeRegularIcons");
                    fonts.AddFont("fa-solid-900.ttf", "FontAwesomeSolidIcons");
                    fonts.AddFont("fa-light-300.ttf", "FontAwesomeLightIcons");
                })
                .ConfigureGifAnimation();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            //builder.Services.AddSingleton<IMSALService, MSALService>();
            return builder.Build();
        }

        /// <summary>
        /// CONFIGURE GIF ANIMATION
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static MauiAppBuilder ConfigureGifAnimation(this MauiAppBuilder builder)
        {
            builder.ConfigureMauiHandlers(handlers =>
            {
#if IOS
                Microsoft.Maui.Handlers.ImageHandler.Mapper.AppendToMapping("gifimage", async (handler, view) =>
                {
                    if (view is GIFImage)
                    {
                        var imageStream = await ConvertImageSourceToStreamAsync(((GIFImage)view).ImageName);
                        var nsdata = await GetDataAsync(imageStream);
                        var image = AnimatedImage.GetAnimatedImageView(nsdata, handler.PlatformView);

                        ((GIFImage)view).IsAnimationPlayingChanged += (s, isPlaying) =>
                        {
                            if (isPlaying)
                            {
                                AnimatedImage.StartAnimation();
                            }
                            else
                            {
                                AnimatedImage.StopAnimation();
                            }
                        };

                        if (((GIFImage)view).IsPlaying)
                        {
                            AnimatedImage.StartAnimation();
                        }
                    }
                });
#endif

            });
            return builder;
        }

        #region OTHER METHODS
#if IOS
        /// <summary>
        /// GET STREAM DATA
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        async static Task<NSData> GetDataAsync(Stream stream)
        {
            return await Task.Run(() =>
            {
                return NSData.FromStream(stream);
            });
        }
#endif

        /// <summary>
        /// CONVERT IMAGE TO STREAM
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        public static async Task<Stream> ConvertImageSourceToStreamAsync(string imageName)
        {
            return await FileSystem.OpenAppPackageFileAsync(imageName);
        }
        #endregion
    }
}