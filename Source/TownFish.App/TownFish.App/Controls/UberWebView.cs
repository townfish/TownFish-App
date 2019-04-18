using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xamarin.Forms;


namespace TownFish.App.Controls
{
	/// <summary>
	/// Implements an enhanced <see cref="WebView"/> with error handling and optional
	/// auto-addition of query parameters
	/// </summary>
	/// <seealso cref="Xamarin.Forms.WebView" />
	public class UberWebView: WebView
	{
		#region Nested Types

		/// <summary>
		/// Provides an exception wrapper for platform-specific navigation errors.
		/// </summary>
		/// <remarks>
		/// The platform-specific exception may be provided in the
		/// <see cref="Exception.InnerException"/> property.
		/// </remarks>
		/// <seealso cref="System.Exception" />
		public class NavigationException: Exception
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="NavigationException"/> class.
			/// </summary>
			/// <param name="message">The message.</param>
			/// <param name="errorCode">The optional error code.</param>
			/// <param name="url">The URL.</param>
			/// <param name="innerException">The platform-specific exception, if available.</param>
			public NavigationException (string message, int errorCode = 0,
					string url = null, Exception innerException = null):
				base (message, innerException)
			{
				Url = url;
				ErrorCode = errorCode;
			}

			/// <summary>
			/// Gets the failing URL.
			/// </summary>
			public string Url { get; private set; }

			/// <summary>
			/// Gets the error code.
			/// </summary>
			public int ErrorCode { get; private set; }
		}

		#endregion Nested Types

		#region Methods

		/// <summary>
		/// Invokes the given script in the underlying native WebView.
		/// </summary>
		/// <param name="script">The script to invoke.</param>
		public void InvokeScript (string script)
		{
			Renderer?.InvokeScript (script);
		}

		/// <inheritdoc />
		protected override void OnPropertyChanged (string propertyName)
		{
			// let those interested know if source changes
			if (propertyName == "Source")
			{
				// we don't get a chance to add our param if we are setting the source,
				// so let's add it here

				if (AddQueryParam (SourceUrl, out string newUrl))
				{
					// this will re-enter me (ooh-err!) but fall through as param is present
					Source = newUrl;

					return;
				}

				// now tell renderer the new source so it can actually go there!
				if (Renderer != null)
					Renderer.Source = SourceUri;
			}

			base.OnPropertyChanged (propertyName);
		}

		/// <summary>
		/// Called when a new navigation has started.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns>True to allow navigation, false to cancel.</returns>
		public bool OnNavigationStarting (string url)
		{
			var ea = new WebNavigatingEventArgs (WebNavigationEvent.NewPage, Source, url);

			NavigationStarting?.Invoke (this, ea);

			return !ea.Cancel;
		}

		public void OnNavigationStarted (string url)
		{
			NavigationStarted?.Invoke (this, url);
		}

		public void OnNavigationFinished (string url, string postMessageScript)
		{
			var actionList = "";
			if (Actions?.Count > 0)
				foreach (var kvp in Actions)
					actionList += string.Format (cActionFormat, kvp.Key, kvp.Value);

            // inject my scripts & user's loaded script
            //Renderer?.InvokeScript (cScriptStart + actionList + cScriptEnd +
            //postMessageScript + "\n// start page loaded script\n" +
            //PageLoadedScript + "\n// end page loaded script\n");

            var fullScript = getStart() +actionList +getEnd() +postMessageScript +PageLoadedScript;

            Renderer?.InvokeScript(fullScript);
            //Renderer?.InvokeScript(postMessageScript);
            //Renderer?.InvokeScript(PageLoadedScript);

            NavigationFinished?.Invoke (this, url);
		}

		public void OnNavigationFailed (NavigationException ex)
		{
			NavigationFailed?.Invoke (this, ex);
		}

		public void OnScriptMessageReceived (string data)
		{
			ScriptMessageReceived?.Invoke (this, data);
		}

		public void Cleanup()
		{
			ScriptMessageReceived = null;
			Renderer = null;
		}

