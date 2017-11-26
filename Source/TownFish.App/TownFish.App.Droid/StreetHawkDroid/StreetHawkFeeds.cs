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
		const string CAMPAIGN = "campaignid";
		const string CONTENT = "content";
        const string CONTENT_JSON = "d";
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

		//RegisterForNewFeedCallback mNewFeedCallbak;
		public void OnNewFeedAvailableCallback(RegisterForNewFeedCallback cb)
		{
			//TODO : implement broadcast receiver in native to receive feed notification and notifies server
			Log.Error("StreetHawk", "OnNewFeedAvailableCallback is not available for this release");
		}

		static RegisterForFeedCallback mRegisterForFeedCallBack;
		public void ReadFeedData(int offset, RegisterForFeedCallback cb)
		{
			mRegisterForFeedCallBack = cb;

            // Set offset to 0, to get ALL Feeds 
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


		/*
         * Typical ShFeedReceived JSON Format 
         * {  
           "count":2,
           "next":null,
           "previous":null,
           "results":[  
              {  
                 "installid":"16b1055c-af42-470f-8f33-74502422c367",
                 "num_feeds":36,
                 "campaignid":"4308f434-a531-4850-a323-810542b07ddf",
                 "expires":"2017-09-14T22:53:15Z",
                 "modified":"2017-08-25T03:47:16.201000Z",
                 "app_key":"SHSample_bison",
                 "created":"2017-08-25T03:47:16.193000Z",
                 "content":{  
                    "aps":{  
                       "sound":"default",
                       "category":"8049",
                       "badge":1,
                       "alert":"FREE DRINKS! Come get your free Drinks "
                          },
                    "c":8049,
           "l":3,
             "titleMsg":"FREE DRINKS!",
             "messageMsg":"Come get your free Drinks"
            },
                 "id":"4b5169cf-849f-4141-b6c5-aab34d419367"
            },
         * 
         * 
         * 
         * 
        */
		public void ShFeedReceived (JSONObject feedResults)
		{
			if (mRegisterForFeedCallBack == null)
				return;

			string errorMsg = null;
			var arrayFeeds = new List<SHFeedObject>();

			try
			{
				if ((feedResults?.Length() ?? 0) > 0)
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

                            // Campaign id
							obj.campaign = jsonObj.GetString(CAMPAIGN);

                            // expires time in ISO format (May not expire if schedule is forever)
                            if (jsonObj.Has(EXPIRES))
                                obj.expires = jsonObj.GetString(EXPIRES);
                            // Can put this in, but do not exit loop if not exist 

                            // created time in ISO format 
                            obj.created = jsonObj.GetString(CREATED);

                            // modified time in ISO format 
							obj.modified = jsonObj.GetString(MODIFIED);

							//obj.deleted = jsonObj.GetString(DELETED);

							// now extract the title and message from the content object
							var contentObj = jsonObj.GetJSONObject (CONTENT);
							obj.title = contentObj.GetString (TITLE);
							obj.message = contentObj.GetString (MESSAGE);
                            obj.content = contentObj.GetString(CONTENT_JSON);

                            arrayFeeds.Add(obj);
						}
						catch (JSONException e)
						{
                            e.PrintStackTrace();

							throw;
						}
					}
				}
			}
			catch (Exception ex)
			{
				errorMsg = ex.Message;
			}

			mRegisterForFeedCallBack.Invoke (arrayFeeds, errorMsg);
		}

		public void SHNotifyNewFeedItem()
		{
			// TODO: get implementation from StreetHawk
		}
	}
}
