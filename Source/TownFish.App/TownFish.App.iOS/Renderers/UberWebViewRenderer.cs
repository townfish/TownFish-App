using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Foundation;
using TownFish.App.Controls;
using TownFish.App.iOS.Renderers;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(UberWebView), typeof(UberWebViewRenderer))]


namespace TownFish.App.iOS.Renderers
{
    /// <summary>
    /// Renders the <see cref="UberWebView"/> on iOS.
    /// </summary>
    public class UberWebViewRenderer: ViewRenderer<UberWebView, WKWebView>,
			IWKScriptMessageHandler, IWKNavigationDelegate, IWKUIDelegate,
			IUberWebViewRenderer
	{
		#region Nested Types

		class UberNavigationDelegate: WKNavigationDelegate
		{
			public UberNavigationDelegate (UberWebView parent,
					UberWebViewRenderer renderer)
			{
				mParent = parent;
				mRenderer = renderer;
			}

			public override void DecidePolicy (WKWebView webView,
					WKNavigationAction navigationAction,
					Action<WKNavigationActionPolicy> decisionHandler)
			{
				var result = WKNavigationActionPolicy.Allow;
				var request = navigationAction.Request;
				var isMainFrame = navigationAction.TargetFrame?.MainFrame ?? false;

				mCurrentUrl = request.Url.ToString();

				try
				{
					if (isMainFrame)
					{
						// if it's a GET and query param isn't present, add it & reload
						if (request.HttpMethod == "GET" &&
								mParent.AddQueryParam (mCurrentUrl, out string newUrl))
						{
							// create a mutable request so we can copy over the headers
							var req = new NSMutableUrlRequest (new NSUrl (newUrl))
									{ Headers = request.Headers };

							// load new request and cancel this one
							webView.LoadRequest (req);

							result = WKNavigationActionPolicy.Cancel;
						}
						else if (!mParent.OnNavigationStarting (mCurrentUrl))
						{
							// parent doesn't want to proceed
							result = WKNavigationActionPolicy.Cancel;
						}
						// (else fall through to allow the request)
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine (
							"UberNavigationDelegate.DecidePolicy - failed with: {0}", ex);
				}
				finally
				{
					decisionHandler (result);
				}
			}

			public override void DecidePolicy (WKWebView webView,
					WKNavigationResponse navigationResponse,
					Action<WKNavigationResponsePolicy> decisionHandler)
			{
				var response = navigationResponse.Response as NSHttpUrlResponse;
				var cookies = NSHttpCookie.CookiesWithResponseHeaderFields (
					response.AllHeaderFields, response.Url);

				// push WKWebView cookies to NSHttpCookieStorage
				if (cookies != null)
					foreach (var cookie in cookies)
						NSHttpCookieStorage.SharedStorage.SetCookie (cookie);

				decisionHandler (WKNavigationResponsePolicy.Allow);
			}

			public override void DidReceiveServerRedirectForProvisionalNavigation (
					WKWebView webView, WKNavigation navigation)
			{
				// no action required here; DON'T call base, which will throw!
			}

			public override void DidStartProvisionalNavigation (WKWebView webView,
					WKNavigation navigation)
			{
				mParent.OnNavigationStarted (webView.Url.ToString());
			}

			public override void DidFinishNavigation (WKWebView webView,
					WKNavigation navigation)
			{
				mRenderer.UpdateCanGoBackForward();

				// inject my post message function
				mParent.OnNavigationFinished (webView.Url.ToString(), cPostMessageScript);
			}

			/// <summary>
			/// When a request fails, raises the <see cref="UberWebView.NavigationFailed"/>
			/// event with an <see cref="ArgumentException"/> indicating the failure reason in
			/// the message and the error code in the parameter name.
			/// </summary>
			/// <param name="webView"></param>
			/// <param name="navigation"></param>
			/// <param name="error"></param>
			public override void DidFailNavigation (WKWebView webView,
					WKNavigation navigation, NSError error)
			{
				mParent.OnNavigationFailed (new UberWebView.NavigationException (
						error.LocalizedDescription, (int) error.Code,
						mCurrentUrl, new NSErrorException (error)));
			}

			UberWebView mParent;
			UberWebViewRenderer mRenderer;
			string mCurrentUrl;
		}

		class UberUIDelegate: WKUIDelegate
		{
			public override WKWebView CreateWebView (WKWebView webView,
					WKWebViewConfiguration configuration,
					WKNavigationAction navigationAction,
					WKWindowFeatures windowFeatures)
			{
				// handle target=_blank (popup) by opening in parent webview instead
				if (navigationAction.TargetFrame == null)
				{
					webView.LoadRequest (navigationAction.Request);
					return null;
				}

				return base.CreateWebView (webView, configuration, navigationAction, windowFeatures);
			}
		}

		#endregion Nested Types

		#region Construction

		/// <summary>
		/// Creates a new instance of <see cref="UberWebViewRenderer"/>.
		/// </summary>
		public UberWebViewRenderer() : base() {}

		#endregion Construction

		#region Methods

		#region IWKScriptMessageHandler implementation

		/// <summary>
		/// Raises <see cref="UberWebView.ScriptMessageReceived"/>.
		/// </summary>
		/// <param name="userContentController"></param>
		/// <param name="message"></param>
		public void DidReceiveScriptMessage (WKUserContentController userContentController,
				WKScriptMessage message)
		{
			Element.OnScriptMessageReceived (message.Body.ToString());
		}

		#endregion IWKScriptMessageHandler implementation

		#region IUberWebViewRenderer implementation

		/// <summary>
		/// Invokes the given script in the underlying WebView.
		/// </summary>
		/// <param name="script"></param>
		public void InvokeScript (string script)
		{
			Control?.EvaluateJavaScript (script, null);
		}

		#endregion IUberWebViewRenderer implementation

		#region ICookieStore implementation

		/// <summary>
		/// Gets cookies for all domains (if URL is null) or for a specific
		/// domain / URL.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		/// <remarks>
		/// URI can't be null on Android.
		/// </remarks>
		public IEnumerable<Cookie> GetCookies (Uri uri)
		{
			lock (mCookieLock)
			{
				foreach (var cookie in NSHttpCookieStorage.SharedStorage.Cookies)
				{
					Cookie c = null;

					try
					{
						var url = uri == null ? null : uri.Host + uri.AbsolutePath;
						if (url == null || url.StartsWith (cookie.Domain + cookie.Path))
						{
							var exp = cookie.ExpiresDate != null ?
									(DateTime) cookie.ExpiresDate :
									DateTime.MinValue;

							c = new Cookie
							{
								Comment = cookie.Comment,
								Domain = cookie.Domain,
								Expires = exp,
								HttpOnly = cookie.IsHttpOnly,
								Name = cookie.Name,
								Path = cookie.Path,
								Secure = cookie.IsSecure,
								Value = cookie.Value,
								Version = Convert.ToInt32 (cookie.Version)
							};
						}
					}
					catch
					{
						// TODO: log bad cookieness

						// failure is not an option; don't return bad ones
						continue;
					}

					yield return c;
				}
			}
		}

		/// <summary>
		/// Sets the cookies to be used for subsequent requests.
		/// </summary>
		/// <param name="cookieJar">The cookie jar.</param>
		public void SetCookies (CookieCollection cookieJar)
		{
			// TODO: test!

			var jar = NSHttpCookieStorage.SharedStorage;

			foreach (Cookie cookie in cookieJar)
				jar.SetCookie (new NSHttpCookie (cookie));
		}

		/// <summary>
		/// Clears cookies for given site/url.
		/// </summary>
		/// <param name="uri"></param>
		/// <exception cref="System.NotImplementedException"></exception>
		public void DeleteCookies (Uri uri)
		{
			var jar = NSHttpCookieStorage.SharedStorage;

			foreach (var cookie in jar.CookiesForUrl (uri))
				jar.DeleteCookie (cookie);

			NSUserDefaults.StandardUserDefaults.Synchronize();
		}

		#endregion ICookieStore implementation

		/// <summary>
		/// Connects and/or disconnects an <see cref="UberWebView"/> instance to this renderer.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnElementChanged (ElementChangedEventArgs<UberWebView> e)
		{
			base.OnElementChanged (e);

			var newEl = e.NewElement;
			if (Control == null && newEl != null)
			{
				NSHttpCookieStorage.SharedStorage.AcceptPolicy = NSHttpCookieAcceptPolicy.Always;

				mController = new WKUserContentController();

				var config = new WKWebViewConfiguration { UserContentController = mController };
				var webView = new WKWebView (Frame, config)
				{
					NavigationDelegate = new UberNavigationDelegate (newEl, this),
					UIDelegate = new UberUIDelegate()
				};

				SetNativeControl (webView);
			}

			if (e.OldElement is UberWebView oldEl)
			{
				mController.RemoveScriptMessageHandler ("uberWebViewPostMessage");

				oldEl.Cleanup();
			}

			if (newEl != null)
			{
				// hook us up!
				mUberWebView = newEl;
				mUberWebView.Renderer = this;

				mController.AddScriptMessageHandler (this, "uberWebViewPostMessage");

				if (!string.IsNullOrEmpty (newEl.CustomUserAgent))
					Control.CustomUserAgent = newEl.CustomUserAgent + " (iOS UberWebView)";

				// if source already set, pretend it was just set
				if (newEl.Source != null)
					Source = newEl.SourceUri;
			}
		}

		void UpdateCanGoBackForward()
		{
			mUberWebView.CanGoBack = Control?.CanGoBack ?? false;
			mUberWebView.CanGoForward = Control?.CanGoForward ?? false;
		}

		#endregion Methods

		#region Properties

		#region IUberWebViewRenderer

		/// <summary>
		/// Gets or sets the source URL.
		/// </summary>
		public Uri Source
		{
			get => Control?.Url;
			set => Control?.LoadRequest (new NSUrlRequest (value));
		}

		#endregion IUberWebViewRenderer

		#endregion Properties

		#region Fields

		const string cPostMessageScript = @"
			//----------------
			// inject iOS-specific post message implementation
			//
			if (UberWebView && UberWebView.setPostMessageFn) {
				UberWebView.setPostMessageFn (function (msg) {
					UberWebView.log ('iOS: posting \'' + msg + '\'');
					window.webkit.messageHandlers.uberWebViewPostMessage.postMessage (msg);
				});
			}
			//----------------
			";

		readonly object mCookieLock = new object();

		UberWebView mUberWebView;
		WKUserContentController mController;

		#endregion Fields
	}
}
