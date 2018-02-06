using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;

using Java.Util;

using Com.Streethawk.Library.Push;
using StreetHawkCrossplatform;


namespace TownFish.App.Droid
{
    [Service(Name = "com.townfish.app.JsonService")]
    class JsonService : Service, ISHObserver
    {
		#region Nested Types

		class MyTimerTask: TimerTask
		{
			public override void Run()
			{
				Log.Debug (TAG, "in timer ++++  " + (mCounter++));
			}

			int mCounter = 0;
		}

		#endregion Nested Types

		#region Service Overrides

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (mIsStarted)
            {
                Log.Debug(TAG, "OnStartCommand: This service has already been started.");
            }
            else
            {
                Log.Debug(TAG, "OnStartCommand: The service is starting.");
                mIsStarted = true;
                StartTimer();
            }

            // This tells Android to restart the service if it is killed to reclaim resources.
            return StartCommandResult.Sticky;
        }

        public override void OnCreate()
        {
            Log.Debug(TAG, "OnCreate: the service is initializing.");

            base.OnCreate();
            try
            {
                var pushInstance = Push.GetInstance(this);
                if (pushInstance != null)
                {
                    pushInstance.RegisterSHObserver(this);
                    Log.Debug(TAG, "StreetHawk Observer registered.");
                }
                else
                {
                    Log.Error(TAG, "StreetHawk Observer failed to register.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Error registering push callback with Streethawk\n" + ex);
            }
        }

        public override void OnDestroy()
        {
            Log.Debug("JsonService", "OnDestroy");

            base.OnDestroy();

            StopTimer();

            Log.Debug("JsonService", "Sending broadcast intent");
            Intent broadcastIntent = new Intent("com.townfish.app.RestartSensor");
            SendBroadcast(broadcastIntent);
        }

        public override IBinder OnBind(Intent intent)
        {
            // Return null because this is a pure started service. A hybrid service would return a binder that would
            // allow access to the GetFormattedStamp() method.
            return null;
        }

		#endregion Service Overrides

		#region Methods

		#region StreetHawk

		public void OnReceiveNonSHPushPayload(Bundle p0)
        {
            Log.Debug(TAG, "OnReceiveNonSHPushPayLoad");
        }

        public void OnReceivePushData(Com.Streethawk.Library.Push.PushDataForApplication p0)
        {
            Log.Debug(TAG, "OnReceivePushData");
        }

        public void OnReceiveResult(Com.Streethawk.Library.Push.PushDataForApplication p0, int p1)
        {
            Log.Debug(TAG, "OnReceiveResult");
        }

        public void ShNotifyAppPage(string p0)
        {
            Log.Debug(TAG, "ShNotifyAppPage");
        }

        public void ShReceivedRawJSON(string title, string body, string json)
        {
            Log.Debug(TAG, "ShReceivedRawJSON");

            try
            {
                if (MainActivity.IsActive && Callback != null)
                {
                    Log.Debug(TAG, "MainActivity.IsActive is true");
                    Callback.Invoke(title, body, json);
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Main Activity not available " + ex);
            }

            SendNotification(title, body, json);
        }

		#endregion StreetHawk

		void SendNotification(string title, string body, string json)
        {
            Log.Debug(TAG, "Notification Received");

            try
            {
                var intent = new Intent(this.ApplicationContext, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.ClearTop);
                intent.PutExtra("json", json);
                var pendingIntent = PendingIntent.GetActivity(ApplicationContext, 0, intent, PendingIntentFlags.OneShot);

                if (title.EndsWith(","))
                {
                    title = title.Substring(0, title.Length - 1);
                }

                var notificationBuilder = new NotificationCompat.Builder(ApplicationContext)
                    .SetSmallIcon(Resource.Drawable.icon)
                    .SetContentTitle(title)
                    .SetContentText(body)
                    .SetAutoCancel(true)
                    .SetExtras(intent.Extras)
                    .SetDefaults((int)(NotificationDefaults.Vibrate | NotificationDefaults.Sound))
                    .SetContentIntent(pendingIntent);

                var notificationManager = (NotificationManager)ApplicationContext.GetSystemService(NotificationService);
                notificationManager.Notify(0, notificationBuilder.Build());
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Error handling notification " + ex);
            }
        }

		void StartTimer()
        {
            //set a new Timer
            mTimer = new Timer();
            mTimerTask = new MyTimerTask();

            //schedule the timer, to wake up every 1 second
            mTimer.Schedule (mTimerTask, 1000, 1000);
        }

        void StopTimer()
        {
            mTimer?.Cancel();
            mTimer = null;
        }

		#endregion Methods

		#region Properties

		public static RegisterForShReceivedRawJSONCallback Callback { get; internal set; }

		#endregion Properties

		#region Fields

		const string TAG = "JSONService";

        bool mIsStarted;
        Timer mTimer;
        TimerTask mTimerTask;

		#endregion Fields
	}
}
