using System;

using Android.Content;
using Android.Widget;
using Android.Util;

namespace TownFish.App.Droid
{
    [BroadcastReceiver(Name = "com.townfish.app.JsonServiceRestarter")]
    public class JsonServiceRestarter : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "Restarting TownFish Push Notification Service",
					ToastLength.Short).Show();

            Log.Debug("JsonServiceRestarter", "Restarting JsonService");
            context.StartService(new Intent(context, typeof(JsonService)));
        }
    }
}
