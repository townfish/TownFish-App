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
		/// <returns>The feed result.</returns>
		/// <param name="feedid">Feed id.</param>
		/// <param name="result">Result.</param>
		void NotifyFeedResult(int feedid, int result);

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

	/*
	/// <summary>
	/// Result when click positive button such as "Agree", "Yes Please".
	/// </summary>
	public int SHFeedResult_Accept = 1;
	/// <summary>
	/// Result when click neutral button such as "Later", "Not now".
	/// </summary>
	public int SHFeedResult_Postpone = 0;
	/// <summary>
	/// Result when click negative button such as "Never", "Cancel".
	/// </summary>
	public int SHFeedResult_Decline = -1;
	*/

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

