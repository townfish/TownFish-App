using System;
using System.Collections.Generic;

namespace StreetHawkCrossplatform
{
	/// <summary>
	/// Callback for fetch feeds.
	/// </summary>
	public delegate void RegisterForFeedCallback(List<SHFeedObject> arrayFeeds, string error);

	/// <summary>
	/// Callback when new feeds is available.
	/// </summary>
	public delegate void RegisterForNewFeedCallback();

	public interface IStreetHawkFeeds
	{
		/// <summary>
		/// Send acknowledgement for a feed item.
		/// </summary>
		/// <param name="feedid">Feed id.</param>
		void SendFeedAck(int feedid);

		/// <summary>
		/// Notifies user's action on feed item. 1 for accepted, 0 for later and -1 for decline.
		/// </summary>
		/// <param name="feedid">The feed id of result feed.</param>
		/// <param name="result">Result. 1 for accepted, 0 for postpone, -1 for declined.</param>
		void NotifyFeedResult(int feedid, int result);

		/// <summary>
		/// Notifies user's action on feed item. 
		/// </summary>
		/// <param name="feedid">The feed id of result feed.</param>
		/// <param name="stepid">The ID or label about a step.</param>
		/// <param name="feedresult">The result for accept, or postpone or decline. String must be accepted|postponed|rejected.</param>
		/// <param name="feedDelete">Set to <c>true</c> if feed items should be deleted from server for the given install. </param>
		/// <param name="completed">Set to <c>true</c> when tour complete.</param>
		void NotifyFeedResult(int feedid, string stepid, string feedresult, bool feedDelete, bool completed);

		/// <summary>
		/// Fetch feed data from StreetHawk server.
		/// </summary>
		/// <param name="offset">Offset.</param>
		/// <param name="cb">Callback for fetched feeds.</param>
		void ReadFeedData(int offset, RegisterForFeedCallback cb);

		/// <summary>
		/// Registers for new feeds is available in server.
		/// </summary>
		/// <param name="cb">Callback for new feeds available.</param>
		void OnNewFeedAvailableCallback(RegisterForNewFeedCallback cb);
	}

	public class SHFeedObject
	{
		/// <summary>
		/// A unique identifier for the feed item.
		/// </summary>
		public int feed_id { get; set; }

		/// <summary>
		/// Title of this feed.
		/// </summary>
		public string title { get; set; }

		/// <summary>
		/// Message of this feed.
		/// </summary>
		public string message { get; set; }

		/// <summary>
		/// Campaign this feed belongs to.
		/// </summary>
		public string campaign { get; set; }

		/// <summary>
		/// Json content.
		/// </summary>
		public string content { get; set; }

		/// <summary>
		/// A timestamp when the item activated.
		/// </summary>
		public string activates { get; set; }

		/// <summary>
		/// A timestamp when the item expires (it will not be visible to clients after). It's possible to be nil.
		/// </summary>
		public string expires { get; set; }

		/// <summary>
		/// A timestamp when the Feed item has been created.
		/// </summary>
		public string created { get; set; }

		/// <summary>
		/// A timestamp when the Feed item was modified the last time.
		/// </summary>
		public string modified { get; set; }

		/// <summary>
		/// A timestamp when the Feed item was deleted.
		/// </summary>
		public string deleted { get; set; }
	}
}

