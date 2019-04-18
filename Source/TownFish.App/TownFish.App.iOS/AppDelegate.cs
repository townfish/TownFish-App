using System;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using Foundation;
using UIKit;
using TownFish.App.iOS.Renderers;

namespace TownFish.App.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : FormsApplicationDelegate
	{
        bool IsLaunch = false;
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
            UIApplication.SharedApplication.SetStatusBarStyle (UIStatusBarStyle.LightContent, false);
			UIApplication.SharedApplication.SetStatusBarHidden (false, false);

            InitializeDependencies();

            string nativeParam = null;
            if (options != null)
            {
                if (options.ContainsKey(UIApplication.LaunchOptionsRemoteNotificationKey))
                {
                    NSDictionary userInfo = (NSDictionary)options[UIApplication.LaunchOptionsRemoteNotificationKey];
                    if (userInfo != null)
                    {
                        IsLaunch = true;
                        var notificationCustomPayload = userInfo["custom"] as NSDictionary;
                        if (notificationCustomPayload != null)
                        {
                            var customPayload = notificationCustomPayload["a"] as NSDictionary;
                            if (customPayload != null)
                            {
                                var finalPayload = customPayload["townfishPayload"] as NSObject;
                                if (finalPayload != null)
                                    nativeParam = finalPayload.ToString();
                            }
                        }
                    }
                }
            }

            // NOTE: .ToString on old (iPhone 4?) device returns runtime junk text before the UUID,
            // but .AsString seems to return just UUID text in all cases tested.
            var deviceID = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
			LoadApplication (new App (deviceID, dataFromNotification: nativeParam));

			return base.FinishedLaunching (app, options);
		}

        void InitializeDependencies()
        {
            // reference these to make sure they're included by linker (?!)
            var load1 = new UberWebViewRenderer();
            var load2 = new Renderers.TownFishEntryRenderer();

            Forms.Init();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
        }

        //public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        //{
        //    //ProcessNotification(userInfo, false);
        //    UIApplicationState state = application.ApplicationState;
        //    //if (state != UIApplicationState.Inactive && !IsLaunch)
        //    //if (!IsLaunch)
        //    //return;

        //    //if (userInfo == null)
        //    //return;

        //    var notificationCustomPayload = userInfo["custom"] as NSDictionary;
        //    if (notificationCustomPayload == null)
        //        return;

        //    var customPayload = notificationCustomPayload["a"] as NSDictionary;
        //    if (customPayload == null)
        //        return;

        //    var finalPayload = customPayload["townfishPayload"] as NSObject;
        //    if (finalPayload == null)
        //        return;

        //    var msg = finalPayload.ToString();        
        //}
    }
}
