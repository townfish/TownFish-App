using System;
using System.Reflection;

using Xamarin.Forms;

using TownFish.App.Pages;
using TownFish.App.ViewModels;

using StreetHawkCrossplatform;


namespace TownFish.App
{
	public partial class App : Application
	{
		#region Construction

		public App (string deviceID)
		{
			DeviceID = deviceID;

			var vm = new BrowserPageViewModel();
			MainPage = new BrowserPage { BindingContext = vm };

			// now everything's wired up, kick off initial page load
			vm.SourceUrl = BaseUrl + StartPath + QueryString;
		}

		#endregion Construction

		#region Methods

		protected override void OnStart()
		{
			// Handle when your app starts

			InitStreetHawk();
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}

		public void OnBackButtonPressed()
		{
			BackButtonPressed?.Invoke (this, new EventArgs());
		}

		public void CloseApp()
		{
			AppCloseRequested?.Invoke (this, new EventArgs());
		}

		#region StreetHawk

		void InitStreetHawk()
		{
			// NOTE: this code is derived from the SH Xamarin demo here:
			// https://github.com/StreetHawkSDK/XamHawkDemoApp/blob/master/XamHawkDemo/XamHawkDemo/App.xaml.cs

			var shAnalytics = DependencyService.Get<IStreetHawkAnalytics>();
			var shBeacon = DependencyService.Get<IStreetHawkBeacon>();
			//var shFeeds = DependencyService.Get<IStreetHawkFeeds>();
			var shGeofence = DependencyService.Get<IStreetHawkGeofence>();
			var shLocations = DependencyService.Get<IStreetHawkLocations>();
			var shPush = DependencyService.Get<IStreetHawkPush>();

#if DEBUG
			//Optional: enable XCode console logs.
			shAnalytics.SetEnableLogs (true);
#endif

			shAnalytics.SetAppKey ("TownFish");

			// Initialize StreetHawk when App starts.
			//Mandatory: set app key and call init.
			shAnalytics.Init();

			shPush.RegisterForPushMessaging (GcmSenderID); // GCM push ID (Android only)
			//shPush.SetGcmSenderId(gcm); // appears to be obsolete in latest SH Xamarin SDK

			//Optional: iOS specific, set AppStore Id for upgrading or rating App.
			//shAnalytics.SetsiTunesId ("944344799");
			//Optional: if App is allowed to get advertising identifier, pass to SDK.
			//shAnalytics.SetAdvertisementId("BEE83220-9385-4B36-81E1-BF4305834093");

			//Optional: not enable location when launch, delay ask for permission. Below three  APIs are equivalent. 
			shBeacon.SetIsDefaultLocationServiceEnabled (true);
			shGeofence.SetIsDefaultLocationServiceEnabled (true);
			shLocations.SetIsDefaultLocationServiceEnabled (true);

			//Optional: not enable notification when launch, delay ask for permission.
			shPush.SetIsDefaultNotificationServiceEnabled (true);

			//Optional: callback when install register successfully.
			/*shAnalytics.RegisterForInstallEvent (delegate (string installId)
			{
				Device.BeginInvokeOnMainThread (() =>
				{
					MainPage.DisplayAlert ("Install register successfully: ", installId, "OK");
				});
			});

			//Optional: callback when open url.
			shAnalytics.RegisterForDeeplinkURL (delegate (string openUrl)
			{
				Device.BeginInvokeOnMainThread (() =>
				{
					MainPage.DisplayAlert ("Open url: ", openUrl, "OK");
				});
			});

			//Optional: Callback when enter or exit beacon.
			shBeacon.RegisterForBeaconStatus (delegate (SHBeaconObj beacon)
			{
				Device.BeginInvokeOnMainThread (() =>
				{
					string message = string.Format("uuid: {0}, major: {1}, minor: {2}, server id: {3}, inside: {4}.", beacon.uuid, beacon.major, beacon.minor, beacon.serverId, beacon.isInside);
					MainPage.DisplayAlert ("Enter/Exit beacon: ", message, "OK");
				});
			});

			//Optional: Callback when enter or exit geofence.
			shGeofence.RegisterForGeofenceStatus (delegate (SHGeofenceObj geofence)
			{
				Device.BeginInvokeOnMainThread (() =>
				{
					string message = string.Format("latitude: {0}, longitude: {1}, radius: {2}, server id: {3}, inside: {4}, title: {5}.", geofence.latitude, geofence.longitude, geofence.radius, geofence.serverId, geofence.isInside, geofence.title);
					MainPage.DisplayAlert ("Enter/Exit geofence: ", message, "OK");
				});
			});

			//Optional: Callback when new feeds are available.
			shFeeds.OnNewFeedAvailableCallback (delegate()
			{
				shFeeds.ReadFeedData (0, delegate (System.Collections.Generic.List<SHFeedObject> arrayFeeds, string error)
				{
					Device.BeginInvokeOnMainThread (() =>
					{
						if (error != null)
						{
							MainPage.DisplayAlert ("New feeds available but fetch meet error:", error, "OK");
						}
						else
						{
							string feeds = string.Empty;
							for (int i = 0; i < arrayFeeds.Count; i++)
							{
								SHFeedObject feed = arrayFeeds[i];
								feeds = string.Format ("Title: {0}; Message: {1}; Content: {2}. \r\n{3}", feed.title, feed.message, feed.content, feeds);
								shFeeds.SendFeedAck (feed.feed_id);
								shFeeds.NotifyFeedResult (feed.feed_id, 1);
							}
							MainPage.DisplayAlert (string.Format ("New feeds available and fetch {0}:", arrayFeeds.Count), feeds, "OK");
						}
					});
				});
			});*/

			//Optional: Callback when receive push notification payload.
			//shPush.OnReceivePushData(delegate (PushDataForApplication pushData)
			//		   {
			//			   Device.BeginInvokeOnMainThread(() =>
			//				   {
			//					   string message = string.Format("msgid: {0}; code: {1}; action: {2};\r\ntitle: {3}\r\nmessage: {4}\r\ndata: {5}", pushData.msgID, pushData.code, pushData.action, pushData.title, pushData.message, pushData.data);
			//					   MainPage.DisplayAlert("Show custom dialog:", message, "Continue");
			//						//Mandatory: Send push result.
			//						shPush.SendPushResult(pushData.msgID, SHPushResult.SHPushResult_Accept);
			//				   });
			//		   });

			//Optional: Callback when decide push result.
			//shPush.OnReceiveResult(delegate (PushDataForApplication pushData, SHPushResult result)
			//		   {
			//			   Device.BeginInvokeOnMainThread(() =>
			//				   {
			//					   string title = string.Format("Push result: {0}", result);
			//					   string message = string.Format("msgid: {0}; code: {1}; action: {2};\r\ntitle: {3}\r\nmessage: {4}\r\ndata: {5}", pushData.msgID, pushData.code, pushData.action, pushData.title, pushData.message, pushData.data);
			//					   MainPage.DisplayAlert(title, message, "OK");
			//				   });
			//		   });

			//Optional: Callback when receive json push.
			shPush.RegisterForRawJSON (delegate (string title, string message, string JSON)
			{
				Device.BeginInvokeOnMainThread (() =>
				{
					string msg = string.Format("title: {0}\r\nmessage: {1}\r\njson: {2}", title, message, JSON);
					MainPage.DisplayAlert ("Receive json push:", msg, "OK");
				});
			});

			//Optional: Callback when none 
			shPush.OnReceiveNonSHPushPayload (delegate (string payload)
			{
				Device.BeginInvokeOnMainThread (() =>
				{
					MainPage.DisplayAlert ("Receive none StreetHawk push:", payload, "OK");
				});
			});
		}

