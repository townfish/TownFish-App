using System;
using StreetHawkCrossplatform;
using Com.Streethawk.Library.Feeds;
using Android.App;
using Org.Json;
using System.Collections.Generic;
using Android.Util;

using XamHawkDemo.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(StreetHawkFeeds))]
namespace XamHawkDemo.Droid
                     
{
	public class StreetHawkFeeds : IStreetHawkFeeds,ISHFeedItemObserver
	{
		private static Application mApplication;

		private const string FEED_ID = "feed_id";

		private const string TITLE = "title";

		private const string MESSAGE = "message";

		private const string CAMPAIGN = "campaign";

		private const string CONTENT = "content";

		private const string ACTIVATES = "activates";

		private const string EXPIRES = "expires";

		private const string CREATED = "created";

		private const string MODIFIED = "modified";

		private const string DELETED = "deleted";


		public StreetHawkFeeds() { }

		public StreetHawkFeeds(Application application)
		{
			mApplication = application;
		}

		public void NotifyFeedResult(int feedid, int result)
		{
			SHFeedItem.GetInstance(mApplication.ApplicationContext).NotifyFeedResult(feedid,result);
		}

		private static RegisterForNewFeedCallback mNewFeedCallbak;

		public IntPtr Handle
		{
			get;
			set;
		}

		public void OnNewFeedAvailableCallback(RegisterForNewFeedCallback cb)
		{
			//TODO : implement broadcast receiver in native to receive feed notification and notifys server
			Log.Error("StreetHawk", "OnNewFeedAvailableCallback is not available for this relase");

		}

		private static RegisterForFeedCallback mRegisterForFeedCallBack;
		public void ReadFeedData(int offset, RegisterForFeedCallback cb)
		{
			mRegisterForFeedCallBack = cb;
			SHFeedItem.GetInstance(mApplication.ApplicationContext).ReadFeedData(offset);
		}

		public void SendFeedAck(int feedid)
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
							obj.feed_id = jsonObj.GetInt(FEED_ID);
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
		public void Dispose()
		{
			
		}
	}
}