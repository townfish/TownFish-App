using System;
using StreetHawkCrossplatform;
using Android.App;
using Com.Streethawk.Library.Core;
using Android.Util;

[assembly: Xamarin.Forms.Dependency(typeof(StreetHawkAnalytics))]
namespace StreetHawkCrossplatform
{
	public class StreetHawkAnalytics : IStreetHawkAnalytics,ISHEventObserver
	{

		private static Application mApplication;
		private static bool mIsInitialised = false;

		public StreetHawkAnalytics() { }

		public StreetHawkAnalytics(Application app)
		{
			mApplication = app;
			Log.Error("Anurag","Application "+mApplication);
		}

		public void DisplayBadge(int badgeCount)
		{
			Log.Error("StreetHawk", "Function DisplayBadge is not implemented for Android");
		}

		public string GetAdvertisementId()
		{
			Log.Error("StreetHawk","Function GetAdvertisementId is not implemented for Android");
			return null;
		}

		public string GetAppKey()
		{
			Log.Error("Anurag","inside getApp[Key "+mApplication);
			return StreetHawk.Instance.GetAppKey(mApplication.ApplicationContext);
		}

		public string GetCurrentFormattedDateTime()
		{
			Log.Error("StreetHawk", "Function GetCurrentFormattedDateTime is not implemented for Android");
			return null;
		}

		public bool GetEnableLogs()
		{
			Log.Error("StreetHawk", "Function GetEnableLogs is not implemented for Android");
			return false;
		}

		public string GetFormattedDateTime(long time)
		{
			return StreetHawk.GetFormattedDateTime(time);
		}

		public string GetInstallId()
		{
			return StreetHawk.Instance.GetInstallId(mApplication.ApplicationContext);
		}

		public string GetSHLibraryVersion()
		{
			//TODO return from library
			return "1.8.2";
		}
		public string GetsiTunesId()
		{
			Log.Error("StreetHawk", "Function GetsiTunesId is not implemented for Android");
			return null;
		}

		public void IncrementTag(string key)
		{
			StreetHawk.Instance.IncrementTag(key);
		}

		public void IncrementTag(string key, int incrValue)
		{
			StreetHawk.Instance.IncrementTag(key,incrValue);
		}

		public void Init()
		{
			mIsInitialised = true;
			StreetHawk.Instance.Init(mApplication);
		}

		public void NotifyViewEnter(string viewName)
		{
			if (mIsInitialised)
				StreetHawk.Instance.NotifyViewEnter(viewName);
		}

		public void NotifyViewExit(string viewName)
		{
			if(mIsInitialised)
				StreetHawk.Instance.NotifyViewExit(viewName);
		}

		public void RegisterForDeeplinkURL(RegisterForDeeplinkURLCallback cb)
		{
			Log.Error("StreetHawk", "Function RegisterForDeeplinkURL is not implemented for Android");
		}
		private static OnInstallRegisteredCallback mInstallRegisterCallback;

		public IntPtr Handle
		{
			get;
			set;
		}

		public void RegisterForInstallEvent(OnInstallRegisteredCallback cb)
		{
			mInstallRegisterCallback = cb;
		}

		public void RemoveTag(string key)
		{
			StreetHawk.Instance.RemoveTag(key);
		}

		public void SendSimpleFeedback(string title, string message)
		{
			StreetHawk.Instance.SendSimpleFeedback(title,message);
		}

		public void SetAdvertisementId(string id)
		{
			StreetHawk.Instance.SetAdvertisementId(mApplication.ApplicationContext,id);
		}

		public void SetAppKey(string appKey)
		{
			StreetHawk.Instance.SetAppKey(appKey);
		}

		public void SetEnableLogs(bool isEnable)
		{
			Util.SetSHDebugFlag(mApplication.ApplicationContext,isEnable);
		}

		public void SetsiTunesId(string iTunesId)
		{
			Log.Error("StreetHawk", "Function SetsiTunesId is not implemented for Android");
			return;
		}

		public void TagCuid(string cuid)
		{
			StreetHawk.Instance.TagCuid(cuid);
		}

		public void TagDateTime(string key, DateTime value)
		{
			StreetHawk.Instance.TagDatetime(key,value.ToString());
		}

		public void TagNumeric(string key, double value)
		{
			StreetHawk.Instance.TagNumeric(key,value);
		}

		public void TagString(string key, string value)
		{
			StreetHawk.Instance.TagString(key,value);
		}

		public void TagUserLanguage(string language)
		{
			StreetHawk.Instance.TagUserLanguage(language);
		}

		public void OnInstallRegistered(string installId)
		{
			if (null != mInstallRegisterCallback)
			{
				mInstallRegisterCallback.Invoke(installId);
			}
		}

		public void Dispose()
		{
			
		}
	}
}