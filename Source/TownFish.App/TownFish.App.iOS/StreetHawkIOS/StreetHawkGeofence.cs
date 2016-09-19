using System;
using Foundation;
using Xamarin.Forms;

using StreetHawkCrossplatform;
using StreethawkIOS.Geofence;

[assembly: Dependency(typeof(StreetHawkGeofence))]

namespace StreetHawkCrossplatform
{
	public class StreetHawkGeofence : IStreetHawkGeofence
	{
		public void RegisterForGeofenceStatus(RegisterForGeofenceCallback cb)
		{
			SHGeofence.instance().notifyGeofenceEventCallback = delegate (NSDictionary dictGeofence)
			   {
				   SHGeofenceObj geofence = new SHGeofenceObj();
				   geofence.serverId = int.Parse(dictGeofence["serverId"].ToString());
				   geofence.latitude = double.Parse(dictGeofence["latitude"].ToString());
				   geofence.longitude = double.Parse(dictGeofence["longitude"].ToString());
				   geofence.radius = double.Parse(dictGeofence["radius"].ToString());
				   geofence.isInside = (int.Parse(dictGeofence["isInside"].ToString()) == 1);
				   geofence.title = dictGeofence["title"].ToString();
				   geofence.suid = dictGeofence["suid"].ToString();
				   cb(geofence);
			   };
		}

		public void SetIsDefaultLocationServiceEnabled(bool isEnable)
		{
			SHGeofence.instance().isDefaultLocationServiceEnabled = isEnable;
		}

		public bool GetIsDefaultLocationServiceEnabled()
		{
			return SHGeofence.instance().isDefaultLocationServiceEnabled;
		}

		public void SetIsLocationServiceEnabled(bool isEnable)
		{
			SHGeofence.instance().isLocationServiceEnabled = isEnable;
		}

		public bool GetIsLocationServiceEnabled()
		{
			return SHGeofence.instance().isLocationServiceEnabled;
		}

		public void StartGeofenceMonitoring()
		{
			Console.WriteLine("Android specific, not implemented in iOS.");
		}

		public void StopGeofenceMonitoring()
		{
			Console.WriteLine("Android specific, not implemented in iOS.");
		}

		public void StartGeofenceWithPermissionDialog(string message)
		{
			Console.WriteLine("Android specific, not implemented in iOS.");
		}
	}
}

