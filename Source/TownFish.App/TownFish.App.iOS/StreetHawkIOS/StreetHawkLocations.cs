using System;
using Foundation;
using Xamarin.Forms;

using StreetHawkCrossplatform;
using StreethawkIOS.Location;

[assembly: Dependency(typeof(StreetHawkLocations))]

namespace StreetHawkCrossplatform
{
	public class StreetHawkLocations : IStreetHawkLocations
	{
		public void ReportWorkHomeLocationsOnly()
		{
			SHLocation.instance().reportWorkHomeLocationOnly = true;
		}

		public void UpdateLocationMonitoringParams(int UPDATE_INTERVAL_FG, int UPDATE_DISTANCE_FG, int UPDATE_INTERVAL_BG, int UPDATE_DISTANCE_BG)
		{
			SHLocation.instance().updateLocationMonitoringParams(UPDATE_INTERVAL_FG, UPDATE_DISTANCE_FG, UPDATE_INTERVAL_BG, UPDATE_DISTANCE_BG);
		}

		public void SetIsDefaultLocationServiceEnabled(bool isEnable)
		{
			SHLocation.instance().isDefaultLocationServiceEnabled = isEnable;
		}

		public bool GetIsDefaultLocationServiceEnabled()
		{
			return SHLocation.instance().isDefaultLocationServiceEnabled;
		}

		public void SetIsLocationServiceEnabled(bool isEnable)
		{
			SHLocation.instance().isLocationServiceEnabled = isEnable;
		}

		public bool GetIsLocationServiceEnabled()
		{
			return SHLocation.instance().isLocationServiceEnabled;
		}

		public void StartLocationReporting()
		{
			Console.WriteLine("Android specific, not implemented in iOS.");
		}

		public void StopLocationReporting()
		{
			Console.WriteLine("Android specific, not implemented in iOS.");
		}

		public void StartLocationWithPermissionDialog(string message)
		{
			Console.WriteLine("Android specific, not implemented in iOS.");
		}
	}
}

