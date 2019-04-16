using System;
using System.Collections.Generic;

namespace TownFish.App.Models
{
	/// <summary>
	/// Callback for fetch feeds.
	/// </summary>
	public class FeedsModel
	{
		/// <summary>
		/// A unique identifier for the feed item.
		/// </summary>
		public string feed_id { get; set; }

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

