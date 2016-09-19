using System;
using Foundation;
using Xamarin.Forms;

using StreetHawkCrossplatform;
using StreethawkIOS.Beacon;

[assembly: Dependency(typeof(StreetHawkBeacon))]

namespace StreetHawkCrossplatform
{
	public class StreetHawkBeacon : IStreetHawkBeacon
	{
		public void RegisterForBeaconStatus(RegisterForBeaconCallback cb)
		{ 
			SHBeacon.instance().notifyBeaconDetectCallback = delegate (NSDictionary dictBeacon)
			   {
				   SHBeaconObj beacon = new SHBeaconObj();
				   beacon.uuid = dictBeacon["uuid"].ToString();
				   beacon.major = int.Parse(dictBeacon["major"].ToString());
				   beacon.minor = int.Parse(dictBeacon["minor"].ToString());
				   beacon.serverId = int.Parse(dictBeacon["serverId"].ToString());
				   beacon.isInside = (int.Parse(dictBeacon["isInside"].ToString()) == 1);
				   cb(beacon);
			   };
		}

		public void SetIsDefaultLocationServiceEnabled(bool isEnable)
		{
			SHBeacon.instance().isDefaultLocationServiceEnabled = isEnable;
		}

		public bool GetIsDefaultLocationServiceEnabled()
		{
			return SHBeacon.instance().isDefaultLocationServiceEnabled; 
		}

		public void SetIsLocationServiceEnabled(bool isEnable)
		{ 
			SHBeacon.instance().isLocationServiceEnabled = isEnable;
		}

		public bool GetIsLocationServiceEnabled()
		{
			return SHBeacon.instance().isLocationServiceEnabled;
		}

		public int ShEnterBeacon(string uuid, int major, int minor, double distance)
		{ 
			Console.WriteLine("Android specific, not implemented in iOS.");
			return 0;
		}

		public int ShExitBeacon(string uuid, int major, int minor)
		{
			Console.WriteLine("Android specific, not implemented in iOS.");
			return 0;
		}

		public void StartBeaconMonitoring()
		{
			Console.WriteLine("Android specific, not implemented in iOS.");
		}

		public void StopBeaconMonitoring()
		{
			Console.WriteLine("Android specific, not implemented in iOS.");
		}
	}
}

