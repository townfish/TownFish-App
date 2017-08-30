using System;

using Foundation;
using UIKit;

using TownFish.App.Helpers;
using TownFish.App.iOS.Helpers;


[assembly: Xamarin.Forms.Dependency (typeof (PlatformHelper))]


namespace TownFish.App.iOS.Helpers
{
	class PlatformHelper: IPlatformHelper
	{
		/// <summary>
		/// Opens the given URI without using <see cref="Device.OpenUri"/>, as it's broken
		/// on iOS.
		/// </summary>
		/// <param name="uri">The URI.</param>
		public void OpenUri (Uri uri)
		{
			// convert from original string, but if that fails concatenate component parts
			var url = NSUrl.FromString (uri.OriginalString) ??
					new NSUrl ($"{uri.Scheme}://{uri.Host}{uri.LocalPath}{uri.Query}");

			UIApplication.SharedApplication.OpenUrl (url);
		}
	}
}
