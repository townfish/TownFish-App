using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Views;

using Com.Streethawk.Library.Core;
using Com.Streethawk.Library.Push;


namespace TownFish.App.Droid
{
	[Activity(Label = "TownFish.App", Icon = "@drawable/icon", Theme = "@style/townfishTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		#region Methods

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			StreetHawk.Instance.SetAppKey ("TownFish");
			StreetHawk.Instance.Init (Application);
			Push.GetInstance (this).RegisterForPushMessaging (""); // TODO: add Google Project Number

			global::Xamarin.Forms.Forms.Init(this, bundle);

			mApp = new App();
			mApp.AppCloseRequested += App_AppCloseRequested;
			LoadApplication(mApp);
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