//#define SH_OPTIONAL

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Newtonsoft.Json;

using StreetHawkCrossplatform;

using TownFish.App.Helpers;
using TownFish.App.Pages;
using TownFish.App.ViewModels;


[assembly: XamlCompilation (XamlCompilationOptions.Compile)]


namespace TownFish.App
{
	public partial class App: Application
	{
		#region Construction

		public App (string deviceID)
		{
			InitializeComponent();

			DeviceID = deviceID;
		}

		#endregion Construction

		#region Methods

		protected override void OnStart()
		{
			mStartTime = DateTime.UtcNow;
			mSleepTime = null;
			mResumedTime = null;

			InitStreetHawk();

			MainPage = new BrowserPage
			{
				BindingContext = new BrowserPageViewModel
				{
					SourceUrl = BaseUrl + StartPath + QueryString
				}
			};
		}

		protected override void OnSleep()
		{
			mSleepTime = DateTime.UtcNow;
			mResumedTime = null;
			AppSuspended?.Invoke (this, EventArgs.Empty);
		}

		protected override void OnResume()
		{
			mResumedTime = DateTime.UtcNow;
			AppResumed?.Invoke (this, EventArgs.Empty);
		}

		async void OnPushUrlReceived (string route, bool wasBackgrounded)
		{
			Debug.WriteLine ($"App.OnPushUrlReceived: " +
				$"Waiting for PushUrlReceived to be hooked");

			// wait for main page to hook event so we can then raise it
			while (PushUrlReceived == null)
				await Task.Delay (100);

			Debug.WriteLine ($"App.OnPushUrlReceived: " +
				$"GOT PushUrlReceived; invoking");

			PushUrlReceived.Invoke (this, (route, wasBackgrounded));
		}

		void OnDiscoveriesUpdated()
		{
			DiscoveriesUpdated?.Invoke (this, EventArgs.Empty);
		}

		async void OnBackgroundDiscoveriesReceived()
		{
			Debug.WriteLine ($"App.OnBackgroundDiscoveriesReceived: " +
				$"Waiting for BackgroundDiscoveriesReceived to be hooked");

			// wait for main page to hook event so we can then raise it
			while (BackgroundDiscoveriesReceived == null)
				await Task.Delay (100);

			Debug.WriteLine ($"App.OnBackgroundDiscoveriesReceived: " +
				$"GOT BackgroundDiscoveriesReceived; invoking");

			BackgroundDiscoveriesReceived.Invoke (this, EventArgs.Empty);
		}

		public async void LaunchedFromNotification (string json)
		{
			Debug.WriteLine ($"App.LaunchedFromNotification: " +
				$"Calling HandlePushNotification");

			await HandlePushNotification (json, wasBackgrounded: true);
		}

		public void OnBackButtonPressed()
		{
			BackButtonPressed?.Invoke (this, EventArgs.Empty);
		}

		public void CloseApp()
		{
			AppCloseRequested?.Invoke (this, EventArgs.Empty);
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

			//Optional: enable XCode console logs.
			shAnalytics.SetEnableLogs (true);

			shAnalytics.SetAppKey (StreetHawkAppKey);

			// Initialize StreetHawk when App starts.
			//Mandatory: set app key and call init.
			shAnalytics.Init();

			shPush.RegisterForPushMessaging (GcmSenderID); // GCM push ID (Android only)
			shPush.Register();

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

#if SH_OPTIONAL

			//----------------------------------------------------------------
			// Apparently, hooking these stops them from working. Apparently.
			//

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

			//
			//----------------------------------------------------------------

#endif // SH_OPTIONAL

			//Optional: Callback when receive json push.
			//Called when application is active (Android/iOS) and
			//when application is launched/activated by clicking on
			//a push notification ON iOS ONLY
			shPush.RegisterForRawJSON (async (title, message, json) =>
				{
#if DEBUG
					Device.BeginInvokeOnMainThread (() =>
						{
							var msg = $"title: {title}\r\nmessage: {message}\r\nJSON: {json}";
							MainPage.DisplayAlert ("Received JSON push:", msg, "OK");
						});
#endif
					try
					{
						var now = DateTime.UtcNow;
						var wasBackgrounded = (now - mStartTime).TotalSeconds < 5 ||
								(mSleepTime != null && mResumedTime == null) ||
								(now - mResumedTime.GetValueOrDefault()).TotalSeconds < 5;

						await HandlePushNotification (json, wasBackgrounded);
					}
					catch (Exception ex)
					{
						Debug.WriteLine (
								$"App.InitStreetHawk/RegisterForRawJSON: {ex.Message}\r\n{ex}");
					}
				});

#if SH_OPTIONAL

			//----------------------------------------------------------------
			// SH supposedly forwards non-SH push payloads via this
			//

			//Optional: Callback when none 
			shPush.OnReceiveNonSHPushPayload (payload =>
			{
#if DEBUG
				Device.BeginInvokeOnMainThread (() =>
					MainPage.DisplayAlert ("Receive non-StreetHawk push:", payload, "OK"));
#endif
			});

			//
			//----------------------------------------------------------------

#endif // SH_OPTIONAL

			// on app load, we need to get feed to show count and cache items
			try
			{
				Debug.WriteLine ($"App.InitStreetHawk: Getting Discoveries");

				Discoveries = await GetDiscoveries();

				Debug.WriteLine ($"App.InitStreetHawk: GOT Discoveries");
			}
			catch (Exception ex)
			{
				Debug.WriteLine ($"App.InitStreetHawk: Error {ex.Message}\r\n{ex}");
			}
		}

