using System;
using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;
using Com.Streethawk.Library.Push;
using Android.Support.V4.App;
using Android.Graphics;
using StreetHawkCrossplatform;
using Android.Widget;
using Java.Util;

namespace TownFish.App.Droid
{
    [Service(Name = "com.townfish.app.JsonService")]
    class JsonService : Service, ISHObserver
    {
        static readonly string TAG = "JSONService";
        bool isStarted;

        public static RegisterForShReceivedRawJSONCallback Callback { get; internal set; }

        public void OnReceiveNonSHPushPayload(Bundle p0)
        {
            Log.Info(TAG, "OnReceiveNonSHPushPayLoad");
        }

        public void OnReceivePushData(Com.Streethawk.Library.Push.PushDataForApplication p0)
        {
            Log.Info(TAG, "OnReceivePushData");
        }

        public void OnReceiveResult(Com.Streethawk.Library.Push.PushDataForApplication p0, int p1)
        {
            Log.Info(TAG, "OnReceiveResult");
        }

        public void ShNotifyAppPage(string p0)
        {
            Log.Info(TAG, "ShNotifyAppPage");
        }

        public void ShReceivedRawJSON(string title, string body, string json)
        {
            Log.Info(TAG, "ShReceivedRawJSON");
            try
            {
                if (MainActivity.IsActive && Callback != null)
                {
                    Log.Info(TAG, "MainActivity.IsActive is true");
                    Callback?.Invoke(title, body, json);
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Main Activity not available " + ex);
            }
            SendNotification(title, body, json);
        }


        void SendNotification(string title, string body, string json)
        {
            Log.Info(TAG, "Notification Received");
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


        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (isStarted)
            {
                Log.Info(TAG, "OnStartCommand: This service has already been started.");
            }
            else
            {
                Log.Info(TAG, "OnStartCommand: The service is starting.");
                isStarted = true;
                StartTimer();
            }

            // This tells Android to restart the service if it is killed to reclaim resources.
            return StartCommandResult.Sticky;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Info(TAG, "OnCreate: the service is initializing.");

            try
            {
                var pushInstance = Push.GetInstance(this);
                if (pushInstance != null)
                {
                    pushInstance.RegisterSHObserver(this);
                    Log.Info(TAG, "StreetHawk Observer registered.");
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

        private Timer mTimer;
        private TimerTask mTimerTask;

        private void StopTimer()
        {
            //stop the timer, if it's not already null
            if (mTimer != null)
            {
                mTimer.Cancel();
                mTimer = null;
            }
        }

        private class MyTimerTask : TimerTask
        {
            int counter = 0;
            public override void Run()
            {
                Log.Info(TAG, "in timer ++++  " + (counter++));
            }
        }

        private void StartTimer()
        {
            //set a new Timer
            mTimer = new Timer();

            mTimerTask = new MyTimerTask();

            //schedule the timer, to wake up every 1 second
            mTimer.Schedule(mTimerTask, 1000, 1000);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            StopTimer();
            Log.Info("JsonService", "OnDestroy");

            Log.Info("JsonService", "Sending broadcast intent");
            Intent broadcastIntent = new Intent("com.townfish.app.RestartSensor");
            SendBroadcast(broadcastIntent);
        }


        public override IBinder OnBind(Intent intent)
        {
            // Return null because this is a pure started service. A hybrid service would return a binder that would
            // allow access to the GetFormattedStamp() method.
            return null;
        }


    }
}