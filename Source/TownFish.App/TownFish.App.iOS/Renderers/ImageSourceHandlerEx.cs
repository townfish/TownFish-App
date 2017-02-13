using System;
using System.Threading;
using System.Threading.Tasks;

using Foundation;
using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using TownFish.App.Helpers;
using TownFish.App.iOS.Renderers;


[assembly: ExportImageSourceHandler (typeof (UriImageSource), typeof (UriImageSourceHandlerEx))]
[assembly: ExportImageSourceHandler (typeof (StreamImageSource), typeof (StreamImageSourceHandlerEx))]


namespace TownFish.App.iOS.Renderers
{
	public class UriImageSourceHandlerEx: IImageSourceHandler
	{
		ImageLoaderSourceHandler mImageLoaderSourceHandler;

		public UriImageSourceHandlerEx()
		{
			mImageLoaderSourceHandler = new ImageLoaderSourceHandler();
		}

		async public Task<UIImage> LoadImageAsync (ImageSource imageSource, CancellationToken cancelationToken, float scale)
		{
			var image = await mImageLoaderSourceHandler
					.LoadImageAsync (imageSource, cancelationToken, scale);

			ImageSourceWatcher.GetImageLoadedHandler (imageSource)?
					.Invoke (imageSource, image != null);

			return image;
		}
	}

	public class StreamImageSourceHandlerEx: IImageSourceHandler
	{
		StreamImagesourceHandler mStreamImageSourceHandler;

		public StreamImageSourceHandlerEx()
		{
			mStreamImageSourceHandler = new StreamImagesourceHandler();
		}

		async public Task<UIImage> LoadImageAsync (ImageSource imageSource, CancellationToken cancelationToken, float scale)
		{
			var image = await mStreamImageSourceHandler
					.LoadImageAsync (imageSource, cancelationToken, scale);

			ImageSourceWatcher.GetImageLoadedHandler (imageSource)?
					.Invoke (imageSource, image != null);

			return image;
		}
	}
}
