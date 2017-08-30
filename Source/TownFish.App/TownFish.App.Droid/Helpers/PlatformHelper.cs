using System;

using TownFish.App.Helpers;
using TownFish.App.Droid.Helpers;

using Xamarin.Forms;


[assembly: Dependency (typeof (PlatformHelper))]


namespace TownFish.App.Droid.Helpers
{
	class PlatformHelper: IPlatformHelper
	{
		/// <summary>
		/// Opens the given URI using <see cref="Device.OpenUri"/>, as it's not broken
		/// on Android.
		/// </summary>
		/// <param name="uri">The URI.</param>
		public void OpenUri (Uri uri)
		{
			// this works on Android
			Device.OpenUri (uri);
		}
	}
}
