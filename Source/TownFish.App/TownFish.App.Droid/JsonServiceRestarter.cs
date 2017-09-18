using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace TownFish.App.Droid
{
    [BroadcastReceiver(Name = "com.townfish.app.JsonServiceRestarter")]
    public class JsonServiceRestarter : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "JsonServiceRestarter.OnReceive!", ToastLength.Short).Show();
            Log.Info("JsonServiceRestarter", "Restarting JsonService");
            
            context.StartService(new Intent(context, typeof(JsonService)));
        }
    }
}