		public bool AddQueryParam (string url, out string newUrl)
		{
			// quickly check first if there's a param to add
			if (!string.IsNullOrEmpty (QueryParam))
			{
				var uri = new Uri (url);
				var scheme = uri.Scheme.ToUpper();
				var query = uri.Query ?? "";

				if ((scheme == "HTTP" || scheme == "HTTPS") &&
						(string.IsNullOrEmpty (QueryParamDomain) ||
								uri.Host.EndsWith (QueryParamDomain)) &&
						!query.Contains (QueryParam))
				{
					var baseUrl = uri.Scheme + "://" + uri.Host + uri.LocalPath;
					var fragment = uri.Fragment ?? "";

					query += (query.Contains ("?") ? "&" : "?") + QueryParam;
					newUrl = baseUrl + query + fragment;

					return true;
				}
			}

			newUrl = url;

			return false;
		}

		#endregion Methods

		#region Events & Properties

		/// <summary>
		/// Occurs when navigation is about to start, giving subscribers the option to cancel.
		/// </summary>
		public event EventHandler<WebNavigatingEventArgs> NavigationStarting;

		/// <summary>
		/// Occurs when navigation has actually started.
		/// </summary>
		public event EventHandler<string> NavigationStarted;

		/// <summary>
		/// Occurs when navigation has finished successfully.
		/// </summary>
		public event EventHandler<string> NavigationFinished;

		/// <summary>
		/// Occurs when navigation has failed.
		/// </summary>
		/// <remarks>
		/// The <see cref="Exception.Message"/> property contains the navigation failure
		/// reason or description and the <see cref="NavigationException.ErrorCode"/>
		/// property contains the platform-specific error code or HTTP status value.
		/// </remarks>
		public event EventHandler<NavigationException> NavigationFailed;

		/// <summary>
		/// Occurs when a script message is received.
		/// </summary>
		public event EventHandler<string> ScriptMessageReceived;

		/// <summary>
		/// Gets <see cref="WebView.Source"/> as a string.
		/// </summary>
		public string SourceUrl => (Source as UrlWebViewSource).Url;

		/// <summary>
		/// Gets <see cref="WebView.Source"/> as a URI.
		/// </summary>
		public Uri SourceUri => new Uri (SourceUrl);

		/// <summary>
		/// Indicates whether the user can navigate to previous pages.
		/// </summary>
		/// <remarks>
		/// Note: this is set by the renderer after each navigation completes.
		/// </remarks>
		public new bool CanGoBack { get; set; }

		/// <summary>
		/// Indicates whether the user can navigate forward.
		/// </summary>
		/// <remarks>
		/// Note: this is set by the renderer after each navigation completes.
		/// </remarks>
		public new bool CanGoForward { get; set; }

		/// <summary>
		/// Gets or sets the query parameter(s) to add to all requests.
		/// </summary>
		public string QueryParam { get; set; }

		/// <summary>
		/// Gets or sets the domain for which the query parameter will be added, or
		/// leave empty to add parameter to queries for all domains.
		/// </summary>
		public string QueryParamDomain { get; set; }

		/// <summary>
		/// Gets or sets the script to inject on each page load.
		/// </summary>
		public string PageLoadedScript { get; set; } = "";

		/// <summary>
		/// Collection of action methods to invoke on certain actions, where key is the action
		/// and value is the method to invoke on that action occurring.
		/// </summary>
		public Dictionary<string, string> Actions { get; set; }

		/// <summary>
		/// Gets or sets the custom user agent string, to which " ([platform] UberWebView)"
		/// is appended, e.g. "my.app.name (Android UberWebView)" or
		/// "My Super App (iOS UberWebView)".
		/// </summary>
		public string CustomUserAgent { get; set; }

		/// <summary>
		/// Set by the renderer to allow direct calling
		/// via <see cref="IUberWebViewRenderer"/>.
		/// </summary>
		public IUberWebViewRenderer Renderer { set; private get; }

