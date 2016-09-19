using System;
using Foundation;
using Xamarin.Forms;

using StreetHawkCrossplatform;
using StreethawkIOS.Growth;

[assembly: Dependency(typeof(StreetHawkGrowth))]

namespace StreetHawkCrossplatform
{
	public class StreetHawkGrowth : IStreetHawkGrowth
	{
		public void GetShareUrlForAppDownload(string utm_campaign, string share_url, string default_url)
		{
			SHGrowth.instance().originateShareWithSourceSelection(utm_campaign, share_url, default_url);
		}

		public void GetShareUrlForAppDownload(string utm_campaign, string share_url, string utm_source, string utm_medium, string utm_term, string campaign_content, string default_url, RegisterForShareURLCallback cb)
		{ 
			SHGrowth.instance().originateShareWithCampaign(utm_campaign, utm_source, utm_medium, campaign_content, utm_term, share_url, default_url, delegate (NSObject result, NSError error)
			  {
				  if (cb != null)
				  {
					  string shareUrl = result.ToString();
					  string errorMessage = (error != null) ? error.LocalizedDescription : null;
					  cb(shareUrl, errorMessage);
				  }
			  });
		}

		public void RegisterForDeepLinkURLCallback(RegisterForDeepLinkCallback cb)
		{
			throw new NotImplementedException();
		}
	}
}

