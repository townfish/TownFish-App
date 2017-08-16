using System;
using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;
using Com.Streethawk.Library.Push;
using Android.Support.V4.App;
using Android.Graphics;
using StreetHawkCrossplatform;

namespace TownFish.App.Droid
{
    [Service]
    class JsonService : Service, ISHObserver
    {
        static readonly string TAG = "streethawk Service";
        bool isStarted;

        public static RegisterForShReceivedRawJSONCallback Callback { get; internal set; }

        public void OnReceiveNonSHPushPayload(Bundle p0)
        {

        }

        public void OnReceivePushData(Com.Streethawk.Library.Push.PushDataForApplication p0)
        {

        }

        public void OnReceiveResult(Com.Streethawk.Library.Push.PushDataForApplication p0, int p1)
        {

        }

        public void ShNotifyAppPage(string p0)
        {

        }

        public void ShReceivedRawJSON(string title, string body, string json)
        {
            if (MainActivity.IsActive)
            {
                if (null != Callback)
                {
                    Callback.Invoke(title, body, json);
                }
            }
            else
            {
                SendNotification(title, body, json);
            }
        }


        void SendNotification(string title, string body, string json)
        {
            var intent = new Intent(this.ApplicationContext, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            intent.PutExtra("json", json);
            var pendingIntent = PendingIntent.GetActivity(ApplicationContext, 0, intent, PendingIntentFlags.UpdateCurrent);

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
            }

            // This tells Android to restart the service if it is killed to reclaim resources.
            return StartCommandResult.Sticky;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Info(TAG, "OnCreate: the service is initializing.");

            Push.GetInstance(this).RegisterSHObserver(this);
        }


        public override IBinder OnBind(Intent intent)
        {
            // Return null because this is a pure started service. A hybrid service would return a binder that would
            // allow access to the GetFormattedStamp() method.
            return null;
        }


    }
}