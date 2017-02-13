using System;
using System.Threading;
using System.Threading.Tasks;

using Android.Content;
using Android.Graphics;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using TownFish.App.Helpers;
using TownFish.App.Droid.Renderers;


[assembly: ExportImageSourceHandler (typeof (UriImageSource), typeof (UriImageSourceHandlerEx))]
[assembly: ExportImageSourceHandler (typeof (StreamImageSource), typeof (StreamImageSourceHandlerEx))]


namespace TownFish.App.Droid.Renderers
{
	public class UriImageSourceHandlerEx: IImageSourceHandler
	{
		ImageLoaderSourceHandler _imageLoaderSourceHandler;

		public UriImageSourceHandlerEx()
		{
			_imageLoaderSourceHandler = new ImageLoaderSourceHandler();
		}

		public async Task<Bitmap> LoadImageAsync (ImageSource imageSource, Context context, CancellationToken cancelationToken = default (CancellationToken))
		{
			var image = await _imageLoaderSourceHandler
					.LoadImageAsync (imageSource, context, cancelationToken);

			ImageSourceWatcher.GetImageLoadedHandler (imageSource)?
					.Invoke (imageSource, image != null);

			return image;
		}
	}

	public class StreamImageSourceHandlerEx: IImageSourceHandler
	{
		StreamImagesourceHandler _imageLoaderSourceHandler;

		public StreamImageSourceHandlerEx()
		{
			_imageLoaderSourceHandler = new StreamImagesourceHandler();
		}

		public async Task<Bitmap> LoadImageAsync (ImageSource imageSource, Context context, CancellationToken cancelationToken = default (CancellationToken))
		{
			var image = await _imageLoaderSourceHandler
					.LoadImageAsync (imageSource, context, cancelationToken);

			ImageSourceWatcher.GetImageLoadedHandler (imageSource)?
					.Invoke (imageSource, image != null);

			return image;
		}
	}
}
