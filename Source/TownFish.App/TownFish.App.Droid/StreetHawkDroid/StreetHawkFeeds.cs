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
        const string ID = "id";
        const string APS = "aps";
        const string D = "d";
        const string ALERT = "alert";
        const string TITLE = "title";
		const string MESSAGE = "message";
		const string CAMPAIGN = "campaign";
		const string CONTENT = "content";
		const string ACTIVATES = "activates";
		const string EXPIRES = "expires";
		const string CREATED = "created";
		const string MODIFIED = "modified";
		const string DELETED = "deleted";

        const char ALERT_SEPARATOR = ',';

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
                            if (jsonObj.Has(FEED_ID))
                                obj.feed_id = jsonObj.GetString(FEED_ID);
                            if (jsonObj.Has(ID))
                                obj.feed_id = jsonObj.GetString(ID);
                            if (jsonObj.Has(TITLE))
                                obj.title = jsonObj.GetString(TITLE);
                            if (jsonObj.Has(MESSAGE))
                                obj.message = jsonObj.GetString(MESSAGE);
                            if (jsonObj.Has(CAMPAIGN))
                                obj.campaign = jsonObj.GetString(CAMPAIGN);
                            if (jsonObj.Has(CONTENT))
                                obj.content = jsonObj.GetString(CONTENT);
                            if (jsonObj.Has(ACTIVATES))
                                obj.activates = jsonObj.GetString(ACTIVATES);
                            if (jsonObj.Has(EXPIRES))
                                obj.expires = jsonObj.GetString(EXPIRES);
                            if (jsonObj.Has(CREATED))
                                obj.created = jsonObj.GetString(CREATED);
                            if (jsonObj.Has(MODIFIED))
                                obj.modified = jsonObj.GetString(MODIFIED);
                            if (jsonObj.Has(DELETED))
                                obj.deleted = jsonObj.GetString(DELETED);

                            var content = jsonObj.GetJSONObject(CONTENT);

                            if (obj.title == null && obj.message == null && content.Has(APS))
                            {
                                obj.content = content.GetString(D);

                                var alert = content.GetJSONObject(APS).GetString(ALERT);

                                if (alert != null)
                                {
                                    var alertParts = alert.Split(ALERT_SEPARATOR);
                                    obj.title = alertParts[0].Trim();
                                    if (alertParts.Length == 2)
                                    {
                                        obj.message = alertParts[1].Trim();
                                    }
                                }
                            }
                            else
                            {
                                obj.content = jsonObj.GetString(CONTENT);
                            }

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
