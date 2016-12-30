using System;

using Android.App;
using Android.Util;

using Com.Streethawk.Library.Geofence;
using StreetHawkCrossplatform;


[assembly: Xamarin.Forms.Dependency(typeof(StreetHawkGeofence))]


namespace StreetHawkCrossplatform
{
	public class StreetHawkGeofence : Java.Lang.Object, IStreetHawkGeofence, INotifyGeofenceTransition
	{
		static Application mApplication => StreetHawkAnalytics.Application;

		public bool GetIsDefaultLocationServiceEnabled()
		{
			Log.Error("StreetHawk", "Function GetIsDefaultLocationServiceEnabled is not implemented for Android");
			return false;
		}

		public bool GetIsLocationServiceEnabled()
		{
			Log.Error("StreetHawk", "Function GetIsDefaultLocationServiceEnabled is not implemented for Android");
			return false;
		}

		RegisterForGeofenceCallback mGeofenceCallBack;
		public void RegisterForGeofenceStatus(RegisterForGeofenceCallback cb)
		{
			mGeofenceCallBack = cb;
		}

		public void SetIsDefaultLocationServiceEnabled(bool isEnable)
		{
			Log.Error("StreetHawk", "Function GetIsDefaultLocationServiceEnabled is not implemented for Android");
		}

		public void SetIsLocationServiceEnabled(bool isEnable)
		{
			Log.Error("StreetHawk", "Function GetIsDefaultLocationServiceEnabled is not implemented for Android");
		}

		public void StartGeofenceMonitoring()
		{
			SHGeofence.GetInstance(mApplication.ApplicationContext).StartGeofenceMonitoring();
		}

		public void StartGeofenceWithPermissionDialog(string message)
		{
			Log.Error("StreetHawk", "Function StartGeofenceWithPermissionDialog is not implemented for Android");
		}

		public void StopGeofenceMonitoring()
		{
			SHGeofence.GetInstance(mApplication.ApplicationContext).StopMonitoring();
		}

		public void OnDeviceEnteringGeofence()
		{
			//TODO Add function in native to support this callback

			Log.Error("StreetHawk","OnDeviceEnteringGeofence is not supported in this release");
		}

		public void OnDeviceLeavingGeofence()
		{
			//TODO Add function in native to support this callback

			Log.Error("StreetHawk", "OnDeviceEnteringGeofence is not supported in this release");
		}
	}
}
