using System;


namespace TownFish.App.Helpers
{
	public interface IPlatformHelper
	{
		/// <summary>
		/// Opens the given URI without using <see cref="Device.OpenUri"/> which
		/// is broken on iOS.
		/// </summary>
		/// <param name="uri">The URI.</param>
		void OpenUri (Uri uri);
	}
}
