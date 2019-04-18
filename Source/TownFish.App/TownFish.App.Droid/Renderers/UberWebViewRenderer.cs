using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Webkit;
using Java.Interop;
using TownFish.App.Controls;
using TownFish.App.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ExportRenderer(typeof(UberWebView), typeof(UberWebViewRenderer))]
namespace TownFish.App.Droid.Renderers
{
	/// <summary>
	/// Renders the <see cref="UberWebView"/> on Android.
	/// </summary>
	public class UberWebViewRenderer: WebViewRenderer, IUberWebViewRenderer
	{
		#region Nested Types

		class UberWebViewClient: WebViewClient
		{
			#region Construction

			public UberWebViewClient (UberWebView parent,
					UberWebViewRenderer renderer)
			{
				mParent = parent;
				mRenderer = renderer;
			}

			#endregion Construction

			#region Methods

			// disable this as we're still targeting older platforms
#pragma warning disable 672 // Member overrides obsolete member

			public override bool ShouldOverrideUrlLoading (Android.Webkit.WebView webView,
					string url)
			{
				mLastLoadFailed = false;

				try
				{
					// see if parent wants to add query param
					if (mParent.AddQueryParam (url, out string newUrl))
					{
						webView.LoadUrl (newUrl);

						return true;
					}

					// skip load altogether if parent cancels
					if (!mParent.OnNavigationStarting (url))
						return true;
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine (
							"UberWebViewClient.ShouldOverrideUrlLoading - failed with: {0}", ex);

					mLastLoadFailed = true;
				}

				return false;
			}

#pragma warning restore 672

			public override void OnPageStarted (Android.Webkit.WebView webView, string url,
					Bitmap favicon)
			{
				mParent.OnNavigationStarted (url);
				base.OnPageStarted (webView, url, favicon);
			}

			public override void OnPageFinished (Android.Webkit.WebView webView, string url)
			{
                // if load failed, ignore it to prevent re-installing load script
                if (!mLastLoadFailed)
                {
                    mRenderer.UpdateCanGoBackForward();

                    mParent.OnNavigationFinished (url, getPostScript());
                }

                base.OnPageFinished (webView, url);
			}

// disable these as we're still targeting older platforms
#pragma warning disable 672 // Member overrides obsolete member
#pragma warning disable 618 // Calling obsolete (base) method

			/// <summary>
			/// When an HTTP request fails, raises the
			/// <see cref="UberWebView.NavigationFailed"/> event with an
			/// <see cref="ArgumentException"/> indicating the error description in
			/// the message and the error code in the parameter name.
			/// </summary>
			/// <param name="webView">The web view.</param>
			/// <param name="errorCode">The error code.</param>
			/// <param name="description">The description.</param>
			/// <param name="failingUrl">The failing URL.</param>
			public override void OnReceivedError (Android.Webkit.WebView webView,
					[GeneratedEnum] ClientError errorCode,
					string description, string failingUrl)
			{
				mLastLoadFailed = true;

				mParent.OnNavigationFailed (new UberWebView.NavigationException (
						description, (int) errorCode, failingUrl));

				base.OnReceivedError (webView, errorCode, description, failingUrl);
			}

#pragma warning restore 618
#pragma warning restore 672

			/// <summary>
			/// When an HTTP error is received from the server, raises the
			/// <see cref="UberWebView.NavigationFailed"/> event with an
			/// <see cref="ArgumentException"/> indicating the error reason in
			/// the message and the status code in the parameter name.
			/// </summary>
			/// <param name="webView">The view.</param>
			/// <param name="request">The request.</param>
			/// <param name="errorResponse">The error response.</param>
			public override void OnReceivedHttpError (Android.Webkit.WebView webView,
					IWebResourceRequest request, WebResourceResponse errorResponse)
			{
				if (request.IsForMainFrame)
				{
					mLastLoadFailed = true;

					mParent.OnNavigationFailed (new UberWebView.NavigationException (
							errorResponse.ReasonPhrase, errorResponse.StatusCode,
							request.Url.ToString()));
				}

				base.OnReceivedHttpError (webView, request, errorResponse);
			}

