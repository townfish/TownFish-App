using System;

using Xamarin.Forms;


namespace TownFish.App.Helpers
{
	class PlatformHelper
	{
		#region Methods

		/// <summary>
		/// Opens the given URI without using <see cref="Device.OpenUri"/>, which
		/// is broken on iOS.
		/// </summary>
		/// <param name="uri">The URI.</param>
		public static void OpenUri (Uri uri)
		{
			sPlatformHelper.OpenUri (uri);
		}

		#endregion Methods

		#region Fields

		static IPlatformHelper sPlatformHelper =
				DependencyService.Get<IPlatformHelper> (DependencyFetchTarget.GlobalInstance);

		#endregion Fields
	}
}
