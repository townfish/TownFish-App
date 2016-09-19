using System;

namespace StreetHawkCrossplatform
{
	public interface IStreetHawkLocations
	{
		/// <summary>
		/// Reports the work home locations only.
		/// </summary>
		void ReportWorkHomeLocationsOnly();

		/// <summary>
		/// Updates the location monitoring parameters.
		/// </summary>
		/// <param name="UPDATE_INTERVAL_FG">Update interval fg.</param>
		/// <param name="UPDATE_DISTANCE_FG">Update distance fg.</param>
		/// <param name="UPDATE_INTERVAL_BG">Update interval background.</param>
		/// <param name="UPDATE_DISTANCE_BG">Update distance background.</param>
		void UpdateLocationMonitoringParams(int UPDATE_INTERVAL_FG, int UPDATE_DISTANCE_FG, int UPDATE_INTERVAL_BG, int UPDATE_DISTANCE_BG);

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
		/// Starts the location reporting.
		/// </summary>
		void StartLocationReporting();

		/// <summary>
		/// Stops the location reporting.
		/// </summary>
		void StopLocationReporting();

		/// <summary>
		/// Start location and show message.
		/// </summary>
		/// <param name="message">The message show in permission dialog.</param>
		void StartLocationWithPermissionDialog(string message);
	}
}

