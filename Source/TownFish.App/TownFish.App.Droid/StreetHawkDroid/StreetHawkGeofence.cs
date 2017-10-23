using System;
using System.Collections.Generic;
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

        /* GeoFence Data consists of
         * GeoFence ID = id of geoFence of Geofence
         * Latitude = get latitude of GeoFence  
         * Longitude = get Longitude of GeoFence  
         * Radius = getting Radius of Geofence
         * ParentId = getting Parent id of Geofence
         * Distance = getting Distance of Geofence
         */
        public void OnDeviceEnteringGeofence(IList<GeofenceData> geoFence)
        {
            // When Device enters GeoFence
            foreach (var item in geoFence)
            {
                String geoId = item.GeofenceID;
                double geoLatitude = item.Latitude;
                double geoLongitude = item.Longitude;
                float geoRadius = item.Radius;
                double geoDistance = item.Distance;
            }
            return;
        }

        public void OnDeviceLeavingGeofence(IList<GeofenceData> geoFence)
        {
            // When Device leaves GeoFence 
            foreach(var item in geoFence)
            {
                String geoId = item.GeofenceID;
                double geoLatitdue = item.Latitude;
                double geoLongitude = item.Longitude;
                float geoRadius = item.Radius;
                double geoDistance = item.Distance;
            }
            return;
        }
    }
}
