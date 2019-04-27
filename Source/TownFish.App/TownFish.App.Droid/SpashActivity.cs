using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;


namespace TownFish.App.Droid
{
	[Activity(Label = "TownFish", Theme = "@style/townfishSplash", MainLauncher = true, NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
	public class SpashActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);		
		}

		protected override void OnResume()
		{
			base.OnResume();
			StartActivity(new Intent(Application.Context, typeof(MainActivity)));
		}
	}
}