		#endregion StreetHawk

		#endregion Methods

		#region Properties and Events

		public event EventHandler BackButtonPressed;

		public event EventHandler AppCloseRequested;

		public static new App Current { get { return Application.Current as App; } }

		public static Assembly Assembly
		{
			get
			{
				if (sAssembly == null)
					sAssembly = typeof (App).GetTypeInfo().Assembly;

				return sAssembly;
			}
		}

		public bool CheckedCuid { get; set; }

		public string DeviceID { get; }

		#endregion Properties and Events

		#region Fields

		// all magic URLs and paths used in this app
		public const string SiteDomain = "dev.townfish.com";
		public const string BaseUrl = "http://" + SiteDomain;
		public const string StartPath = "/";
		public const string TermsUrl = BaseUrl + "/terms-of-use/";
		public const string SHCuidUrl = BaseUrl + "/profile/shcuid/{0}?syncToken={1}";
		public const string SHSyncUrl = BaseUrl + "/profile/shsync/{0}?syncToken={1}";

		public const string QueryParam = "mode=app"; // parameter added to every request
		public const string QueryString = "?" + QueryParam;

		public const string TwitterApiDomain = "api.twitter.com";

		public const string GcmSenderID = "7712235891";

		static Assembly sAssembly = null;

		#endregion Fields
	}
}
