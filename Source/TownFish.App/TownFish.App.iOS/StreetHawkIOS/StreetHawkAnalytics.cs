using System;
using Foundation;
using Xamarin.Forms;

using StreetHawkCrossplatform;
using StreethawkIOS.Core;
using UIKit;

[assembly: Dependency(typeof(StreetHawkAnalytics))]

namespace StreetHawkCrossplatform
{
	public class StreetHawkAnalytics : IStreetHawkAnalytics
	{
		public void SetAppKey(string appKey)
		{
			SHApp.instance().appKey = appKey;
		}

		public string GetAppKey()
		{ 
			return SHApp.instance().appKey;
		}

		public void SetEnableLogs(bool isEnable)
		{ 
			SHApp.instance().enableLogs = isEnable;
		}

		public bool GetEnableLogs()
		{ 
			return SHApp.instance().enableLogs;
		}

		public void Init()
		{ 
			SHApp.instance().streethawkinit();
		}

		public void TagCuid(string cuid)
		{
			SHApp.instance().tagCuid(cuid);
		}

		public void TagUserLanguage(string language)
		{
			SHApp.instance().tagUserLanguage(language);
		}

		public void TagNumeric(string key, double value)
		{
            if (key == "sh_badge_number")
            {
                UIApplication.SharedApplication.ApplicationIconBadgeNumber = (int)value;
            }

            SHApp.instance().tagNumeric(key, value);
		}

		public void TagString(string key, string value)
		{
			SHApp.instance().tagString(key, value);
		}

		public void TagDateTime(string key, DateTime value)
		{
			SHApp.instance().tagDatetime(key, convertDatetime(value));
		}

		public void IncrementTag(string key)
		{
			SHApp.instance().incrementTag(key);
		}

		public void IncrementTag(string key, int incrValue)
		{
			SHApp.instance().incrementTag(key, incrValue);
		}

		public void RemoveTag(string key)
		{
			SHApp.instance().removeTag(key);
		}

		public void NotifyViewEnter(string viewName)
		{ 
			SHApp.instance().notifyViewEnter(viewName);
		}

		public void NotifyViewExit(string viewName)
		{ 
			SHApp.instance().notifyViewExit(viewName);
		}

		public void SendSimpleFeedback(string title, string message)
		{
			SHApp.instance().shSendSimpleFeedback(title, message);
		}

		public string GetSHLibraryVersion()
		{
			return SHApp.instance().getSHLibraryVersion;
		}

		public string GetCurrentFormattedDateTime()
		{ 
			return SHApp.instance().getCurrentFormattedDateTime();
		}

		public string GetFormattedDateTime(long time)
		{
			return SHApp.instance().getFormattedDateTime(time);
		}

		public string GetInstallId()
		{ 
			return SHApp.instance().getInstallId;
		}

		public void SetAdvertisementId(string id)
		{ 
			SHApp.instance().advertisementId = id;
		}

		public string GetAdvertisementId()
		{
			return SHApp.instance().advertisementId;
		}

		public void RegisterForInstallEvent(OnInstallRegisteredCallback cb)
		{
			SHApp.instance().registerEventCallBack = new StreethawkIOS.Core.SHInstallRegisterHandler(cb);
		}

		public void RegisterForDeeplinkURL(RegisterForDeeplinkURLCallback cb)
		{
			SHApp.instance().shDeeplinking = delegate (NSUrl openUrl)
			   {
				   cb(openUrl.AbsoluteString);
			   };
		}

		public void SetsiTunesId(string iTunesId)
		{ 
			SHApp.instance().iTunesId = iTunesId;
		}

		public string GetsiTunesId()
		{ 
			return SHApp.instance().iTunesId;
		}

		public void DisplayBadge(int badgeCount)
		{
			Console.WriteLine("Android specific, not implemented in iOS.");
		}

		private NSDate convertDatetime(DateTime datetime)
		{
			if (datetime == DateTime.MinValue)
			{
				return null;
			}
			else
			{
				DateTime referenceDate = new DateTime(2001, 1, 1, 0, 0, 0);
				return NSDate.FromTimeIntervalSinceReferenceDate((datetime - referenceDate).TotalSeconds);
			}
		}
	}
}

