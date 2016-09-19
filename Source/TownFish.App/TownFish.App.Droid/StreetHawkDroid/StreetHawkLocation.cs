using System;

using StreetHawkCrossplatform;
using Com.Streethawk.Library.Locations;
using Android.App;
using Android.Util;

using XamHawkDemo.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(StreetHawkLocation))]
namespace XamHawkDemo.Droid
{
	public class StreetHawkLocation:IStreetHawkLocations
	{
		private static Application mApplication;

		public StreetHawkLocation() { }

		public StreetHawkLocation(Application application)
		{
			mApplication = application;
		}

		public bool GetIsDefaultLocationServiceEnabled()
		{
			Log.Error("StreetHawk", "Function GetIsDefaultLocationServiceEnabled is not implemented for Android");
			return false;
		}

		public bool GetIsLocationServiceEnabled()
		{
			Log.Error("StreetHawk", "Function GetIsLocationServiceEnabled is not implemented for Android");
			return false;
		}

		public void ReportWorkHomeLocationsOnly()
		{
			SHLocation.GetInstance(mApplication.ApplicationContext).ReportWorkHomeLocationsOnly();
		}

		public void SetIsDefaultLocationServiceEnabled(bool isEnable)
		{
			Log.Error("StreetHawk", "Function SetIsDefaultLocationServiceEnabled is not implemented for Android");
		}

		public void SetIsLocationServiceEnabled(bool isEnable)
		{
			Log.Error("StreetHawk", "Function SetIsLocationServiceEnabled is not implemented for Android");
		}

		public void StartLocationReporting()
		{
			SHLocation.GetInstance(mApplication.ApplicationContext).StartLocationReporting();
		}

		public void StartLocationWithPermissionDialog(string message)
		{
			Log.Error("StreetHawk", "Function StartLocationWithPermissionDialog is not implemented for Android");
		}

		public void StopLocationReporting()
		{
			SHLocation.GetInstance(mApplication.ApplicationContext).StopLocationReporting();
		}

		public void UpdateLocationMonitoringParams(int UPDATE_INTERVAL_FG, int UPDATE_DISTANCE_FG, int UPDATE_INTERVAL_BG, int UPDATE_DISTANCE_BG)
		{
			SHLocation.GetInstance(mApplication.ApplicationContext).UpdateLocationMonitoringParams(UPDATE_INTERVAL_FG, UPDATE_DISTANCE_FG,UPDATE_INTERVAL_BG,UPDATE_DISTANCE_BG);
		}
		public IntPtr Handle
		{
			get;
			set;
		}

		public void Dispose()
		{

		}
	}
}