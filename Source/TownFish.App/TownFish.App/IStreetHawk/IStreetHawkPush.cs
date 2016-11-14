using System;
using System.Collections.Generic;

namespace StreetHawkCrossplatform
{
	/// <summary>
	/// Callback to receive raw json push.
	/// </summary>
	public delegate void RegisterForShReceivedRawJSONCallback(string title, string message, string JSON);

	/// <summary>
	/// Callback to launch App page.
	/// </summary>
	public delegate void RegisterForShNotifyAppPageCallback(string appPage);

	/// <summary>
	/// Callback to receive a push content.
	/// </summary>
	public delegate void RegisterForOnReceivePushDataCallback(PushDataForApplication pushData);

	/// <summary>
	/// Callback to get a push content with its result. 1 for accepted, 0 for postpone, -1 for declined.
	/// </summary>
	public delegate void RegisterForOnReceiveResultCallback(PushDataForApplication pushData, int result);

	/// <summary>
	/// Callback to receive a none StreetHawk push payload.
	/// </summary>
	public delegate void RegisterForOnReceiveNonSHPushPayloadCallback(string payload);

	public interface IStreetHawkPush
	{
		/// <summary>
		/// Get remaining time in minutes till user won't receive push notification.
		/// </summary>
		/// <returns>The alert settings.</returns>
		int GetAlertSettings();

		/// <summary>
		/// Set in minutes time for which user won't be disturbed with push notification.
		/// </summary>
		/// <param name="minutes">The alert settings.</param>
		void SetAlertSettings(int minutes);

		/// <summary>
		/// Submit notification's interactive button pairs to server. The button pairs will be used to create campaign, and result in notification's interactive buttons.
		/// </summary>
		/// <param name="arrayPairs">The interactive push button pairs.</param>
		void SetInteractivePushBtnPairs(List<InteractivePush> arrayPairs);

		/// <summary>
		/// Registers for raw json.
		/// </summary>
		/// <param name="cb">Callback when receive raw json push.</param>
		void RegisterForRawJSON(RegisterForShReceivedRawJSONCallback cb);

		/// <summary>
		/// Callback when receive push data.
		/// </summary>
		/// <param name="cb">Callback with push data content.</param>
		void OnReceivePushData(RegisterForOnReceivePushDataCallback cb);

		/// <summary>
		/// Callback when get push result.
		/// </summary>
		/// <param name="cb">Callback with push data content and result.</param>
		void OnReceiveResult(RegisterForOnReceiveResultCallback cb);

		/// <summary>
		/// Callback for none StreetHawk payload arrives.
		/// </summary>
		/// <param name="cb">Callback with push content.</param>
		void OnReceiveNonSHPushPayload(RegisterForOnReceiveNonSHPushPayloadCallback cb);

		/// <summary>
		/// Sends the push result. Must call this after handle pushDataCallback to let process continue.
		/// </summary>
		/// <param name="msgid">The message id.</param>
		/// <param name="result">The push result. 1 for accepted, 0 for postpone, -1 for declined.</param>
		void SendPushResult(string msgid, int result);

		/*iOS only functions*/

		/// <summary>
		/// Sets default notification service enabled or not.
		/// </summary>
		/// <param name="enabled">Enable or not.</param>
		void SetIsDefaultNotificationServiceEnabled(bool enabled);

		/// <summary>
		/// Gets default notification service enabled or not.
		/// </summary>
		/// <return>Enable or not.</return>
		bool GetIsDefaultNotificationServiceEnabled();

		/// <summary>
		/// Sets current notification service enabled or not.
		/// </summary>
		/// <param name="enabled">Enable or not.</param>
		void SetIsNotificationServiceEnabled(bool enabled);

		/// <summary>
		/// Gets current notification service enabled or not.
		/// </summary>
		/// <return>Enable or not.</return>
		bool GetIsNotificationServiceEnabled();

		/*Android only functions*/


		/// <summary>
		/// Gets GCM (Google Cloud Message) id.
		/// </summary>
		/// <return>GCM sender Id.</return>
		string GetGcmSenderId();

		/// <summary>
		/// Callback when launch an App page or deeplink.
		/// </summary>
		/// <param name="cb">Callback when receive app page or deeplink.</param>
		void ShNotifyAppPage(RegisterForShNotifyAppPageCallback cb);

		/// <summary>
		/// Gets the name of the page which app should display when launched via deeplink or push message
		/// </summary>
		/// <returns>The app page.</returns>
		string GetAppPage();

		/// <summary>
		/// Forces the push to notification bar.
		/// </summary>
		/// <param name="status">Status.</param>
		void ForcePushToNotificationBar(bool status);

		/// <summary>
		/// Set to true if you wish to display customised dialog for push notifications.
		/// </summary>
		/// <param name="isUse">Whether use custom dialog.</param>
		void SetUseCustomDialog(bool isUse);

		/// <summary>
		/// Returns true if device is using push notifications else false.
		/// </summary>
		/// <returns>The use push.</returns>
		bool IsUsePush();

