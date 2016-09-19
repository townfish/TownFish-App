using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;

using Foundation;
using UIKit;

using StreethawkIOS.Core;
using StreethawkIOS.Push;


namespace TownFish.App.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			#region StreetHawk

			SHApp.instance().appKey = "TownFish";
			SHApp.instance().enableLogs = true;
			SHApp.instance().iTunesId = ""; // TODO: insert iTunes ID
			SHApp.instance().streethawkinit();

			//Simply call one function of Push module to make sure linking to it.
			SHPush.instance().isDefaultNotificationEnabled = true;

			#endregion StreetHawk

			UIApplication.SharedApplication.SetStatusBarStyle (UIStatusBarStyle.LightContent, false);
			UIApplication.SharedApplication.SetStatusBarHidden (false, false);

			// reference these to make sure they're included by linker (?!)
			var load1 = new Xamify.UberWebViewLib.iOS.UberWebViewRenderer();
			var load2 = new Renderers.TownFishEntryRenderer();

			Forms.Init();
			LoadApplication (new App());

			return base.FinishedLaunching (app, options);
		}
	}
}
