using System;
using System.Collections.Generic;
using Foundation;
using Xamarin.Forms;

using StreetHawkCrossplatform;
using StreethawkIOS.Feed;

[assembly: Dependency(typeof(StreetHawkFeeds))]

namespace StreetHawkCrossplatform
{
	public class StreetHawkFeeds : IStreetHawkFeeds
	{
		public void SendFeedAck(int feedid)
		{
			SHFeed.instance().sendFeedAck(feedid);
		}

		public void NotifyFeedResult(int feedid, SHFeedResult result)
		{
			SHFeed.instance().sendLogForFeed(feedid, (StreethawkIOS.Feed.SHFeedResult)result);
		}

		public void ReadFeedData(int offset, RegisterForFeedCallback cb)
		{
			SHFeed.instance().feed(offset, delegate (NSArray arrayFeeds, NSError error)
			  {
				  if (cb != null)
				  {
					  if (error == null)
					  {
						  List<SHFeedObject> ret = new List<SHFeedObject>();
						  for (nuint i = 0; i < arrayFeeds.Count; i++)
						  {
							  StreethawkIOS.Feed.SHFeedObject feedObj = arrayFeeds.GetItem<StreethawkIOS.Feed.SHFeedObject>(i);
							  SHFeedObject feed = new SHFeedObject();
							  feed.feed_id = feedObj.feed_id;
							  feed.title = feedObj.title;
							  feed.message = feedObj.message;
							  feed.campaign = feedObj.campaign;
							  if (feedObj.content != null)
							  {
								  feed.content = feedObj.content.ToString();
							  }
							  feed.activates = convertDatetime(feedObj.activates);
							  feed.expires = convertDatetime(feedObj.expires);
							  feed.created = convertDatetime(feedObj.created);
							  feed.modified = convertDatetime(feedObj.modified);
							  feed.deleted = convertDatetime(feedObj.deleted);
							  ret.Add(feed);
						  }
						  cb(ret, null);
					  }
					  else
					  {
						  cb(null, error.LocalizedDescription);
					  }
				  }
			  });
		}

		public void OnNewFeedAvailableCallback(RegisterForNewFeedCallback cb)
		{
			SHFeed.instance().newFeedHandler = new StreethawkIOS.Feed.SHNewFeedsHandler(cb);
		}

		private DateTime convertDatetime(NSDate datetime)
		{
			if (datetime == null)
			{
				return DateTime.MinValue;
			}
			else
			{
				DateTime reference = new DateTime(2001, 1, 1, 0, 0, 0);
				return reference.AddSeconds(datetime.SecondsSinceReferenceDate);
			}
		}
	}
}

