//#define NOSTREETHAWKFEED
//#define DUMMYFEED

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;

using Newtonsoft.Json;

using StreetHawkCrossplatform;

using TownFish.App.Helpers;
using TownFish.App.Pages;
using TownFish.App.ViewModels;


namespace TownFish.App
{
	public partial class App: Application
	{
		#region Construction

		public App (string deviceID)
		{
			DeviceID = deviceID;

			MainPage = new BrowserPage
			{
				BindingContext = new BrowserPageViewModel
				{
					SourceUrl = BaseUrl + StartPath + QueryString
				}
			};
		}

		#endregion Construction

		#region Methods

		public void OnPushUrlReceived (string route)
		{
			PushUrlReceived?.Invoke (this, route);
		}

		protected override void OnStart()
		{
			// Handle when your app starts

			// for now, don't use StreetHawk on Android
			if (Device.RuntimePlatform != Device.Android)
				InitStreetHawk();
		}

		protected override void OnSleep()
		{
			AppSuspended?.Invoke (this, EventArgs.Empty);
		}

		protected override void OnResume()
		{
			AppResumed?.Invoke (this, EventArgs.Empty);
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

		async void InitStreetHawk()
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
			//shPush.SetGcmSenderId (GcmSenderID); // appears to be obsolete in latest SH Xamarin SDK

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

			//----------------------------------------------------------------
			// Apparently, hooking these stops them from working. Apparently.
			//
#if SH_OPTIONAL

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

#endif // SH_OPTIONAL
			//
			//----------------------------------------------------------------

#if TRUE // SH_RAWJSON_OPTIONAL
			//Optional: Callback when receive json push.
			shPush.RegisterForRawJSON (async (title, message, json) =>
				{
#if DEBUG
					Device.BeginInvokeOnMainThread (() =>
						{
							var msg = $"title: {title}\r\nmessage: {message}\r\nJSON: {json}";
							MainPage.DisplayAlert ("Receive JSON push:", msg, "OK");
						});
#endif
					// TODO: some magic to determine if this notification is related to a new entry
					// in the message feed, in which case we need to show the message feed (see
					// Item 15 on Basecamp TODO list).

					try
					{
						var content = JsonConvert.DeserializeObject<Dictionary<string, string>> (json);
						if (content.TryGetValue ("dataType", out var dataType) &&
								content.TryGetValue ("route", out var route) &&
								dataType == "vanilla_notification" &&
								route.ToLower().Contains ("townfish.com"))
							OnPushUrlReceived (route);

						// JSON push messages might be added to feed, so use this to trigger
						// a feed update

						SHFeedItems = await GetSHFeed();
					}
					catch {}
				});
#endif // SH_RAWJSON_OPTIONAL

			//----------------------------------------------------------------
			//
#if SH_OPTIONAL

			//Optional: Callback when none 
			shPush.OnReceiveNonSHPushPayload (payload =>
			{
#if DEBUG
				Device.BeginInvokeOnMainThread (() =>
					MainPage.DisplayAlert ("Receive non-StreetHawk push:", payload, "OK"));
#endif
			});

#endif // SH_OPTIONAL
			//
			//----------------------------------------------------------------

#if NOSTREETHAWKFEED
			SHFeedItems = await GetSHFeed();
#else
			// Apparently this isn't needed if we're using RegisterForRawJSON, and isn't
			// real-time anyway, and possibly blocks other push notifications from showing (?!)
			//shFeeds.OnNewFeedAvailableCallback (async() => SHFeedItems = await GetSHFeed());

			// however, on app load, we need to get feed anyway to show count and get items
			SHFeedItems = await GetSHFeed();
#endif
		}