            string getPostScript()
            {
                var builder = new StringBuilder();

                builder.Append("if (UberWebView && UberWebView.setPostMessageFn)");
                builder.Append("{");
                builder.Append("UberWebView.setPostMessageFn(function(msg) {");
                builder.Append("UberWebView.log('Android: posting \\'' + msg + '\\'');");
                builder.Append("jsBridge.uberWebViewPostMessage(msg);");
                builder.Append("});");
                builder.Append("}");

                return builder.ToString();
            }

            #endregion Metods

            #region Fields

            UberWebView mParent;
			UberWebViewRenderer mRenderer;
			bool mLastLoadFailed;

			#endregion Fields
		}

		class JSBridge: Java.Lang.Object
		{
			readonly WeakReference<UberWebViewRenderer> mWebViewRenderer;

			#region Construction

			public JSBridge (UberWebViewRenderer renderer)
			{
				mWebViewRenderer = new WeakReference<UberWebViewRenderer> (renderer);
			}

			#endregion Construction

			#region Methods

			[JavascriptInterface]
			[Export ("uberWebViewPostMessage")]
			public void UberWebViewPostMessage (string data)
			{
				if (mWebViewRenderer != null &&
						mWebViewRenderer.TryGetTarget (out UberWebViewRenderer renderer))
					(renderer.Element as UberWebView).OnScriptMessageReceived (data);
			}

			#endregion Methods
		}

		#endregion Netsted Types

		#region Construction

		/// <summary>
		/// Creates a new instance of <see cref="UberWebViewRenderer"/>.
		/// </summary>
		/// <param name="context"></param>
		public UberWebViewRenderer (Context context) : base (context) {}

		#endregion Construction

		#region Methods

		#region IUberWebViewRenderer implementation

		/// <summary>
		/// Invokes the given script in the underlying WebView.
		/// </summary>
		/// <param name="script"></param>
		public void InvokeScript (string script)
		{
			mUberWebView?.Eval(script);
		}

		#endregion IUberWebViewRenderer implementation

		#region ICookieStore implementation

		/// <summary>
		/// Gets the get cookies.
		/// </summary>
		public IEnumerable<Cookie> GetCookies (Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException (nameof (uri),
					"On Android, 'uri' cannot be empty. " +
					"Please provide a URI when getting cookies");
			}

#if false
			// test cookie parser
			var x = ParseCookie ("cookieName=cookie value; expires=Mon, " +
					"24-Oct-2016 13:02:11 GMT; Secure;Max-Age=1200; path=/; " +
					"httponly; commenturi=http://foo.bar/bleh");
#endif

