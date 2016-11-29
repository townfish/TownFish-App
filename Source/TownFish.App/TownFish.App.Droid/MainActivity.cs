using System;

using Android.App;
using Android.Content.PM;
using static Android.Provider.Settings;
using Android.Runtime;
using Android.OS;
using Android.Views;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using StreetHawkCrossplatform;


namespace TownFish.App.Droid
{
	[Activity(Label = "TownFish.App", Icon = "@drawable/icon", Theme = "@style/townfishTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : FormsApplicationActivity
	{
		#region Methods

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			StreetHawkAnalytics.Init (this);

			Forms.Init (this, bundle);

			var deviceID = Secure.GetString (ApplicationContext.ContentResolver,
					Secure.AndroidId);

			mApp = new App (deviceID);
			mApp.AppCloseRequested += App_AppCloseRequested;

			LoadApplication (mApp);
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

		#endregion Fields
	}
}