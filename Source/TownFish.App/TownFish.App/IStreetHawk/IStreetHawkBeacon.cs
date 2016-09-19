using System;

namespace StreetHawkCrossplatform
{
	/// <summary>
	/// Callback when enter/exit server monitoring beacon.
	/// </summary>
	public delegate void RegisterForBeaconCallback(SHBeaconObj beacon);

	public interface IStreetHawkBeacon
	{
		/// <summary>
		/// Callback when enter/exit server monitoring beacon.
		/// </summary>
		/// <param name="cb">The beacon enter or exit.</param>
		void RegisterForBeaconStatus(RegisterForBeaconCallback cb);

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
		/// Notify beacon enter if you are using a third party library to detect beacons
		/// </summary>
		/// <returns>The enter beacon.</returns>
		/// <param name="uuid">UUID.</param>
		/// <param name="major">Major. Major number of detected beacon</param>
		/// <param name="minor">Minor. Minor number of detected beacon</param>
		/// <param name="distance">Distance. Distance between beaocn and device</param>
		int ShEnterBeacon(string uuid, int major, int minor, double distance);

		/// <summary>
		/// Notify beacon exit if you are using a third party library to detect beaocn exits
		/// </summary>
		/// <returns>The exit beacon.</returns>
		/// <param name="uuid">UUID.</param>
		/// <param name="major">Major.</param>
		/// <param name="minor">Minor.</param>
		int ShExitBeacon(string uuid, int major, int minor);

		/// <summary>
		/// Starts the beacon monitoring.
		/// </summary>
		void StartBeaconMonitoring();

		/// <summary>
		/// Stops the beacon monitoring.
		/// </summary>
		void StopBeaconMonitoring();
	}

	public class SHBeaconObj
	{
		/// <summary>
		/// UUID of this beacon.
		/// </summary>
		public string uuid;

		/// <summary>
		/// Major of this beacon.
		/// </summary>
		public int major;

		/// <summary>
		/// Minor of this beacon.
		/// </summary>
		public int minor;

		/// <summary>
		/// StreetHawk server unique id of this beacon.
		/// </summary>
		public int serverId;

		/// <summary>
		/// Whether this beacon is inside or outside.
		/// </summary>
		public bool isInside;
	}
}