		public static Task<IList<FeedItemViewModel>> GetSHFeed()
		{
			if (++sGettingSHFeed > 1)
			{
				--sGettingSHFeed;
				return null;
			}

			var tcs = new TaskCompletionSource<IList<FeedItemViewModel>>();
			var shFeeds = DependencyService.Get<IStreetHawkFeeds>();

#if NOSTREETHAWKFEED

#if DUMMYFEED

			// create dummy feed model
			var items = new List<FeedItemViewModel>
			{
				new FeedItemViewModel
				{
					PictureUrl = "TownFish.App.Images.Dummy.muffin.png",
					LinkUrl = "http://google.com/",
					Title = "Free Sausage & Egg Muffin",
					Text = "Bill's Brekkie Bar, Camden. Valid for Today Only!",
					TimeStamp = DateTime.Now,
					Group = "Restaurants & Dining"
				},
				new FeedItemViewModel
				{
					PictureUrl = "TownFish.App.Images.Dummy.meat.png",
					LinkUrl = "http://google.com/",
					Title = "Côte de Boeuf for two people with sides for only £20!",
					Text = "Hawksmoor Steak house in Covent Garden",
					TimeStamp = DateTime.Now,
					Group = "Restaurants & Dining"
				},
				new FeedItemViewModel
				{
					PictureUrl = "TownFish.App.Images.Dummy.muffin.png",
					LinkUrl = "http://google.com/",
					Title = "Free Sausage & Egg Muffin",
					Text = "Bill's Brekkie Bar, Camden. Valid for Today Only!",
					TimeStamp = DateTime.Now,
					Group = "Restaurants & Dining"
				},
				new FeedItemViewModel
				{
					PictureUrl = "TownFish.App.Images.Dummy.meat.png",
					LinkUrl = "http://google.com/",
					Title = "Côte de Boeuf for two people with sides for only £20!",
					Text = "Hawksmoor Steak house in Covent Garden",
					TimeStamp = DateTime.Now,
					Group = "Restaurants & Dining"
				}
			};

#else // !DUMMYFEED (i.e. empty dummy feed)

			var items = new List<FeedItemViewModel>();

#endif // !DUMMYFEED

			Task.Run (() => tcs.TrySetResult (items));

#else // !NOSTREETHAWKFEED

			// if it takes too long to return (call my callback below), give up to prevent leaks
			var cts = new CancellationTokenSource (5000);
			cts.Token.Register (() => tcs.TrySetException (new TimeoutException()));

			try
			{
				shFeeds.ReadFeedData (0, (feedItems, error) =>
					{
						// now we're back from callback we can cancel the timeout
						cts.Dispose();

						ProcessSHFeed (tcs, shFeeds, feedItems, error);
					});
			}
			catch (Exception ex)
			{
				tcs.TrySetException (ex);
			}

#endif // !NOSTREETHAWKFEED

			--sGettingSHFeed;
			return tcs.Task;
		}

		public static void ProcessSHFeed (TaskCompletionSource<IList<FeedItemViewModel>> tcs,
				IStreetHawkFeeds shFeeds, List<SHFeedObject> feedItems, string error)
		{
			try
			{
				if (error != null)
				{
#if DEBUG
					Current.MainPage.DisplayAlert ("New feeds available but fetch failed:", error, "OK");
#endif
					throw new Exception (error);
				}
				else
				{
#if DEBUG
					var feeds = string.Empty;

					for (int i = 0; i < feedItems.Count; i++)
					{
						var feed = feedItems [i];
						feeds = $"Title: {feed.title}; Message: {feed.message}; Content: {feed.content}. \r\n{feeds}";
						shFeeds.SendFeedAck (feed.feed_id);
						shFeeds.NotifyFeedResult (feed.feed_id, 1);
					}

					//Current.MainPage.DisplayAlert ($"New feeds available and fetched {arrayFeeds.Count}:", feeds, "OK");
#endif
					var items = new List<FeedItemViewModel>();
					foreach (var item in feedItems)
					{
						string imgUrl = null;
						string linkUrl = null;

						var content = JsonConvert.DeserializeObject<Dictionary<string, string>> (item.content);
						foreach (var key in content.Keys)
						{
							var val = content [key];

							if (key == "img")
								imgUrl = val;
							else if (key == "url" || key.StartsWith ("deeplink"))
								linkUrl = val;
						}

						items.Add (new FeedItemViewModel
						{
							PictureUrl = imgUrl,
							LinkUrl = linkUrl,
							Title = item.title.Trim(),
							Text = item.message.Trim(),
							TimeStamp = DateTime.Parse (item.created),
							Group = item.campaign
						});
					}

					tcs.TrySetResult (items);
				}
			}
			catch (Exception ex)
			{
				tcs.TrySetException (ex);
			}
		}

		public static async void CheckCuid (string syncToken)
		{
			// if no sync token, nobody is logged in so don't bother getting ID
			if (string.IsNullOrEmpty (syncToken))
			{
				sCheckedCuid = false; // in case we switch users
				return;
			}

			// if we've already checked CUID or we're already checking it, don't bother
			if (sCheckedCuid || sCheckingCuid)
				return;

			try
			{
				sCheckingCuid = true;

				var http = new HttpClient();
				var devID = App.Current.DeviceID;
				var url = string.Format (App.SHCuidUrl, devID, syncToken);

				var result = await http.GetStringAsync (url);
				var shcuid = JsonConvert.DeserializeObject<Dictionary<string, string>> (result);
				if (!shcuid.TryGetValue (cCode, out var code) &&
						shcuid.TryGetValue (cSHCuid, out var newID))
				{
					var props = App.Current.Properties;
					string userID = null;

					if (props.TryGetValue (cSHCuid, out var obj))
						userID = obj as string;

#if false//DEBUG
					Device.BeginInvokeOnMainThread (() =>
					{
						var message = string.Format (
								"TownFish: userID = {0}; newID = {1}",
								userID ?? "null", newID ?? "null");

						App.Current.MainPage.DisplayAlert (
								"StreetHawk Registration", message, "Continue");
					});
#endif
					if (userID != newID)
					{
						userID = newID;

						var shAnalytics = DependencyService.Get<IStreetHawkAnalytics>();
						shAnalytics.TagCuid (userID);

						url = string.Format (App.SHSyncUrl, devID, syncToken);

						for (var i = 0; i++ <= cSHSyncRetries; )
						{
							// give SH time to process it
							await Task.Delay (cSHSyncDelay);

							result = await http.GetStringAsync (url);
							var syncResult = JsonConvert.DeserializeObject<Dictionary<string, string>> (result);

							if (!shcuid.TryGetValue (cCode, out code) &&
									syncResult.TryGetValue (cSynced, out var sync) &&
									sync == "true")
							{
								props [cSHCuid] = userID;

								// got it, so stop trying
								break;
							}
						}
					}
				}
			}
			catch {} // if it fails, we don't care
			finally
			{
				sCheckedCuid = true;
				sCheckingCuid = false;
			}
		}