		/// <summary>
		/// Gets the cookies.
		/// </summary>
		public CookieCollection Cookies
		{
			get
			{
				var jar = new CookieCollection();

				foreach (var cookie in Renderer.GetCookies (SourceUri))
					jar.Add (cookie);

				return jar;
			}

			set { Renderer.SetCookies (value); }
		}

		#endregion Events & Properties

		#region Fields

        string getStart()
        {
            var start = new StringBuilder();
            start.Append("if (UberWebView && UberWebView.dispose)");
            start.Append("{ UberWebView.dispose(); }");
            start.Append("var UberWebView = (function() {");
            start.Append("var actions = {");
            return start.ToString();
        }

        string getEnd()
        {
            var end = new StringBuilder();
            end.Append("};");

            //end.AppendLine("// capture action's result as JSON into a JSON message object");
            end.Append("var getPostMessage = function (action, result) {");
            end.Append("return JSON.stringify ({");
            end.Append("'action': action,");
            end.Append("'result': JSON.stringify (result)");
            end.Append("});");
            end.Append("};");

            //end.AppendLine("// placeholder to be replaced by platform-specific code");
            end.Append("var postMessageFn = function() { };");

            end.Append("var log = function(msg) {");
            end.Append("console.log(new Date().getTime() + ': UberWebView: ' + msg);");
            end.Append("};");

            end.Append("var dispose = function() {");
            end.Append("log('Disposing');");
            end.Append("};");

            end.Append("var onAction = function(action, value) {");
            end.Append("for (var key in actions)");
            end.Append("{");
            end.Append("if (actions.hasOwnProperty(key) && key == action)");
            end.Append("{");
            end.Append("if (!value) value = actions[key]();");
            end.Append("postMessageFn(getPostMessage(action, value));");
            end.Append("break;");
            end.Append("}");
            end.Append("}");
            end.Append("};");

            end.Append("var setPostMessageFn = function(fn) {");
            end.Append("postMessageFn = fn;");
            end.Append("};");

            //end.AppendLine("// return my public interface");
            end.Append("return {");
            end.Append("log: log,");
            end.Append("dispose: dispose,");
            end.Append("onAction: onAction,");
            end.Append("setPostMessageFn: setPostMessageFn");
            end.Append("};");

            end.Append("})();");

            return end.ToString();
        }

		// script action handler
		const string cScriptStart = @"

///////////////////////////////////////
//
// JavaScript bridge for UberWebView
//
// To invoke an action, call onAction ('someAction');
//

// unhook previous instance, if any
if (UberWebView && UberWebView.dispose)
	UberWebView.dispose();

var UberWebView = (function() {
	//
	// private fields
	//

	//var actions = {";
		const string cActionFormat = "\n		'{0}': function() {{ return {1}; }},";
        //const string cScriptEnd = "\n       _: null" + @"
		const string cScriptEnd = @"
	//};

	//
	// private functions
	//

	// capture action's result as JSON into a JSON message object
	var getPostMessage = function (action, result) {
		return JSON.stringify ({
			'action': action,
			'result': JSON.stringify (result)
		});
	}

	// placeholder to be replaced by platform-specific code
	var postMessageFn = function() {}

	//
	// public functions
	//

	var log = function (msg) {
		console.log (new Date().getTime() + ': UberWebView: ' + msg);
	}

	var dispose = function() {
		log ('Disposing');

		// unhook events, etc.
		// ...
	}

	var onAction = function (action, value) {
		for (var key in actions) {
			if (actions.hasOwnProperty (key) && key == action) {
				// if no value provided, execute action's function to get a result
				if (!value) value = actions [key]();

				postMessageFn (getPostMessage (action, value));
				break;
			}
		}
	}

	var setPostMessageFn = function (fn) {
		postMessageFn = fn;
	}

	// return my public interface
	return {
		log: log,
		dispose: dispose,
		onAction: onAction,
		setPostMessageFn: setPostMessageFn
	};

})();
";

		#endregion Fields
	}
}
