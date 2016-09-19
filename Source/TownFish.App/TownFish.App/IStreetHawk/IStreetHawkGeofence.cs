using System;

namespace StreetHawkCrossplatform
{
	/// <summary>
	/// Callback when enter/exit server monitoring geofence.
	/// </summary>
	public delegate void RegisterForGeofenceCallback(SHGeofenceObj geofence);

	public interface IStreetHawkGeofence
	{
		/// <summary>
		/// Callback when enter/exit server monitoring geofence.
		/// </summary>
		/// <param name="cb">The geofence enter or exit.</param>
		void RegisterForGeofenceStatus(RegisterForGeofenceCallback cb);

		/*iOS only functions*/

		/// <summary>
		/// Set default location service enabled or not.
		/// </summary>
		/// <param name="isEnable">Flag for is enabled or not.</param>
		void SetIsDefaultLocationServiceEnabled(bool isEnable);

		/// <summary>
		/// Get default location service enabled or not.
		/// </summary>
		/// <return>Flag for is enabled or not.</return>
		bool GetIsDefaultLocationServiceEnabled();

		/// <summary>
		/// Set current location service enabled or not.
		/// </summary>
		/// <param name="isEnable">Flag for is enabled or not.</param>
		void SetIsLocationServiceEnabled(bool isEnable);

		/// <summary>
		/// Get current location service enabled or not.
		/// </summary>
		/// <return>Flag for is enabled or not.</return>
		bool GetIsLocationServiceEnabled();

		/*Android only functions*/

		/// <summary>
		/// Starts the geofence monitoring.
		/// </summary>
		void StartGeofenceMonitoring();

		/// <summary>
		/// Stops the monitoring.
		/// </summary>
		void StopGeofenceMonitoring();

		/// <summary>
		/// Start geofence monitoring and show message.
		/// </summary>
		/// <param name="message">The message show in permission dialog.</param>
		void StartGeofenceWithPermissionDialog(string message);
	}

	public class SHGeofenceObj
	{
		/// <summary>
		/// Latitude of this geofence.
		/// </summary>
		public double latitude;

		/// <summary>
		/// Longitude of this geofence.
		/// </summary>
		public double longitude;

		/// <summary>
		/// Radius of this geofence.
		/// </summary>
		public double radius;

		/// <summary>
		/// StreetHawk server unique id of this geofence.
		/// </summary>
		public int serverId;

		/// <summary>
		/// Whether this geofence is inside or outside.
		/// </summary>
		public bool isInside;

		/// <summary>
		/// Title of this geofence.
		/// </summary>
		public string title;

		/// <summary>
		/// StreetHawk internal unique id.
		/// </summary>
		public string suid;
	}
}