		#endregion StreetHawk

		#region App Property Helpers

		/// <summary>
		/// Gets a persistent App property.
		/// </summary>
		/// <typeparam name="T">The type of the property</typeparam>
		/// <param name="key">The key.</param>
		/// <param name="def">Optional default value to return if the property doesn't exist.</param>
		/// <returns></returns>
		public static T GetProp<T> (string key, T def = default (T)) =>
				Current.Properties.TryGetValue (key, out var o) ? o.ConvertTo (def) : def;

		/// <summary>
		/// Sets or clears a persistent App property.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="value">The value to set, or the default value for T to clear.</param>
		public static void SetProp<T> (string key, T value)
		{
			if (Equals (value, default (T)))
				Current.Properties.Remove (key);
			else
				Current.Properties [key] = value;
		}

		#endregion App Property Helpers

		#endregion Methods

		#region Properties and Events

		public event EventHandler AppSuspended;

		public event EventHandler AppResumed;

		public event EventHandler BackButtonPressed;

		public event EventHandler AppCloseRequested;

		public event EventHandler<string> PushUrlReceived;

		public event EventHandler<int> SHFeedItemsAvailable;

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

		public string DeviceID { get; }

		public static DateTime LastSHFeedReadTime
		{
			get
			{
				if (sLastSHFeedReadTime == DateTime.MinValue)
					sLastSHFeedReadTime = GetProp<DateTime> (cLastSHFeedReadTimeKey);

				return sLastSHFeedReadTime;
			}

			set
			{
				sLastSHFeedReadTime = value;

				SetProp (cLastSHFeedReadTimeKey, value);
			}
		}

		public static IList<FeedItemViewModel> SHFeedItems
		{
			get
			{
				LastSHFeedReadTime = DateTime.Now;

				return sSHFeedItems;
			}

			set
			{
				sSHFeedItems = value;

				// now see how many are new and notify subscribers
				var t = LastSHFeedReadTime;
				var c = value.Count (i => i.TimeStamp > t);

				Current.SHFeedItemsAvailable?.Invoke (Current, c);
			}
		}

		public static int SHFeedItemCount => sSHFeedItems?.Count ?? 0;

		#endregion Properties and Events

		#region Fields

		// all magic URLs and paths used in this app
		public const string SiteDomain = "dev.townfish.com";
		public const string BaseUrl = "http://" + SiteDomain;
		public const string StartPath = "/";
		public const string TermsUrl = BaseUrl + "/terms-of-use/";
		public const string SHCuidUrl = BaseUrl + "/profile/shcuid/{0}?syncToken={1}";
		public const string SHSyncUrl = BaseUrl + "/profile/shsync/{0}?syncToken={1}";
		public const string EditProfileUrl = BaseUrl + "/profile/edit/generalinfo";
		public const string EditLikesUrl = BaseUrl + "/profile/edit/likes";

		public const string QueryParam = "mode=app"; // parameter added to every request
		public const string QueryString = "?" + QueryParam;

		public const string TwitterApiDomain = "api.twitter.com";

		public const string GcmSenderID = "7712235891";

		public const string StreetHawkAppKey = "TownFish";

		const string cLastSHFeedReadTimeKey = "LastSHFeedReadTime";

		const string cCode = "Code";
		const string cSHCuid = "shcuid";
		const string cSynced = "synced";
		const int cSHSyncDelay = 3000;
		const int cSHSyncRetries = 3;

		static bool sCheckingCuid;
		static bool sCheckedCuid;

		static Assembly sAssembly = null;

		// NOTE: use LastSHFeedReadTime property to save persistently
		static DateTime sLastSHFeedReadTime = DateTime.MinValue;

		static int sGettingSHFeed;
		static IList<FeedItemViewModel> sSHFeedItems;

		#endregion Fields
	}
}
