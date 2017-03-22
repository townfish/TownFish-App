using System;
using System.Collections.Generic;

using Android.App;
using Android.Util;

using Org.Json;

using Com.Streethawk.Library.Feeds;
using StreetHawkCrossplatform;


[assembly: Xamarin.Forms.Dependency(typeof(StreetHawkFeeds))]


namespace StreetHawkCrossplatform
{
	public class StreetHawkFeeds : Java.Lang.Object, IStreetHawkFeeds, ISHFeedItemObserver
	{
		static Application mApplication => StreetHawkAnalytics.Application;

		const string FEED_ID = "feed_id";
		const string TITLE = "title";
		const string MESSAGE = "message";
		const string CAMPAIGN = "campaign";
		const string CONTENT = "content";
		const string ACTIVATES = "activates";
		const string EXPIRES = "expires";
		const string CREATED = "created";
		const string MODIFIED = "modified";
		const string DELETED = "deleted";

		public void NotifyFeedResult(string feedid, int result)
		{
			SHFeedItem.GetInstance(mApplication.ApplicationContext).NotifyFeedResult(feedid,result);
		}

		public void NotifyFeedResult(string feedid, string stepid, string feedresult, bool feedDelete, bool completed)
		{
			//TODO: not implement yet
		}

		RegisterForNewFeedCallback mNewFeedCallbak;
		public void OnNewFeedAvailableCallback(RegisterForNewFeedCallback cb)
		{
			//TODO : implement broadcast receiver in native to receive feed notification and notifies server
			Log.Error("StreetHawk", "OnNewFeedAvailableCallback is not available for this release");
		}

		RegisterForFeedCallback mRegisterForFeedCallBack;
		public void ReadFeedData(int offset, RegisterForFeedCallback cb)
		{
			mRegisterForFeedCallBack = cb;
			SHFeedItem.GetInstance(mApplication.ApplicationContext).ReadFeedData(offset);
		}

		public void SendFeedAck(string feedid)
		{
			SHFeedItem.GetInstance(mApplication.ApplicationContext).SendFeedAck(feedid);
		}

		public void ShFeedReceived(JSONArray feeds)
		{
			if(null!=mRegisterForFeedCallBack){
				if (null != feeds)
				{
					List<SHFeedObject> arrayFeeds = new List<SHFeedObject>();
					for (int i = 0; i < feeds.Length(); i++)
					{
						try
						{
							JSONObject jsonObj = feeds.GetJSONObject(i);
							SHFeedObject obj = new SHFeedObject();
							obj.feed_id = jsonObj.GetString(FEED_ID);
							obj.title = jsonObj.GetString(TITLE);
							obj.message = jsonObj.GetString(MESSAGE);
							obj.campaign = jsonObj.GetString(CAMPAIGN);
							obj.content = jsonObj.GetString(CONTENT);
							obj.activates = jsonObj.GetString(ACTIVATES);
							obj.expires = jsonObj.GetString(EXPIRES);
							obj.created = jsonObj.GetString(CREATED);
							obj.modified = jsonObj.GetString(MODIFIED);
							obj.deleted = jsonObj.GetString(DELETED);
							arrayFeeds.Add(obj);

						}
						catch (JSONException e)
						{
							e.PrintStackTrace();
						}
					}
					mRegisterForFeedCallBack.Invoke(arrayFeeds,null);
				}
			}
		}

		public void SHNotifyNewFeedItem()
		{
			// TODO: get implementation from StreetHawk
		}
	}
}
