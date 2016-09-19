using System;

namespace StreetHawkCrossplatform
{
	/// <summary>
	/// Register for callback to receive share url from StreetHawk which can be shared with friends.
	/// </summary>
	public delegate void RegisterForShareURLCallback(string url, string error);

	/// <summary>
	/// Register for deeplink url for differed deeplinking
	/// </summary>
	public delegate void RegisterForDeepLinkCallback(String url);

	public interface IStreetHawkGrowth
	{
		/// <summary>
		/// Gets the share URL for app download.
		/// </summary>
		/// <param name="utm_campaign">Utm campaign.</param>
		/// <param name="share_url">Share URL.</param>
		/// <param name="default_url">Default URL.</param>
		void GetShareUrlForAppDownload(string utm_campaign, string share_url, string default_url);

		/// <summary>
		/// Gets the share URL for app download.
		/// </summary>
		/// <param name="utm_campaign">Utm campaign.</param>
		/// <param name="share_url">Deeplink URI.</param>
		/// <param name="utm_source">Utm source.</param>
		/// <param name="utm_medium">Utm medium.</param>
		/// <param name="utm_term">Utm term.</param>
		/// <param name="campaign_content">Campaign content.</param>
		/// <param name="default_url">Default URL.</param>
		/// <param name="cb">Callback for the share URL for app download.</param>
		void GetShareUrlForAppDownload(string utm_campaign, string share_url, string utm_source, string utm_medium, string utm_term, string campaign_content, string default_url, RegisterForShareURLCallback cb);

		void RegisterForDeepLinkURLCallback(RegisterForDeepLinkCallback cb);
	
	
	}
}

