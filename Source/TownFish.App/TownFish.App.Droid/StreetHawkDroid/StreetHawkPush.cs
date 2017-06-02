﻿using System;
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
	public class StreetHawkPush : Java.Lang.Object, IStreetHawkPush, ISHObserver
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
			Push.GetInstance(mApplication.ApplicationContext).RegisterSHObserver(this);
		}

		RegisterForShReceivedRawJSONCallback mRawJSONCB;
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
				appData.msgID = pushData.MsgId;
				appData.portion = pushData.Portion;
				appData.orientation = (SHSlideDirection)pushData.Orientation;
				appData.speed = pushData.Speed;
				appData.sound = pushData.Sound;
				appData.badge = pushData.Badge;
			}

			mPushDataCallback?.Invoke(appData);
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
				appData.msgID = pushData.MsgId;
				appData.portion = pushData.Portion;
				appData.orientation = (SHSlideDirection)pushData.Orientation;
				appData.speed = pushData.Speed;
				appData.sound = pushData.Sound;
				appData.badge = pushData.Badge;
			}

			mPushResultCallback?.Invoke(appData,result);
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
            if (MainActivity.IsActive)
            {
                if (null != mRawJSONCB)
                {
                    mRawJSONCB.Invoke(title, message, json);
                }
            }
            else
            {
                SendNotification(title, message, json);
            }
        }

        void SendNotification(string title, string body, string json)
        {
            var intent = new Intent(mApplication.ApplicationContext, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            intent.PutExtra("json", json);
            var pendingIntent = PendingIntent.GetActivity(mApplication.ApplicationContext, 0, intent, PendingIntentFlags.UpdateCurrent);

            var notificationBuilder = new NotificationCompat.Builder(mApplication.ApplicationContext)
                .SetLargeIcon(BitmapFactory.DecodeResource(mApplication.Resources, Resource.Drawable.icon))
                .SetSmallIcon(Resource.Drawable.icon)
                .SetContentTitle(title)
                .SetContentText(body)
                .SetAutoCancel(true)
                .SetExtras(intent.Extras)
                .SetDefaults((int)(NotificationDefaults.Vibrate | NotificationDefaults.Sound))
                .SetContentIntent(pendingIntent);

            var notificationManager = (NotificationManager)mApplication.ApplicationContext.GetSystemService(Context.NotificationService);
            notificationManager.Notify(0, notificationBuilder.Build());
        }

    }
}
