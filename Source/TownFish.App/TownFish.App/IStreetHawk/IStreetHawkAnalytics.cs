using System;

namespace StreetHawkCrossplatform
{
	/// <summary>
	/// Register for install successfully callback.
	/// </summary>
	public delegate void OnInstallRegisteredCallback(string installId);

	/// <summary>
	/// Register for callback to receive deeplink url for differed deeplinking in cross platforms.
	/// </summary>
	public delegate void RegisterForDeeplinkURLCallback(string url);

	public interface IStreetHawkAnalytics
	{
		/// <summary>
		/// Sets the app key.
		/// </summary>
		/// <param name="appKey">App key.</param>
		void SetAppKey(string appKey);

		/// <summary>
		/// Gets the app key.
		/// </summary>
		/// <returns>The app key.</returns>
		string GetAppKey();

		/// <summary>
		/// Sets whether enable console log. If enabled the SDK will prints log in XCode or Android Studio console.
		/// </summary>
		/// <param name="isEnable">Flag to indicate whether enable.</param>
		void SetEnableLogs(bool isEnable);

		/// <summary>
		/// Gets the flag whether enable console log. If enabled the SDK will prints log in XCode or Android Studio console.
		/// </summary>
		/// <returns>The flag of whether enable.</returns>
		bool GetEnableLogs();

		/// <summary>
		/// Init StreeHawk.
		/// </summary>
		void Init();

		/// <summary>
		/// Tags user's unique identifier (sh_cuid).
		/// </summary>
		/// <param name="cuid">Cuid.</param>
		void TagCuid(string cuid);

		/// <summary>
		/// Tags the user language (sh_language).
		/// </summary>
		/// <param name="language">The user language.</param>
		void TagUserLanguage(string language);

		/// <summary>
		/// Tags numeric event inside the app.
		/// </summary>
		/// <param name="key">Key for the event.</param>
		/// <param name="value">Value for the event.</param>
		void TagNumeric(string key, double value);

		/// <summary>
		/// Tags string event inside the app.
		/// </summary>
		/// <param name="key">Key for the event.</param>
		/// <param name="value">Value for the event.</param>
		void TagString(string key, string value);

		/// <summary>
		/// Tags datetime event for the app.
		/// </summary>
		/// <param name="key">Key for the event.</param>
		/// <param name="value">Value for the event.</param>
		void TagDateTime(string key, DateTime value);

		/// <summary>
		/// Increment the value of a previous tag by 1. If the tag is not present then API will create the Tag with value 1.
		/// </summary>
		/// <param name="key">Key for the event.</param>
		void IncrementTag(string key);

		/// <summary>
		/// Increments the tag with given value.  If the tag is not present then API will create the Tag with value given.
		/// </summary>
		/// <param name="key">Key for the event.</param>
		/// <param name="incrValue">Increase value.</param>
		void IncrementTag(string key, int incrValue);

		/// <summary>
		/// Removes the tag from StreetHawk server.
		/// </summary>
		/// <param name="key">Key for the event.</param>
		void RemoveTag(string key);

		/// <summary>
		/// Notifies the view entered by the user.
		/// </summary>
		/// <param name="viewName">View name.</param>
		void NotifyViewEnter(string viewName);

		/// <summary>
		/// Notifies the view exit by the user.
		/// </summary>
		/// <param name="viewName">View name.</param>
		void NotifyViewExit(string viewName);

		/// <summary>
		/// Sends the simple feedback.
		/// </summary>
		/// <param name="title">Title for the feedback.</param>
		/// <param name="message">Message for the feedback.</param>
		void SendSimpleFeedback(string title, string message);

		/// <summary>
		/// Gets the native SDK library version.
		/// </summary>
		/// <returns>The StreetHawk's library version.</returns>
		string GetSHLibraryVersion();

		/// <summary>
		/// Gets the current formatted date time. It's UTC and formatted as yyyy-MM-dd HH:mm:ss.
		/// </summary>
		/// <returns>The current formatted date time.</returns>
		string GetCurrentFormattedDateTime();

		/// <summary>
		/// Gets the formatted date time for given time since 1970 in seconds. It's UTC and formatted as yyyy-MM-dd HH:mm:ss.
		/// </summary>
		/// <returns>The formatted date time.</returns>
		/// <param name="time">Seconds since 1970.</param>
		string GetFormattedDateTime(long time);

		/// <summary>
		/// Gets the install id for the install in StreetHawk.
		/// </summary>
		/// <returns>The install identifier.</returns>
		string GetInstallId();

		/// <summary>
		/// Sets the advertisement identifier.
		/// </summary>
		/// <param name="id">The advertisement identifier.</param>
		void SetAdvertisementId(string id);

		/// <summary>
		/// Gets the advertisement identifier.
		/// </summary>
		/// <returns>The advertisement identifier.</returns>
		string GetAdvertisementId();

		/// <summary>
		/// Callback function which will be called when install is successfully registered with StreetHawk server.
		/// </summary>
		/// <param name="cb">Callback function.</param>
		void RegisterForInstallEvent(OnInstallRegisteredCallback cb);

		/// <summary>
		/// Registers for deeplink URL.
		/// </summary>
		/// <param name="cb">Callback for deeplink URL.</param>
		void RegisterForDeeplinkURL(RegisterForDeeplinkURLCallback cb);

		/*iOS only functions*/

		/// <summary>
		/// Sets iTunes Id of this App. It's the unique number for an App in AppStore. This is used for rate or upgrade App.
		/// </summary>
		/// <param name="iTunesId">iTunes Id string.</param>
		void SetsiTunesId(string iTunesId);

		/// <summary>
		/// Gets iTunes Id of this App. It's the unique number for an App in AppStore. This is used for rate or upgrade App.
		/// </summary>
		/// <return>iTunes Id string.</return>
		string GetsiTunesId();

		/*Android only functions*/

		/// <summary>
		/// Displays the badge. Note that for Android, the API depends on home screen launcher and badge wont be displayed on non supported devices.
		/// </summary>
		/// <returns>The badge.</returns>
		/// <param name="badgeCount">Badge count.</param>
		void DisplayBadge(int badgeCount);
	}
}

