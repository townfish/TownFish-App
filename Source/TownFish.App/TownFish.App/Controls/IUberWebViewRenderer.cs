using System;


namespace TownFish.App.Controls
{
	/// <summary>
	/// Defines the contract for <see cref="UberWebView"/> renderer.
	/// </summary>
	/// <seealso cref="ICookieStore" />
	public interface IUberWebViewRenderer: ICookieStore
	{
		/// <summary>
		/// Invokes the given script in the underlying WebView.
		/// </summary>
		void InvokeScript (string script);

		/// <summary>
		/// Gets or sets the source URL of the underlying WebView.
		/// </summary>
		Uri Source { get; set; }
	}
}
