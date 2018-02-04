using System;
using System.Collections.Generic;
using Foundation;
using Xamarin.Forms;

using StreetHawkCrossplatform;
using StreethawkIOS.Push;

[assembly: Dependency(typeof(StreetHawkPush))]

namespace StreetHawkCrossplatform
{
	public class StreetHawkPush : ISHCustomiseHandler, IStreetHawkPush
	{
		public StreetHawkPush()
		{
			SHPush.instance().shSetCustomiseHandler(this);
		}

		public int GetAlertSettings()
		{ 
			return SHPush.instance().shAlertSetting;
		}

		public void SetAlertSettings(int minutes)
		{
			SHPush.instance().shAlertSetting = minutes;
		}

		public void SetInteractivePushBtnPairs(List<InteractivePush> arrayPairs)
		{ 
			StreethawkIOS.Push.InteractivePush[] array = new StreethawkIOS.Push.InteractivePush[arrayPairs.Count];
			for (int i = 0; i < arrayPairs.Count; i++)
			{
				array[i] = new StreethawkIOS.Push.InteractivePush(arrayPairs[i].pairTitle, arrayPairs[i].b1Title, arrayPairs[i].b2Title);
			}
			SHPush.instance().setInteractivePushBtnPairs(array);
		}

		private RegisterForShReceivedRawJSONCallback _shRawJsonCallback;
		public void RegisterForRawJSON(RegisterForShReceivedRawJSONCallback cb)
		{
			_shRawJsonCallback = cb;
		}

		private RegisterForOnReceivePushDataCallback _pushDataCallback;
		public void OnReceivePushData(RegisterForOnReceivePushDataCallback cb)
		{
			_pushDataCallback = cb;
		}

		private RegisterForOnReceiveResultCallback _pushResultCallback;
		public void OnReceiveResult(RegisterForOnReceiveResultCallback cb)
		{
			_pushResultCallback = cb;
		}

		public void OnReceiveNonSHPushPayload(RegisterForOnReceiveNonSHPushPayloadCallback cb)
		{
			SHPush.instance().registerNonSHPushPayloadObserver = delegate (NSDictionary dictPayload)
			   {
				   NSError error = null;
				   NSData data = NSJsonSerialization.Serialize(dictPayload, 0, out error);
				   cb(data.ToString(NSStringEncoding.UTF8));
			   };
		}

		private List<KeyValuePair<string, ClickButtonHandler>> dictPushMsgHandler = new List<KeyValuePair<string, ClickButtonHandler>>();
		public void SendPushResult(string msgid, int result)
		{
			KeyValuePair<string, ClickButtonHandler> findItem = default(KeyValuePair<string, ClickButtonHandler>);
			for (int i = 0; i < this.dictPushMsgHandler.Count; i++)
			{
				if (this.dictPushMsgHandler[i].Key.CompareTo(msgid) == 0)
				{
					findItem = this.dictPushMsgHandler[i];
					break;
				}
			}
			if (!findItem.Equals(default(KeyValuePair<string, ClickButtonHandler>)))
			{
				findItem.Value((StreethawkIOS.Push.SHPushResult)result);
				this.dictPushMsgHandler.Remove(findItem);
			}
		}

		public void SetIsDefaultNotificationServiceEnabled(bool enabled)
		{ 
			SHPush.instance().isDefaultNotificationEnabled = enabled;
		}

		public bool GetIsDefaultNotificationServiceEnabled()
		{ 
			return SHPush.instance().isDefaultNotificationEnabled;
		}

		public void SetIsNotificationServiceEnabled(bool enabled)
		{
			SHPush.instance().isNotificationEnabled = enabled;
		}

		public bool GetIsNotificationServiceEnabled()
		{ 
			return SHPush.instance().isNotificationEnabled;
		}

		public void SetGcmSenderId(string gcmId)
		{ 
			Console.WriteLine("Android specific, not implemented in iOS.");
		}

		public string GetGcmSenderId()
		{
			Console.WriteLine("Android specific, not implemented in iOS.");
			return null;
		}

		public void ShNotifyAppPage(RegisterForShNotifyAppPageCallback cb)
		{
			Console.WriteLine("Android specific, not implemented in iOS.");
		}

