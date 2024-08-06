using CoreAnimation;
using CoreGraphics;
using Foundation;
using ImageIO;
using System.Drawing;
using UIKit;

namespace Chatbot.App
{
    public class AnimatedImage
    {
        private static CAKeyFrameAnimation frameAnimation = new CAKeyFrameAnimation();
        private static List<NSObject> frameImages;
        private static double totalFrameDuration;
        public static UIImageView animationImageView;
        private static bool isAnimating;

        /// <summary>
        /// GET ANIMATED IMAGE VIEW
        /// </summary>
        /// <param name="nsData"></param>
        /// <param name="imageView"></param>
        /// <returns></returns>
        public static UIImageView GetAnimatedImageView(NSData nsData, UIImageView imageView = null)
        {
            var sourceRef = CGImageSource.FromData(nsData);
            return CreateAnimatedImageView(sourceRef, imageView);
        }

        /// <summary>
        /// CREATE ANIMATED IMAGE VIEW
        /// </summary>
        /// <param name="imageSource"></param>
        /// <param name="imageView"></param>
        /// <returns></returns>
        private static UIImageView CreateAnimatedImageView(CGImageSource imageSource, UIImageView imageView = null)
        {
            var frameCount = (int)imageSource.ImageCount;
            frameImages = new List<NSObject>(frameCount);

            var frameCGImages = new List<CGImage>(frameCount);
            var frameDurations = new List<double>(frameCount);

            totalFrameDuration = 0.0;

            for (int i = 0; i < frameCount; i++)
            {
                var frameImage = imageSource.CreateImage(i, null);
                frameCGImages.Add(frameImage);
                frameImages.Add(NSObject.FromObject(frameImage));

                var properties = imageSource.GetProperties(i, null);
                var duration = properties.Dictionary["{GIF}"];
                var delayTime = duration.ValueForKey(new NSString("DelayTime"));
                duration.Dispose();

                var realDuration = double.Parse(delayTime.ToString());
                frameDurations.Add(realDuration);
                totalFrameDuration += realDuration;
                frameImage.Dispose();
            }

            var framePercentageDurations = new List<NSNumber>(frameCount);
            var framePercentageDurationsDouble = new List<double>(frameCount);
            double currentDurationDouble = 0.0f;
            NSNumber currentDurationPercentage = 0.0f;

            for (int i = 0; i < frameCount; i++)
            {
                if (i != 0)
                {
                    var previousDuration = frameDurations[i - 1];
                    var previousDurationPercentage = framePercentageDurationsDouble[i - 1];
                    var number = previousDurationPercentage + (previousDuration / totalFrameDuration);

                    currentDurationDouble = number;
                    currentDurationPercentage = new NSNumber(number);
                }
                framePercentageDurationsDouble.Add(currentDurationDouble);
                framePercentageDurations.Add(currentDurationPercentage);
            }

            var imageSourceProperties = imageSource.GetProperties(null);
            var imageSourceGIFProperties = imageSourceProperties.Dictionary["{GIF}"];
            var loopCount = imageSourceGIFProperties.ValueForKey(new NSString("LoopCount"));
            var imageSourceLoopCount = float.Parse(loopCount.ToString());
            frameAnimation.KeyPath = "contents";

            if (imageSourceLoopCount <= 0.0f)
            {
                frameAnimation.RepeatCount = float.MaxValue;
            }
            else
            {
                frameAnimation.RepeatCount = imageSourceLoopCount;
            }

            imageSourceGIFProperties.Dispose();

            frameAnimation.CalculationMode = CAAnimation.AnimationDiscrete;//.AnimationDescrete;
            frameAnimation.Values = frameImages.ToArray();
            frameAnimation.Duration = totalFrameDuration;
            frameAnimation.KeyTimes = framePercentageDurations.ToArray();
            frameAnimation.RemovedOnCompletion = false;

            var firstFrame = frameCGImages[0];
            if (imageView == null)
                imageView = new UIImageView(new RectangleF(0.0f, 0.0f, 100, 100));
            else
                imageView.Layer.RemoveAllAnimations();

            animationImageView = imageView;
            return imageView;
        }

        /// <summary>
        /// START ANIMATION
        /// </summary>
        public static void StartAnimation()
        {
            if (!isAnimating)
            {
                animationImageView.Layer.AddAnimation(frameAnimation, "contents");
                isAnimating = true;
            }
        }

        /// <summary>
        /// STOP ANIMATION
        /// </summary>
        public static void StopAnimation()
        {
            if (isAnimating)
            {
                animationImageView.Layer.RemoveAnimation("contents");
                isAnimating = false;
            }
        }
    }
}
