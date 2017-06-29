using System;
using System.Collections.Generic;

using Android.App;
using Android.Util;
using Android.OS;

using Com.Streethawk.Library.Push;
using StreetHawkCrossplatform;
using Android.Content;
using TownFish.App.Droid;
using Android.Support.V7.App;
using Android.Graphics;

[assembly: Xamarin.Forms.Dependency(typeof(StreetHawkPush))]


namespace StreetHawkCrossplatform
{
	public class StreetHawkPush : Java.Lang.Object, IStreetHawkPush
	{
		static Application mApplication => StreetHawkAnalytics.Application;

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
			Log.Error("StreetHawk", "Function GetAppPage is not implemented in this release ");
			return null;
		}

		public void GetButtinPairFromId()
		{
			Log.Error("StreetHawk", "Function GetButtinPairFromId is not implemented in this release ");
		}

		public string GetGcmSenderId()
		{
			Log.Error("StreetHawk", "Function GetGcmSenderId is not implemented in this release ");
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

		RegisterForOnReceiveNonSHPushPayloadCallback mNonSHPushPayloadCB;
		public void OnReceiveNonSHPushPayload(RegisterForOnReceiveNonSHPushPayloadCallback cb)
		{
			mNonSHPushPayloadCB = cb;
		}

		RegisterForOnReceivePushDataCallback mPushDataCallback;
		public void OnReceivePushData(RegisterForOnReceivePushDataCallback cb)
		{
			mPushDataCallback = cb;
		}

		RegisterForOnReceiveResultCallback mPushResultCallback;
		public void OnReceiveResult(RegisterForOnReceiveResultCallback cb)
		{
			mPushResultCallback = cb;
		}

		public void RegisterForPushMessaging(string projectNumber)
		{
			Push.GetInstance(mApplication.ApplicationContext).RegisterForPushMessaging(projectNumber);
        }

        RegisterForShReceivedRawJSONCallback mRawJSONCB;
		public void RegisterForRawJSON(RegisterForShReceivedRawJSONCallback cb)
		{
			mRawJSONCB = cb;
            JsonService.Callback = cb;
		}

       // Starting our service that implements ISHObserver
        public void Register()
        {
            Intent serviceToStart = new Intent(Xamarin.Forms.Forms.Context, typeof(JsonService));
            Xamarin.Forms.Forms.Context.StartService(serviceToStart);
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
			Log.Error("StreetHawk", "Function SetIsDefaultNotificationServiceEnabled is not available for Android ");
		}

		public void SetIsNotificationServiceEnabled(bool enabled)
		{
			Log.Error("StreetHawk", "Function SetIsNotificationServiceEnabled is not available for Android ");
		}

		public void SetUseCustomDialog(bool isUse)
		{
			Push.GetInstance(mApplication.ApplicationContext).SetUseCustomDialog(isUse);
		}

		RegisterForShNotifyAppPageCallback mNotifyAppPageCB;
		public void ShNotifyAppPage(RegisterForShNotifyAppPageCallback cb)
		{
			mNotifyAppPageCB = cb;
		}


    }
}
