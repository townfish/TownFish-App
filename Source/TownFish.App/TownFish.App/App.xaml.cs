using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;
using Newtonsoft.Json;
using Realms;
using TownFish.App.Helpers;
using TownFish.App.Models;
using TownFish.App.Pages;
using TownFish.App.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Device = Xamarin.Forms.Device;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TownFish.App
{
    public partial class App: Application
    {
        #region Fields

        static Assembly sAssembly = null;

        // NOTE: use LastDiscoveriesViewTime property to save persistently
        static DateTime sLastDiscoveriesViewTime = DateTime.MinValue;

        // start this off set so first in gets it
        static AutoResetEvent sDiscoveriesEvent = new AutoResetEvent(true);

        static IList<DiscoveryItemViewModel> sDiscoveries;

        DateTime mStartTime;
        DateTime? mSleepTime;
        DateTime? mResumedTime;

        #endregion Fields

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
                    sAssembly = typeof(App).GetTypeInfo().Assembly;

                return sAssembly;
            }
        }

        public static string DataFromNotification = null;

        public static double WebImageDefaultHeight { get; set; }

        public string DeviceID { get; }

        public static Realm Database { get; set; }
        public static HttpClient Client { get; set; }
        public static string OnesignalPlayerId { get; set; }
        public static string SyncToken { get; set; }
        public static int OnesignalBadgeCount { get; set; }

        public static DateTime LastDiscoveriesViewTime
        {
            get
            {
                if (sLastDiscoveriesViewTime == DateTime.MinValue)
                    sLastDiscoveriesViewTime = GetProp<DateTime>(Constants.cLastDiscoveriesViewTimeKey);

                return sLastDiscoveriesViewTime;
            }

            set
            {
                sLastDiscoveriesViewTime = value;

                SetProp(Constants.cLastDiscoveriesViewTimeKey, value);
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
        public static int NewDiscoveriesCount => Discoveries?.Count(d =>
               d.Created > LastDiscoveriesViewTime) ?? 0;

        #endregion Properties and Events

        #region Construction

        public App (string deviceID, string dataFromNotification = null)
        {
            if (dataFromNotification != null)
                DataFromNotification = dataFromNotification;

            InitializeComponent();

            DeviceID = deviceID;

            WebImageDefaultHeight = (Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width / Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density) * 0.66;

            Client = new HttpClient();
            Database = Realm.GetInstance();
        }

        #endregion Construction

        #region Methods

        protected override void OnStart()
        {
            mStartTime = DateTime.UtcNow;
            mSleepTime = null;
            mResumedTime = null;

            InitializeAppCenter();
            InitOneSignal();

            if(Device.RuntimePlatform == Device.iOS)
                MainPage = new ContentPage();
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

        void OnDiscoveriesUpdated() => DiscoveriesUpdated?.Invoke(this, EventArgs.Empty);

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

            //await HandlePushNotification (json, wasBackgrounded: true);
        }

        public void OnBackButtonPressed()
        {
            BackButtonPressed?.Invoke (this, EventArgs.Empty);
        }

        public void CloseApp()
        {
            AppCloseRequested?.Invoke (this, EventArgs.Empty);
        }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            // App opened through Universal App Link
            base.OnAppLinkRequestReceived(uri);

            if (!(MainPage is BrowserPage))
                return;

            (MainPage as BrowserPage)?.LoadApplink(uri.OriginalString);
        }

        //called when notif is clicked and app ia already launched
        public static async Task OpenNotificationVM(string dataFromNotification)
        {
            if (string.IsNullOrEmpty(dataFromNotification))
                return;

            await SaveAndSync("", "");
        }

        #region Onesignal

        void InitializeAppCenter()
        {
            AppCenter.Start("android=de2720f8-f06b-4cdc-b177-d5c716848991;" +
                  "uwp={Your UWP App secret here};" +
                  "ios=e68a22ee-57cc-4633-ac88-e45cb490399e;",
                  typeof(Analytics), typeof(Crashes), typeof(Push));
        }

        async void InitOneSignal()
        {
            OneSignal.Current
                .StartInit(Constants.OneSignalKey)
                .InFocusDisplaying(OSInFocusDisplayOption.Notification)
                //.HandleNotificationReceived(HandleNotificationReceived)
                .HandleNotificationOpened(HandleNotificationOpened)
                .EndInit();

            OneSignal.Current.SetLogLevel(LOG_LEVEL.VERBOSE, LOG_LEVEL.NONE);
            OneSignal.Current.IdsAvailable(IdsAvailable);

            //Optional: Callback when receive json push.
            //Called when application is active (Android/iOS) and
            //when application is launched/activated by clicking on
            //a push notification ON iOS ONLY
            //            shPush.RegisterForRawJSON (async (title, message, json) =>
            //                {
            //#if DEBUG
            //                    Device.BeginInvokeOnMainThread (() =>
            //                        {
            //                            var msg = $"title: {title}\r\nmessage: {message}\r\nJSON: {json}";
            //                            
            //Device.DisplayAlert ("Received JSON push:", msg, "OK");
            //                        });
            //#endif
            //    try
            //    {
            //        var now = DateTime.UtcNow;
            //        var wasBackgrounded = (now - mStartTime).TotalSeconds < 5 ||
            //                (mSleepTime != null && mResumedTime == null) ||
            //                (now - mResumedTime.GetValueOrDefault()).TotalSeconds < 5;

            //        await HandlePushNotification (json, wasBackgrounded);
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.WriteLine (
            //                $"App.InitStreetHawk/RegisterForRawJSON: {ex.Message}\r\n{ex}");
            //    }
            //});

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
                IList<DiscoveryItemViewModel> GetDiscoveryItems(List<FeedsModel> feedItems)
                {
                    var items = new List<DiscoveryItemViewModel>();
                    foreach (var item in feedItems)
                    {
                        string imgUrl = null;
                        string linkUrl = null;
                        double? scale = null;

                        var content = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.content);
                        foreach (var key in content.Keys)
                        {
                            var val = content[key];

                            if (key == "img" || key.StartsWith("image"))
                                imgUrl = val;
                            else if (key == "url" || key.EndsWith("link"))
                                linkUrl = val;

                            if (key == "scale")
                                scale = double.Parse(val);
                        }

                        var created = string.IsNullOrEmpty(item.created) ? null : (DateTime?)DateTime.Parse(item.created, CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal);
                        var modified = string.IsNullOrEmpty(item.modified) ? null : (DateTime?)DateTime.Parse(item.modified, CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal);
                        var expires = string.IsNullOrEmpty(item.expires) ? null : (DateTime?)DateTime.Parse(item.expires, CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal);

                        items.Add(new DiscoveryItemViewModel
                        {
                            PictureUrl = imgUrl,
                            LinkUrl = linkUrl,
                            Title = item.title?.Trim(),
                            Text = item.message?.Trim(),
                            Created = created,
                            Modified = modified,
                            Expires = expires,
                            Group = item.campaign,
                            PictureScale = scale ?? 1.0
                        });
                    }

                    // return in descending creation order (i.e. reverse date)
                    items.Sort((a, b) => b.Created.GetValueOrDefault().CompareTo(a.Created.GetValueOrDefault()));

                    return items;
                }

                #endregion Local functions

                Enter();

                Debug.WriteLine($"App.GetDiscoveries: Started");

                // if it takes too long to return (i.e. to call my callback
                // below), give up to prevent leaks

                var cts = new CancellationTokenSource(30000);
                cts.Token.Register(() =>
                {
                    tcs.TrySetException(new TimeoutException());

                    Debug.WriteLine($"App.GetDiscoveries: Timeout waiting for callback");

                    Leave();
                });

                //HandleGetFeeds(tcs, cts);
            });

            return tcs.Task;
        }

        private static void HandleGetFeeds(TaskCompletionSource<IList<DiscoveryItemViewModel>> tcs, CancellationTokenSource cts)
        {
//            var shFeeds = DependencyService.Get<IStreetHawkFeeds>();
//            shFeeds.ReadFeedData(0, (feedItems, error) =>
//            {
//                // if task completed (failed or timed out), just leave
//                if (tcs.Task.IsCompleted)
//                    return;

//                try
//                {
//                    // now we're back from callback we can cancel the timeout
//                    cts.Dispose();

//                    if (error != null)
//                    {
//#if DEBUG
//                        Device.BeginInvokeOnMainThread(() =>
//                             Current?.MainPage?.DisplayAlert(
//                                    "App.GetDiscoveries: New feeds " +
//                                    "available but fetch failed:",
//                                    error, "OK"));
//#endif
//                        throw new Exception(error);
//                    }
//#if false//DEBUG
//                                var feedMsg = "";

//                                foreach (var feedItem in feedItems)
//                                    feedMsg = $"Title: {feedItem.title}; Message: {feedItem.message}; Content: {feedItem.content}. \r\n{feedMsg}";

//                                Device.BeginInvokeOnMainThread(() =>
//                                    Current?.MainPage?.DisplayAlert (
//                                            "New feeds available and fetched " +
//                                            $"{feedItems.Count}:", feedMsg, "OK"));
//#endif
            //        // acknowledge receipt to SH and indicate positive result
            //        foreach (var feedItem in feedItems)
            //        {
            //            shFeeds.SendFeedAck(feedItem.feed_id);
            //            shFeeds.NotifyFeedResult(feedItem.feed_id, ACCEPTED, false, true);
            //        }

            //        tcs.TrySetResult(GetDiscoveryItems(feedItems));
            //    }
            //    catch (Exception ex)
            //    {
            //        tcs.TrySetException(ex);

            //        Debug.WriteLine($"App.GetDiscoveries: " +
            //            $"Error {ex.Message}\r\n{ex}");
            //    }
            //    finally
            //    {
            //        Debug.WriteLine($"App.GetDiscoveries: Finished");

            //        Leave();
            //    }
            //});
        }

        private void IdsAvailable(string userID, string pushToken)
        {
            OnesignalPlayerId = userID;

            if ((Device.RuntimePlatform == Device.iOS) && MainPage.GetType() == typeof(BrowserPage))
                return;

            MainPage = new BrowserPage
            {
                BindingContext = new BrowserPageViewModel
                {
                    SourceUrl = Constants.BaseUrl + Constants.StartPath
                }
            };
        }

        //private static void HandleNotificationReceived(OSNotification notification)
        //{
            //OnesignalBadgeCount++;

            ////Sync to server
            //Device.BeginInvokeOnMainThread(async () =>
            //{
            //    OSNotificationPayload payload = notification.payload;
            //    string id = payload.notificationID;
            //    string message = payload.body;
            //    await SaveAndSync(id, message);
            //});
        //}

        private static async Task SaveAndSync(string id, string message)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(message))
                return;

            Database?.Write(() =>
            {
                Database.Add
                (
                    new RecievedPayload()
                    {
                        Id = id,
                        Content = message
                    }
                );
            });

            await SynNotificationsToServer(id);
        }

        private void HandleNotificationOpened(OSNotificationOpenedResult result)
        {
            OnesignalBadgeCount = 0;

            //Sync to server
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    (Application.Current.MainPage as  BrowserPage).ViewModel.IsLoading = true;
                    var payload = result.notification.payload;
                    var additionalData = payload.additionalData;
                    string id = payload.notificationID;
                    string message = payload.body;
                    await SaveAndSync(id, message);
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    (Application.Current.MainPage as BrowserPage).ViewModel.IsLoading = false;
                }
            });
        }

        public void UpdateAppBadgeCount (int count)
        {
            //var shAnalytics = DependencyService.Get<IStreetHawkAnalytics>();
            //shAnalytics?.TagNumeric ("sh_badge_number", count);
        }

        static async Task SynNotificationsToServer(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            if (string.IsNullOrEmpty(SyncToken))
                return;

            //Todo: Verify App has synctoken
            try
            {
                var msg = Database.Find<RecievedPayload>(id);

                if (msg == null)
                    return;

                var url = string.Format(Constants.NotificationsSyncUrl, SyncToken, OnesignalPlayerId);
                var result = await Client.PostAsync(url, new StringContent(msg.Content));
                if (result.IsSuccessStatusCode)
                {
                    var shcuid = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.Content.ToString());
                    await Task.Delay(5000);
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion Onesignal

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
    }
}
