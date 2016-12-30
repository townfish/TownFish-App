using System;

using Android.App;
using Android.Util;

using Com.Streethawk.Library.Beacon;
using StreetHawkCrossplatform;


[assembly: Xamarin.Forms.Dependency(typeof(StreetHawkBeacons))]


namespace StreetHawkCrossplatform
{
	public class StreetHawkBeacons : Java.Lang.Object, IStreetHawkBeacon, INotifyBeaconTransition
	{
		static Application mApplication => StreetHawkAnalytics.Application;

		public bool GetIsDefaultLocationServiceEnabled()
		{
			Log.Error("StreetHawk", "Function DisplayBadge is not implemented for Android");
			return false;
		}

		public bool GetIsLocationServiceEnabled()
		{
			Log.Error("StreetHawk", "Function DisplayBadge is not implemented for Android");
			return false;
		}

		public void NotifyBeaconDetected()
		{
			if (mBeaconCallBack != null)
			{
				Log.Error("StreetHawk", "Function NotifyBeaconDetected is not available in this release");
				mBeaconCallBack.Invoke(null);
			}
		}

		RegisterForBeaconCallback mBeaconCallBack;
		public void RegisterForBeaconStatus(RegisterForBeaconCallback cb)
		{
			mBeaconCallBack = cb;
		}

		public void SetIsDefaultLocationServiceEnabled(bool isEnable)
		{
			Log.Error("StreetHawk", "Function DisplayBadge is not implemented for Android");
		}

		public void SetIsLocationServiceEnabled(bool isEnable)
		{
			Log.Error("StreetHawk", "Function DisplayBadge is not implemented for Android");
		}

		public int ShEnterBeacon(string uuid, int major, int minor, double distance)
		{
			return Beacons.GetInstance(mApplication.ApplicationContext).ShEnterBeacon(uuid, major, minor, distance);
		}

		public int ShExitBeacon(string uuid, int major, int minor)
		{
			return Beacons.GetInstance(mApplication.ApplicationContext).ShExitBeacon(uuid, major, minor);
		}

		public void StartBeaconMonitoring()
		{
			Beacons.GetInstance(mApplication.ApplicationContext).StartBeaconMonitoring();
		}

		public void StopBeaconMonitoring()
		{
			Beacons.GetInstance(mApplication.ApplicationContext).StopBeaconMonitoring();
		}
	}
}
