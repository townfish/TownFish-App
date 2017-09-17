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
	#region Helper Types

	public class FeedException: Exception
	{
		public FeedException (string message) : base (message) {}
	}

	#endregion Helper Types

	public class StreetHawkFeeds : Java.Lang.Object, IStreetHawkFeeds, ISHFeedItemObserver
	{
		static Application mApplication => StreetHawkAnalytics.Application;

		const string FEED_ID = "feed_id";
        const string ID = "id";
		const string CAMPAIGN = "campaign";
		const string CONTENT = "content";
		const string ACTIVATES = "activates";
		const string EXPIRES = "expires";
		const string CREATED = "created";
		const string MODIFIED = "modified";
		const string DELETED = "deleted";

		// added 1.8.19
		const string ERROR = "error";
		const string RESULTS = "results";
        const string TITLE = "titleMsg";
		const string MESSAGE = "messageMsg";

        public void NotifyFeedResult(string feedid, int result)
		{
            try
            {
                var item = SHFeedItem.GetInstance(mApplication.ApplicationContext);
                item.NotifyFeedResult(feedid, result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
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

		static RegisterForFeedCallback mRegisterForFeedCallBack;
		public void ReadFeedData(int offset, RegisterForFeedCallback cb)
		{
			mRegisterForFeedCallBack = cb;
			SHFeedItem.GetInstance(mApplication.ApplicationContext).ReadFeedData(offset);
		}

		public void SendFeedAck(string feedid)
		{
            try
            {
                var item = SHFeedItem.GetInstance(mApplication.ApplicationContext);
                item.SendFeedAck(feedid);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

		public void ShFeedReceived (JSONObject feedResults)
		{
			if (mRegisterForFeedCallBack != null && feedResults != null)
			{
				var arrayFeeds = new List<SHFeedObject>();

				if (feedResults.Length() > 0)
				{
					if (feedResults.Has (ERROR))
						throw new FeedException (feedResults.GetString (ERROR));

					var feeds = feedResults.GetJSONArray (RESULTS);
					for (int i = 0; i < feeds.Length(); i++)
					{
						try
						{
							JSONObject jsonObj = feeds.GetJSONObject(i);
							SHFeedObject obj = new SHFeedObject();

							// we apparently don't know if the ID will be FEED_ID or ID, so check both
                            if (jsonObj.Has(FEED_ID))
                                obj.feed_id = jsonObj.GetString(FEED_ID);
                            else if (jsonObj.Has(ID))
                                obj.feed_id = jsonObj.GetString(ID);

							obj.campaign = jsonObj.GetString(CAMPAIGN);
							obj.content = jsonObj.GetString(CONTENT);
							obj.activates = jsonObj.GetString(ACTIVATES);
							obj.expires = jsonObj.GetString(EXPIRES);
							obj.created = jsonObj.GetString(CREATED);
							obj.modified = jsonObj.GetString(MODIFIED);
							obj.deleted = jsonObj.GetString(DELETED);

							// now extract the title and message from the content object
							var contentObj = jsonObj.GetJSONObject (CONTENT);
							obj.title = contentObj.GetString (TITLE);
							obj.message = contentObj.GetString (MESSAGE);
							
							arrayFeeds.Add(obj);
						}
						catch (JSONException e)
						{
							e.PrintStackTrace();

							throw;
						}
					}
				}

				mRegisterForFeedCallBack.Invoke (arrayFeeds, null);
			}
		}

		public void SHNotifyNewFeedItem()
		{
			// TODO: get implementation from StreetHawk
		}
	}
}