		public string GetAppPage()
		{ 
			Console.WriteLine("Android specific, not implemented in iOS.");
			return null;
		}

		public void ForcePushToNotificationBar(bool status)
		{
			Console.WriteLine("Android specific, not implemented in iOS.");
		}

		public void SetUseCustomDialog(bool isUse)
		{
			Console.WriteLine("Android specific, not implemented in iOS.");
		}

		public bool IsUsePush()
		{
			Console.WriteLine("Android specific, not implemented in iOS.");
			return true;
		}

		public void RegisterForPushMessaging(string projectNumber)
		{
			Console.WriteLine("Android specific, not implemented in iOS.");
		}

		public int GetIcon(string iconName)
		{
			Console.WriteLine("Android specific, not implemented in iOS.");
			return 0;
		}

		public void GetButtinPairFromId()
		{
			Console.WriteLine("Android specific, not implemented in iOS.");
		}

		public override bool onReceive(StreethawkIOS.Push.PushDataForApplication pushData, ClickButtonHandler handler)
		{
			if (_pushDataCallback != null)
			{
				StreetHawkCrossplatform.PushDataForApplication data = new PushDataForApplication();
				data.code = pushData.code;
				data.action = (StreetHawkCrossplatform.SHAction)pushData.action;
				data.msgID = pushData.msgID;
				data.title = pushData.title;
				data.message = pushData.message;
				data.data = pushData.data.Description;
				data.isAppOnForeground = pushData.isAppOnForeground;
				data.portion = pushData.portion;
				data.orientation = (StreetHawkCrossplatform.SHSlideDirection)pushData.orientation;
				data.speed = pushData.speed;
				data.sound = pushData.sound;
				data.badge = pushData.badge;
				data.displayWithoutDialog = pushData.displayWithoutDialog;
				data.isInAppSlide = pushData.isInAppSlide;
				data.category = pushData.category;
				//insert into memory cache
				if (handler != null)
				{
					ClickButtonHandler findHandler = null;
					for (int i = 0; i < this.dictPushMsgHandler.Count; i++)
					{
						if (this.dictPushMsgHandler[i].Key == data.msgID)
						{
							findHandler = this.dictPushMsgHandler[i].Value;
							break;
						}
					}
					if (findHandler == null)
					{
						this.dictPushMsgHandler.Add(new KeyValuePair<string, ClickButtonHandler>(data.msgID, handler));
					}
				}
				_pushDataCallback(data);
				return true; //not able to get return value from js, so once implement `registerPushDataCallback` all confirm dialog use js, not like native sdk which can override some and leave others not affected.
			}
			return true;
		}

		public override void onReceiveResult(StreethawkIOS.Push.PushDataForApplication pushData, StreethawkIOS.Push.SHPushResult result)
		{
			if (_pushResultCallback != null)
			{
				StreetHawkCrossplatform.PushDataForApplication data = new PushDataForApplication();
				data.code = pushData.code;
				data.action = (StreetHawkCrossplatform.SHAction)pushData.action;
				data.msgID = pushData.msgID;
				data.title = pushData.title;
				data.message = pushData.message;
				data.data = pushData.data.Description;
				data.isAppOnForeground = pushData.isAppOnForeground;
				data.portion = pushData.portion;
				data.orientation = (StreetHawkCrossplatform.SHSlideDirection)pushData.orientation;
				data.speed = pushData.speed;
				data.sound = pushData.sound;
				data.badge = pushData.badge;
				data.displayWithoutDialog = pushData.displayWithoutDialog;
				data.isInAppSlide = pushData.isInAppSlide;
				data.category = pushData.category;
				_pushResultCallback(data, (int)result);
			}
		}

		public override void shRawJsonCallbackWithTitle(string title, string message, NSDictionary json)
		{
			if (_shRawJsonCallback != null)
			{
				NSError error;
				NSData data = NSJsonSerialization.Serialize(json, 0, out error);
				NSString jsonStr = new NSString(data, NSStringEncoding.UTF8);
				_shRawJsonCallback(title, message, jsonStr.ToString());
			}
		}

        public void Register()
        {
            // Empty method, put anything you like here !
            // See Android StreetHawkPush.cs 
        }
    }
}