		async Task HandlePushNotification (string json, bool wasBackgrounded)
		{
			Debug.WriteLine ("App.HandlePushNotification: " +
					$"wasBackgrounded = {wasBackgrounded}; json = \r\n{json}");

			try
			{
				var content = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
				if (content.TryGetValue ("dataType", out var dataType) &&
						content.TryGetValue ("route", out var route) &&
						dataType == "vanilla_notification" &&
						route.ToLower().Contains ("townfish.com"))
				{
					Debug.WriteLine ("App.HandlePushNotification: " +
						$"Vanilla notification received; navigating to:\r\n{route}");

					OnPushUrlReceived (route, wasBackgrounded);
				}
				else
				{
					Debug.WriteLine ($"App.HandlePushNotification: Getting Discoveries");

					Discoveries = await GetDiscoveries();

					Debug.WriteLine ($"App.HandlePushNotification: GOT Discoveries");

					if ((dataType == "messageFeed" ||
							content.TryGetValue ("img", out var img)) &&
							wasBackgrounded)
						OnBackgroundDiscoveriesReceived();
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine ($"App.HandlePushNotification: " +
					$"Error {ex.Message}\r\n{ex}");
			}
		}

		/// <summary>
		/// Gets the list of <see cref="DiscoverItemViewModel"/> items.
		/// </summary>
		/// <remarks>
		/// We have to go through this entire manual async and monitor palaver
		/// because StreetHawk sometimes doesn't call back the callback, so we
		/// have to set a timeout and handle it manually to avoid leaking
		/// task instances.
		/// </remarks>
		/// <returns></returns>
		public static Task<IList<DiscoveryItemViewModel>> GetDiscoveries()
		{
			Debug.WriteLine ($"App.GetDiscoveries: Starting");

			var tcs = new TaskCompletionSource<IList<DiscoveryItemViewModel>>();

			// move to threadpool to make sure monitor doesn't block the UI thread
			Task.Run (() =>
				{
					#region Local functions

					// waits for event before proceeding
					void Enter() => sDiscoveriesEvent.WaitOne();

					// sets event to allow next waiter to proceed
					void Leave() => sDiscoveriesEvent.Set();

					// converts list of SHFeed items to list of Discovery items
					IList<DiscoveryItemViewModel> GetDiscoveryItems (List<SHFeedObject> feedItems)
					{
						var items = new List<DiscoveryItemViewModel>();
						foreach (var item in feedItems)
						{
							string imgUrl = null;
							string linkUrl = null;

							var content = JsonConvert.DeserializeObject<Dictionary<string, string>> (item.content);
							foreach (var key in content.Keys)
							{
								var val = content [key];

								if (key == "img" || key.StartsWith ("image"))
									imgUrl = val;
								else if (key == "url" || key.EndsWith ("link"))
									linkUrl = val;
							}

							var created = string.IsNullOrEmpty(item.created) ? null : (DateTime?)DateTime.Parse(item.created, CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal);
							var modified = string.IsNullOrEmpty(item.modified) ? null : (DateTime?)DateTime.Parse(item.modified, CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal);
							var expires = string.IsNullOrEmpty(item.expires) ? null : (DateTime?)DateTime.Parse(item.expires, CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal);


							items.Add (new DiscoveryItemViewModel
							{
								PictureUrl = imgUrl,
								LinkUrl = linkUrl,
								Title = item.title?.Trim(),
								Text = item.message?.Trim(),
								Created = created,
								Modified = modified,
								Expires = expires,
								Group = item.campaign
							});
						}

						// return in descending creation order (i.e. reverse date)
						items.Sort ((a, b) => b.Created.GetValueOrDefault().CompareTo (a.Created.GetValueOrDefault()));

						return items;
					}

					#endregion Local functions

					Enter();

					Debug.WriteLine ($"App.GetDiscoveries: Started");

					// if it takes too long to return (i.e. to call my callback
					// below), give up to prevent leaks

					var cts = new CancellationTokenSource (30000);
					cts.Token.Register (() =>
						{
							tcs.TrySetException (new TimeoutException());

							Debug.WriteLine ($"App.GetDiscoveries: Timeout waiting for callback");

							Leave();
						});

					var shFeeds = DependencyService.Get<IStreetHawkFeeds>();
					shFeeds.ReadFeedData (0, (feedItems, error) =>
						{
							// if task completed (failed or timed out), just leave
							if (tcs.Task.IsCompleted)
								return;

							try
							{
								// now we're back from callback we can cancel the timeout
								cts.Dispose();

								if (error != null)
								{
#if DEBUG
									Device.BeginInvokeOnMainThread (() =>
										 Current?.MainPage?.DisplayAlert (
												"App.GetDiscoveries: New feeds " +
												"available but fetch failed:",
												error, "OK"));
#endif
									throw new Exception (error);
								}
#if false//DEBUG
								var feedMsg = "";

								foreach (var feedItem in feedItems)
									feedMsg = $"Title: {feedItem.title}; Message: {feedItem.message}; Content: {feedItem.content}. \r\n{feedMsg}";

								Device.BeginInvokeOnMainThread(() =>
									Current?.MainPage?.DisplayAlert (
											"New feeds available and fetched " +
											$"{feedItems.Count}:", feedMsg, "OK"));
#endif
								// acknowledge receipt to SH and indicate positive result
								foreach (var feedItem in feedItems)
								{
									shFeeds.SendFeedAck (feedItem.feed_id);
									shFeeds.NotifyFeedResult (feedItem.feed_id, ACCEPTED, false, true);
								}

								tcs.TrySetResult (GetDiscoveryItems (feedItems));
							}
							catch (Exception ex)
							{
								tcs.TrySetException (ex);

								Debug.WriteLine ($"App.GetDiscoveries: " +
									$"Error {ex.Message}\r\n{ex}");
							}
							finally
							{
								Debug.WriteLine ($"App.GetDiscoveries: Finished");

								Leave();
							}
						});
				});

			return tcs.Task;
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

				var shcuid = JsonConvert.DeserializeObject<Dictionary<string, object>> (result);

				if (!shcuid.TryGetValue (cCode, out var code) &&
						shcuid.TryGetValue (cSHCuid, out var newID))
				{
					var userID = GetProp<string> (cSHCuid);
					if (userID != newID as string)
					{
#if false//DEBUG
						Device.BeginInvokeOnMainThread (() =>
						{
							var message = string.Format (
									"TownFish: userID = {0}; newID = {1}",
									userID ?? "<null>", newID ?? "<null>");

							App.Current.MainPage.DisplayAlert (
									"StreetHawk Registration", message, "Continue");
						});
#endif
						userID = newID as string;

						var shAnalytics = DependencyService.Get<IStreetHawkAnalytics>();
						shAnalytics.TagCuid (userID);

						url = string.Format (SHSyncUrl, devID, syncToken);

						for (var i = 0; i++ <= cSHSyncRetries; )
						{
							// give SH time to process it
							await Task.Delay (cSHSyncDelay);

							try
							{
								result = await http.GetStringAsync (url);
								var syncResult = JsonConvert.DeserializeObject<Dictionary<string, object>> (result);

								if (!shcuid.TryGetValue (cCode, out code) &&
										syncResult.TryGetValue (cSynced, out var sync) &&
										sync as string == "true")
								{
									SetProp (cSHCuid, userID);

									// got it, so stop trying
									break;
								}
							}
							catch (Exception ex)
							{
								Debug.WriteLine ($"App.CheckCuid: " +
									$"Sync attempt {i} to {url} failed with {ex.Message}\r\n{ex}");
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine ($"App.CheckCuid: " +
					$"Failed with {ex.Message}\r\n{ex}");
			}
			finally
			{
				sCheckedCuid = true;
				sCheckingCuid = false;
			}
		}

		public void UpdateAppBadgeCount (int count)
		{
			var shAnalytics = DependencyService.Get<IStreetHawkAnalytics>();
			shAnalytics?.TagNumeric ("sh_badge_number", count);
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

		public event EventHandler<(string, bool)> PushUrlReceived;

		public event EventHandler DiscoveriesUpdated;

		public event EventHandler BackgroundDiscoveriesReceived;

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

		public static DateTime LastDiscoveriesViewTime
		{
			get
			{
				if (sLastDiscoveriesViewTime == DateTime.MinValue)
					sLastDiscoveriesViewTime = GetProp<DateTime> (cLastDiscoveriesViewTimeKey);

				return sLastDiscoveriesViewTime;
			}

			set
			{
				sLastDiscoveriesViewTime = value;

				SetProp (cLastDiscoveriesViewTimeKey, value);
			}
		}

		/// <summary>
		/// Gets or sets the list of discoveries, raising <see cref="DiscoveriesUpdated"/>
		/// when setting a new value.
		/// </summary>
		public static IList<DiscoveryItemViewModel> Discoveries
		{
			get => sDiscoveries;

			private set
			{
				sDiscoveries = value;

				Current.OnDiscoveriesUpdated();
			}
		}

		/// <summary>
		/// Gets the number of discovery items available, returning 0 if discoveries not loaded.
		/// </summary>
		public static int DiscoveriesCount => sDiscoveries?.Count ?? 0;

		/// <summary>
		/// Gets the number of new discovery items received since last being viewed.
		/// </summary>
		public static int NewDiscoveriesCount => Discoveries?.Count (d =>
				d.Created > LastDiscoveriesViewTime) ?? 0;

		#endregion Properties and Events

		#region Fields

		// all magic URLs and paths used in this app
		public const string SiteDomain = "dev.townfish.com";
		public const string BaseUrl = "https://" + SiteDomain;
		public const string StartPath = "/";
		public const string TermsUrl = BaseUrl + "/terms-of-use/";
		public const string SHCuidUrl = BaseUrl + "/profile/shcuid/{0}?syncToken={1}";
		public const string SHSyncUrl = BaseUrl + "/profile/shsync/{0}?syncToken={1}";
		public const string EditProfileUrl = BaseUrl + "/profile/edit/generalinfo";
		public const string EditLikesUrl = BaseUrl + "/profile/edit/likes";
		public const string ShowFeedUrl = BaseUrl + "/profile/showfeed";

		public const string QueryParam = "mode=app"; // parameter added to every request
		public const string QueryString = "?" + QueryParam;

		public const string TwitterApiDomain = "api.twitter.com";

		public const string GcmSenderID = "7712235891";

		public const string StreetHawkAppKey = "TownFish";

		const string cLastDiscoveriesViewTimeKey = "LastDiscoveriesViewTime";

		private const string ACCEPTED = "accepted";

		const string cCode = "Code";
		const string cSHCuid = "shcuid";
		const string cSynced = "synced";
		const int cSHSyncDelay = 3000;
		const int cSHSyncRetries = 3;

		static bool sCheckingCuid;
		static bool sCheckedCuid;

		static Assembly sAssembly = null;

		// NOTE: use LastDiscoveriesViewTime property to save persistently
		static DateTime sLastDiscoveriesViewTime = DateTime.MinValue;

		// start this off set so first in gets it
		static AutoResetEvent sDiscoveriesEvent = new AutoResetEvent (true);

		static IList<DiscoveryItemViewModel> sDiscoveries;

		DateTime mStartTime;
		DateTime? mSleepTime;
		DateTime? mResumedTime;

		#endregion Fields
	}
}
