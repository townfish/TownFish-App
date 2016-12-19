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
			var shFeeds = DependencyService.Get<IStreetHawkFeeds>();
			var shGeofence = DependencyService.Get<IStreetHawkGeofence>();
			var shLocations = DependencyService.Get<IStreetHawkLocations>();
			var shPush = DependencyService.Get<IStreetHawkPush>();

#if DEBUG
			//Optional: enable XCode console logs.
			shAnalytics.SetEnableLogs (true);
#endif

			shAnalytics.SetAppKey (StreetHawkAppKey);

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

			// start listening for beacons and geofences
			shBeacon.StartBeaconMonitoring();
			shGeofence.StartGeofenceMonitoring();

			//Optional: callback when install register successfully.
			shAnalytics.RegisterForInstallEvent (installId =>
			{
#if DEBUG
				Device.BeginInvokeOnMainThread (() =>
					MainPage.DisplayAlert ("Install register successfully: ", installId, "OK"));
#endif
			});

			//Optional: callback when open url.
			shAnalytics.RegisterForDeeplinkURL (openUrl =>
			{
#if DEBUG
				Device.BeginInvokeOnMainThread (() =>
					MainPage.DisplayAlert ("Open url: ", openUrl, "OK"));
#endif
			});

			//Optional: Callback when enter or exit beacon.
			shBeacon.RegisterForBeaconStatus (beacon =>
			{
#if DEBUG
				Device.BeginInvokeOnMainThread (() =>
					{
						var message = $"uuid: {beacon.uuid}, major: {beacon.major}, minor: {beacon.minor}, server id: {beacon.serverId}, inside: {beacon.isInside}.";
						MainPage.DisplayAlert ("Enter/Exit beacon: ", message, "OK");
					});
#endif
			});

			//Optional: Callback when enter or exit geofence.
			shGeofence.RegisterForGeofenceStatus (geofence =>
			{
#if DEBUG
				Device.BeginInvokeOnMainThread (() =>
					{
						var message = $"latitude: {geofence.latitude}, longitude: {geofence.longitude}, radius: {geofence.radius}, server id: {geofence.serverId}, inside: {geofence.isInside}, title: {geofence.title}.";
						MainPage.DisplayAlert ("Enter/Exit geofence: ", message, "OK");
					});
#endif
			});

			//Optional: Callback when new feeds are available.
			shFeeds.OnNewFeedAvailableCallback (() =>
			{
				shFeeds.ReadFeedData (0, (arrayFeeds, error) =>
				{
#if DEBUG
					Device.BeginInvokeOnMainThread (() =>
						{
							if (error != null)
							{
								MainPage.DisplayAlert ("New feeds available but fetch failed:", error, "OK");
							}
							else
							{
								var feeds = string.Empty;

								for (int i = 0; i < arrayFeeds.Count; i++)
								{
									var feed = arrayFeeds[i];
									feeds = $"Title: {feed.title}; Message: {feed.message}; Content: {feed.content}. \r\n{feeds}";
									shFeeds.SendFeedAck (feed.feed_id);
									shFeeds.NotifyFeedResult (feed.feed_id, 1);
								}

								MainPage.DisplayAlert ($"New feeds available and fetched {arrayFeeds.Count}:", feeds, "OK");
							}
						});
#endif
				});
			});

			//Optional: Callback when receive push notification payload.
			shPush.OnReceivePushData (pushData =>
				{
#if DEBUG
					Device.BeginInvokeOnMainThread (() =>
						{
							var message = $"msgid: {pushData.msgID}; code: {pushData.code}; action: {pushData.action};\r\ntitle: {									pushData.title}\r\nmessage: {pushData.message}\r\ndata: {pushData.data}";
							MainPage.DisplayAlert ("Push Data:", message, "Continue");
						});
#endif

					//Mandatory: Send push result.
					shPush.SendPushResult (pushData.msgID, 1);// SHPushResult.SHPushResult_Accept);
				});

			//Optional: Callback when decide push result.
			shPush.OnReceiveResult ((pushData, result) =>
			{
#if DEBUG
				Device.BeginInvokeOnMainThread(() =>
					{
						var title = $"Push Result: {result}";
						var message = $"msgid: {pushData.msgID}; code: {pushData.code}; action: {pushData.action};\r\ntitle: {									pushData.title}\r\nmessage: {pushData.message}\r\ndata: {pushData.data}";

						MainPage.DisplayAlert (title, message, "OK");
					});
#endif
			});

			//Optional: Callback when receive json push.
			shPush.RegisterForRawJSON ((title, message, Json) =>
				{
#if DEBUG
					Device.BeginInvokeOnMainThread (() =>
						{
							var msg = $"title: {title}\r\nmessage: {message}\r\nJSON: {Json}";
							MainPage.DisplayAlert ("Receive JSON push:", msg, "OK");
						});
#endif
				});

			//Optional: Callback when none 
			shPush.OnReceiveNonSHPushPayload (payload =>
			{
#if DEBUG
				Device.BeginInvokeOnMainThread (() =>
					MainPage.DisplayAlert ("Receive non-StreetHawk push:", payload, "OK"));
#endif
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

		public const string StreetHawkAppKey = "TownFish";

		static Assembly sAssembly = null;

		#endregion Fields
	}
}
