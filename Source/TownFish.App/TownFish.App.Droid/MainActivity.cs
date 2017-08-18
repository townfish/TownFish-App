//#define SH_NO_FEED

using System;

using Android.App;
using Android.Content.PM;
using static Android.Provider.Settings;
using Android.Runtime;
using Android.OS;
using Android.Views;
using Android.Webkit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Content;
using Android.Util;

#if !SH_NO_FEED
using Com.Streethawk.Library.Feeds;
#endif

using StreetHawkCrossplatform;

namespace TownFish.App.Droid
{
	[Activity (Label = "TownFish.App", Icon = "@drawable/icon",
        Theme = "@style/townfishTheme", MainLauncher = false, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
		ScreenOrientation = ScreenOrientation.UserPortrait,
		// in case this helps - looks like needs to be set programmatically in XF
		// due to https://bugzilla.xamarin.com/show_bug.cgi?id=39765
		WindowSoftInputMode = SoftInput.AdjustResize)]
	public class MainActivity : FormsApplicationActivity
	{
		#region Nested Types

#if DEBUG
		#region WebChromeClient

		class WebPlayerWebChromeClient: WebChromeClient
		{
			public override bool OnJsAlert (Android.Webkit.WebView view, string url, string message, JsResult result)
			{
				new AlertDialog.Builder (view.Context)
					.SetTitle ("javaScript dialog")
					.SetMessage (message)
					.SetPositiveButton (Android.Resource.String.Ok, (dlg, dcea) => result.Confirm ())
					.SetCancelable (false)
					.Create()
					.Show();

				return true;
			}
		}

        #endregion WebChromeClient
#endif
        #endregion Nested Types

        #region Methods

        protected override void OnPause()
        {
            Log.Info("MainActivity", "OnPause");
            IsActive = false;
            base.OnPause();
        }

        protected override void OnResume()
        {
            Log.Info("MainActivity", "OnResume");
            IsActive = true;
            base.OnResume();
        }

        protected override void OnNewIntent(Intent intent)
        {
            try
            {
                if (intent != null)
                {
                    var json = intent.GetStringExtra("json");
                    if (json != null)
                    {
                        App.Current.LaunchedFromNotification(json);
                    }
                }
            }
            catch { }
        }

        protected override void OnCreate (Bundle bundle)
		{
            IsActive = true;
			base.OnCreate (bundle);

#if DEBUG
			if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
				Android.Webkit.WebView.SetWebContentsDebuggingEnabled (true);
#endif

			Forms.Init (this, bundle);

			StreetHawkAnalytics.Init (this);

#if !SH_NO_FEED
			// separately initialise SH feeds
			var shFeeds = DependencyService.Get<ISHFeedItemObserver>();
			SHFeedItem.GetInstance (this).RegisterFeedItemObserver (shFeeds);
#endif // !SH_NO_FEED

			var deviceID = Secure.GetString (ApplicationContext.ContentResolver,
					Secure.AndroidId);

			mApp = new App (deviceID);
			mApp.AppCloseRequested += App_AppCloseRequested;

			LoadApplication (mApp);
            OnNewIntent(Intent);
        }

        void App_AppCloseRequested (object sender, EventArgs e)
		{
			Finish();
		}

		public override void OnBackPressed()
		{
			// Hardware Back
			mApp.OnBackButtonPressed();
		}

		public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back)
			{
				mApp.OnBackButtonPressed();
				return true;
			}
			else
			{
				return base.OnKeyDown(keyCode, e);
			}
		}

		#endregion Methods

		#region Fields

		App mApp;

        public static bool IsActive { get; set; } = false;

		#endregion Fields
	}
}