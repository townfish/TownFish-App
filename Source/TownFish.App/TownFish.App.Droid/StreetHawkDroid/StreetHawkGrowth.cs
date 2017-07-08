using System;

using Android.App;
using Android.Util;

using Org.Json;

using Com.Streethawk.Library.Growth;
using StreetHawkCrossplatform;


[assembly: Xamarin.Forms.Dependency(typeof(StreetHawkGrowth))]


namespace StreetHawkCrossplatform
{
	public class StreetHawkGrowth : Java.Lang.Object, IStreetHawkGrowth, IGrowth
	{
		static Activity mActivity => StreetHawkAnalytics.MainActivity;

		public void GetShareUrlForAppDownload(string utm_campaign, string share_url, string default_url)
		{
			Growth.GetInstance(mActivity).OriginateShareWithCampaign(utm_campaign,share_url,null);
		}

		static RegisterForShareURLCallback mGrowthCallback;
		public void GetShareUrlForAppDownload(string utm_campaign, string share_url, string utm_source, string utm_medium, string utm_term, string campaign_content, string default_url, RegisterForShareURLCallback cb)
		{
			if (null == cb)
			{
				Growth.GetInstance(mActivity).GetShareUrlForAppDownload(utm_campaign, share_url, utm_source, utm_medium, utm_term, campaign_content, default_url, null);
			}
			else
			{
				mGrowthCallback = cb;
				Growth.GetInstance(mActivity).GetShareUrlForAppDownload(utm_campaign, share_url, utm_source, utm_medium, utm_term, campaign_content, default_url,this);
			}
		}

		static RegisterForDeepLinkCallback mDeepLinkCb;
		public void RegisterForDeepLinkURLCallback(RegisterForDeepLinkCallback cb)
		{
			mDeepLinkCb = cb;
		}

		public void OnReceiveDeepLinkUrl(string shareUrl)
		{
			if (null != mDeepLinkCb)
			{
				mDeepLinkCb.Invoke(shareUrl);
			}
		}

		public void OnReceiveErrorForShareUrl(JSONObject error)
		{
			if (null != mGrowthCallback)
			{
				mGrowthCallback.Invoke(null, error.ToString());
			}
		}

		public void OnReceiveShareUrl(string shareUrl)
		{
			if (null != mGrowthCallback)
			{
				mGrowthCallback.Invoke(shareUrl, null);
			}
		}
	}
}