		/// <summary>
		/// Registers for push messaging by passing project number as received from StreetHawk server.
		/// </summary>
		/// <returns>The for push messaging.</returns>
		/// <param name="projectNumber">Project number.</param>
		void RegisterForPushMessaging(string projectNumber);

		/// <summary>
		/// Get the identifier for a given icon.
		/// </summary>
		/// <returns>The icon.</returns>
		/// <param name="iconName">Icon name.</param>
		int GetIcon(string iconName);

		//TODO
		void GetButtinPairFromId();
	}

	public class InteractivePush
	{
		/// <summary>
		/// Title for given pair, it's the identifier of pairs, case sensitive. 
		/// </summary>
		public string pairTitle { get; set; }

		/// <summary>
		/// Title for button 1, whose result is 1. For customized button, the action is always foreground. 
		/// </summary>
		public string b1Title { get; set; }

		/// <summary>
		/// Android Specific.
		/// Icon for button 1.
		/// </summary>
		public string b1Icon { get; set; }

		/// <summary>
		/// Title for button 2, whose result is -1. For customized button, the action is always foreground.
		/// </summary>
		public string b2Title { get; set; }

		/// <summary>
		/// Android Specific.
		/// Icon for button 2.
		/// </summary>
		public string b2Icon { get; set; }

		/// <summary>
		/// Creator.
		/// </summary>
		public InteractivePush(string pairTitle, string b1Title, string b1Icon, string b2Title, string b2Icon)
		{
			this.pairTitle = pairTitle;
			this.b1Title = b1Title;
			this.b1Icon = b1Icon;
			this.b2Title = b2Title;
			this.b2Icon = b2Icon;
		}
	}

	public enum SHAction
	{
		SHAction_OpenUrl = 1,
		SHAction_LaunchActivity = 2,
		SHAction_RateApp = 3,
		SHAction_UserRegistrationScreen = 4,
		SHAction_UserLoginScreen = 5,
		SHAction_UpdateApp = 6,
		SHAction_CallTelephone = 7,
		SHAction_SimplePrompt = 8,
		SHAction_Feedback = 9,
		SHAction_EnableBluetooth = 10,
		SHAction_EnablePushMsg = 11,
		SHAction_EnableLocation = 12,
		SHAction_CheckAppStatus = 13,
		SHAction_CustomJson = 14,
		SHAction_Ghost = 15,
		SHAction_Undefined = 16,
		SHAction_CustomAction = 17,
	}

	public enum SHSlideDirection
	{
		/// <summary>
		/// Move from device's bottom to up.
		/// </summary>
		SHSlideDirection_Up = 0,
		/// <summary>
		/// Move from device's top to down.
		/// </summary>
		SHSlideDirection_Down = 1,
		/// <summary>
		/// Move from device's right to left.
		/// </summary>
		SHSlideDirection_Left = 2,
		/// <summary>
		/// Move from device's left to right.
		/// </summary>
		SHSlideDirection_Right = 3,
	}

	public class PushDataForApplication
	{
		/// <summary>
		/// The title of this notification, usually used for title in UIAlertView.
		/// </summary>
		public string title { get; set; }

		/// <summary>
		/// The message of this notification, usually used for detail message in UIAlertView.
		/// </summary>
		public string message { get; set; }

		/// <summary>
		/// A flag to indicate not show confirm dialog.
		/// </summary>
		public bool displayWithoutDialog { get; set; }

		/// <summary>
		/// The data of this notification, it's different according to different push, for example it's url for SHAction_OpenUrl, it's telephone number for SHAction_CallTelephone.
		/// </summary>
		public string data { get; set; }

		/// <summary>
		/// The action of this notification.
		/// </summary>
		public SHAction action { get; set; }

		/// <summary>
		/// StreetHawk system defined code, used internally.
		/// </summary>
		public int code { get; set; }

		/// <summary>
		/// When the notification arrives, whether App on foreground or background.
		/// </summary>
		public bool isAppOnForeground { get; set; }

		/// <summary>
		/// The msg id from server inside this notification, used internally.
		/// </summary>
		public string msgID { get; set; }

		/// <summary>
		/// A flag indicate whether this notification is for slide.
		/// </summary>
		public bool isInAppSlide { get; set; }

		/// <summary>
		/// Used for SHAction_OpenUrl to slide web page inside App. This indicates how many percentage screen should be covered by web page. 
		/// </summary>
		public float portion { get; set; }

		/// <summary>
		/// Used for SHAction_OpenUrl to slide web page inside App. This indicates the direction where web page slide in.
		/// </summary>
		public SHSlideDirection orientation { get; set; }

		/// <summary>
		/// Used for SHAction_OpenUrl to slide web page inside App. This indicates how many seconds the animation takes.
		/// </summary>
		public float speed { get; set; }

		/// <summary>
		/// The sound file name in notification payload. Normally no need to handle this in iOS, system play the sound automatically when notification arrives. 
		/// </summary>
		public string sound { get; set; }

		/// <summary>
		/// The badge number in notification payload. Normally no need to handle this in iOS, system set badge in App icon automatically when notification arrives. 
		/// </summary>
		public int badge { get; set; }

		/// <summary>
		/// The category string identifier for interactive buttons.
		/// </summary>
		public string category { get; set; }

	}
}

