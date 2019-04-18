using System;
using System.Collections.Generic;
using System.Net;


namespace TownFish.App.Controls
{
	/// <summary>
	/// Defines the contract for cross-platform cookie access.
	/// </summary>
	public interface ICookieStore
	{
		/// <summary>
		/// Gets cookies for all domains (if URI is null) or for a specific
		/// domain / URI.
		/// </summary>
		/// <remarks>
		/// URI can't be null on Android.
		/// </remarks>
		IEnumerable<Cookie> GetCookies (Uri uri = null);

		/// <summary>
		/// Sets the cookies to be used for subsequent requests.
		/// </summary>
		/// <param name="cookieJar">The cookies.</param>
		void SetCookies (CookieCollection cookieJar);

		/// <summary>
		/// Deletes all cookies for given site/url.
		/// </summary>
		void DeleteCookies (Uri uri);
	}
}
