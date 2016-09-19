using System;
using StreetHawkCrossplatform;
using Com.Streethawk.Library.Push;
using Android.App;
using Android.Util;
using System.Collections.Generic;
using Android.OS;

[assembly: Xamarin.Forms.Dependency(typeof(StreetHawkPush))]
namespace StreetHawkCrossplatform
{
	public class StreetHawkPush : IStreetHawkPush, ISHObserver
	{

		private static Application mApplication;

		public StreetHawkPush() { }

		public StreetHawkPush(Application application)
		{
			mApplication = application;
		}

		public void ForcePushToNotificationBar(bool status)
		{
			Push.GetInstance(mApplication.ApplicationContext).ForcePushToNotificationBar(status);
		}

		public int GetAlertSettings()
		{
			return Push.GetInstance(mApplication.ApplicationContext).ShGetAlertSettings();
		}

		public string GetAppPage()
		{
			//TODO
			Log.Error("StreetHawk","Function GetAppPage is not imeplemnted in this release ");
			return null;
		}

		public void GetButtinPairFromId()
		{
			Log.Error("StreetHawk", "Function GetButtinPairFromId is not imeplemnted in this release ");
		}

		public string GetGcmSenderId()
		{
			Log.Error("StreetHawk", "Function GetGcmSenderId is not imeplemnted in this release ");
			return null;
		}

		public int GetIcon(string iconName)
		{
			Log.Error("StreetHawk", "Function GetIcon is not implemented in this release ");
			return -1;
		}

		public bool GetIsDefaultNotificationServiceEnabled()
		{
			Log.Error("StreetHawk", "Function GetIsDefaultNotificationServiceEnabled is not available for Android");
			return true;
		}

		public bool GetIsNotificationServiceEnabled()
		{
			Log.Error("StreetHawk", "Function GetIsDefaultNotificationServiceEnabled is not available for Android");
			return true;
		}

		public bool IsUsePush()
		{
			Log.Error("StreetHawk", "Function IsUsePush is not implemented in this release");
			return true;
		}

		private static RegisterForOnReceiveNonSHPushPayloadCallback mNonSHPushPayloadCB;
		public void OnReceiveNonSHPushPayload(RegisterForOnReceiveNonSHPushPayloadCallback cb)
		{
			mNonSHPushPayloadCB = cb;
		}
		private static RegisterForOnReceivePushDataCallback mPushDataCallback;
		public void OnReceivePushData(RegisterForOnReceivePushDataCallback cb)
		{
			mPushDataCallback = cb;
		}


		private static RegisterForOnReceiveResultCallback mPushResultCallback;
		public void OnReceiveResult(RegisterForOnReceiveResultCallback cb)
		{
			mPushResultCallback = cb;
		}

		public void RegisterForPushMessaging(string projectNumber)
		{
			Push.GetInstance(mApplication.ApplicationContext).RegisterForPushMessaging(projectNumber);
			Push.GetInstance(mApplication.ApplicationContext).RegisterSHObserver(this);
		}

		private static RegisterForShReceivedRawJSONCallback mRawJSONCB;
		public void RegisterForRawJSON(RegisterForShReceivedRawJSONCallback cb)
		{
			mRawJSONCB = cb;
		}

		public void SendPushResult(string msgid, int result)
		{
			Push.GetInstance(mApplication.ApplicationContext).SendPushResult(msgid, result);
		}

		public void SetAlertSettings(int minutes)
		{
			Push.GetInstance(mApplication.ApplicationContext).ShAlertSetting(minutes);
		}

		public void SetInteractivePushBtnPairs(List<StreetHawkCrossplatform.InteractivePush> arrayPairs)
		{
			throw new NotImplementedException();
		}

		public void SetIsDefaultNotificationServiceEnabled(bool enabled)
		{
			Log.Error("Anurag","Function SetIsDefaultNotificationServiceEnabled is not available for Android ");
		}

		public void SetIsNotificationServiceEnabled(bool enabled)
		{
			Log.Error("Anurag", "Function SetIsNotificationServiceEnabled is not available for Android ");
		}

		public void SetUseCustomDialog(bool isUse)
		{
			Push.GetInstance(mApplication.ApplicationContext).SetUseCustomDialog(isUse);
		}

		private static RegisterForShNotifyAppPageCallback mNotifyAppPageCB;

		public IntPtr Handle
		{
			get;
			set;
		}

		public void ShNotifyAppPage(RegisterForShNotifyAppPageCallback cb)
		{
			mNotifyAppPageCB = cb;
		}

		public void OnReceiveNonSHPushPayload(Bundle bundle)
		{
			bundle.ToString();
		}

		public void OnReceivePushData(Com.Streethawk.Library.Push.PushDataForApplication pushData)
		{
			StreetHawkCrossplatform.PushDataForApplication appData = new StreetHawkCrossplatform.PushDataForApplication();
			if (null != pushData)
			{
				appData.title = pushData.Title;
				appData.message = pushData.Message;
				appData.displayWithoutDialog = (bool)pushData.DisplayWithoutConfirmation;
				appData.data = pushData.Data;
				appData.action = (SHAction)pushData.Action;
				appData.isAppOnForeground = false;
				appData.msgID = Int32.Parse(pushData.MsgId);
				appData.portion = pushData.Portion;
				appData.orientation = (SHSlideDirection)pushData.Orientation;
				appData.speed = pushData.Speed;
				appData.sound = pushData.Sound;
				appData.badge = pushData.Badge;
			}
			mPushDataCallback.Invoke(appData);
		}

		public void OnReceiveResult(Com.Streethawk.Library.Push.PushDataForApplication pushData, int result)
		{
			StreetHawkCrossplatform.PushDataForApplication appData = new StreetHawkCrossplatform.PushDataForApplication();
			if (null != pushData)
			{
				appData.title = pushData.Title;
				appData.message = pushData.Message;
				appData.displayWithoutDialog = (bool)pushData.DisplayWithoutConfirmation;
				appData.data = pushData.Data;
				appData.action = (SHAction)pushData.Action;
				appData.isAppOnForeground = false;
				appData.msgID = Int32.Parse(pushData.MsgId);
				appData.portion = pushData.Portion;
				appData.orientation = (SHSlideDirection)pushData.Orientation;
				appData.speed = pushData.Speed;
				appData.sound = pushData.Sound;
				appData.badge = pushData.Badge;

			}
			mPushResultCallback.Invoke(appData,result);
		}

		public void ShNotifyAppPage(string appPage)
		{
			if (null != mNotifyAppPageCB)
			{
				mNotifyAppPageCB.Invoke(appPage);
			}
		}

		public void ShReceivedRawJSON(string title, string message, string json)
		{
			if (null != mRawJSONCB)
			{
				mRawJSONCB.Invoke(title, message, json);
			}
		}

		public void Dispose()
		{
			
		}
	}
}