			lock (mCookieLock)
			{
				var allCookies = CookieManager.Instance.GetCookie (uri.ToString());
				if (string.IsNullOrEmpty (allCookies))
					yield break; // nothing to see here; move along

				// split out multiple cookies, if any
				var cookies = allCookies.Split (';');

				foreach (var cookie in cookies)
				{
					var nameAndValue = cookie.Split ('=');

					// if no '=', it's invalid
					if (nameAndValue.Length < 2)
						throw new Exception (string.Format (
								"Cookie '{0}' has no '='!", cookie));

					// spit it out
					yield return new Cookie (
							nameAndValue [0].TrimStart(), nameAndValue [1]);
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

			var jar = CookieManager.Instance;

			foreach (Cookie cookie in cookieJar)
				jar.SetCookie (cookie.Domain + cookie.Path, string.Format (
						"{0}={1}; expires={2}; Domain={3}; Path={4}{5}{6}",
						cookie.Name, cookie.Value, cookie.Expires.ToString ("R"),
						cookie.Domain, cookie.Path,
						cookie.Secure ? "; Secure" : "",
						cookie.HttpOnly ? "; HttpOnly" : ""));
		}

		/// <summary>
		/// Clears cookies for given site/url.
		/// </summary>
		/// <param name="uri"></param>
		public void DeleteCookies (Uri uri)
		{
			// TODO remove specific cookies by name
			CookieManager.Instance.RemoveAllCookie();
		}

		#endregion ICookieStore implementation

		/// <summary>
		/// Connects and/or disconnects an <see cref="UberWebView"/> instance to this renderer.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnElementChanged (ElementChangedEventArgs<Xamarin.Forms.WebView> e)
		{
			base.OnElementChanged (e);

			if (e.OldElement is UberWebView oldEl)
			{
				Control.RemoveJavascriptInterface ("jsBridge");

				oldEl.Cleanup();
			}

			if (e.NewElement is UberWebView newEl)
			{
				// hook us up!
				mUberWebView = newEl;
				mUberWebView.Renderer = this;

				Control.Settings.JavaScriptEnabled = true;
				Control.AddJavascriptInterface (new JSBridge (this), "jsBridge");
				Control.SetWebViewClient (new UberWebViewClient (newEl, this));

				if (!string.IsNullOrEmpty (newEl.CustomUserAgent))
					Control.Settings.UserAgentString = newEl.CustomUserAgent +
							" (Android UberWebView)";
			}
		}

		void UpdateCanGoBackForward()
		{
			mUberWebView.CanGoBack = Control?.CanGoBack() ?? false;
			mUberWebView.CanGoForward = Control?.CanGoForward() ?? false;
		}


#if false
		/// <summary>
		/// Parses a cookie (not used; only here in case we ever need to do this).
		/// </summary>
		/// <param name="cookieString">The cookie string.</param>
		/// <returns></returns>
		/// <exception cref="System.Exception"></exception>
		Cookie ParseCookie (string cookieString)
		{
			if (string.IsNullOrEmpty (cookieString))
				return null;

			// split out attributes, if any
			var attributes = cookieString.Split (';');

			// the first pair is always the cookie's name & value
			// (attr 0 always exists due to null check on cookies string above)

			var nameAndValue = attributes [0].Split ('=');
			if (nameAndValue.Length < 2)
				throw new Exception (string.Format (
						"First pair in cookie '{0}' has no '='!", attributes [0]));

			// create the minimal cookie
			var cookie = new Cookie (nameAndValue [0], nameAndValue [1]);

			// now add attributes, if any
			for (var a = 1; a < attributes.Length; a++)
			{
				// limit split to 2 items in case value itself contains '=',
				var attr = attributes [a].Split (new[] { '=' }, 2);

				// trim space from start of attr name as space normally follows
				// ';' in cookie string

				var propName = attr [0].TrimStart();
				var prop = sCookieType.GetProperty (propName, BindingFlags.Public |
						 BindingFlags.Instance | BindingFlags.IgnoreCase);

				if (prop != null)
				{
					try
					{
						object val = null;

						if (attr.Length < 2 && prop.PropertyType == sBoolType)
						{
							// special-case bools with no value; always true
							val = true;
						}
						else if (prop.PropertyType == sUriType)
						{
							// special-case URI as ChangeType can't handle it
							Uri uri;
							Uri.TryCreate (attr [1], UriKind.Absolute, out uri);
							val = uri;
						}
						else
						{
							// all others, try this
							val = Convert.ChangeType (attr [1], prop.PropertyType);
						}

						prop.SetValue (cookie, val);
					}
					catch {} // TODO: log unhandled attribute type
				}
			}

			return cookie;
		}
#endif

		#endregion Methods

		#region Properties

		#region IUberWebViewRenderer

		/// <summary>
		/// Gets or sets the source URL.
		/// </summary>
		public Uri Source
		{
			get => Control?.Url == null ? null : new Uri (Control.Url);
			set => Control?.LoadUrl (value.ToString());
		}

		#endregion IUberWebViewRenderer

		#endregion Properties

		#region Fields

		//const string cPostMessageScript = @"
			////----------------
			//// inject Android-specific post message implementation
			////
			//if (UberWebView && UberWebView.setPostMessageFn) {
			//	UberWebView.setPostMessageFn (function (msg) {
			//		UberWebView.log ('Android: posting \'' + msg + '\'');
			//		jsBridge.uberWebViewPostMessage (msg);
			//	});
			//}
			////----------------
			//";

#if false
		static readonly Type sCookieType = typeof (Cookie);
		static readonly Type sBoolType = typeof (bool);
		static readonly Type sUriType = typeof (Uri);
#endif

		readonly object mCookieLock = new object();

		UberWebView mUberWebView;

		#endregion Fields
	}
}
