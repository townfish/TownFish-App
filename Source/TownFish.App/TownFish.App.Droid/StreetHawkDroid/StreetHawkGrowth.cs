using System;
using StreetHawkCrossplatform;
using Com.Streethawk.Library.Growth;
using Android.App;
using Android.Util;
using Org.Json;

using XamHawkDemo.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(StreetHawkGrowth))]
namespace XamHawkDemo.Droid
{
	public class StreetHawkGrowth :IStreetHawkGrowth,IGrowth
	{
		private Activity mActivity;

		public StreetHawkGrowth() { }

		public StreetHawkGrowth(Activity activity)
		{
			mActivity = activity;
		}

		public IntPtr Handle
		{
			get;
			set;
		}

		public void Dispose()
		{
			
		}

		public void GetShareUrlForAppDownload(string utm_campaign, string share_url, string default_url)
		{
			Growth.GetInstance(mActivity).OriginateShareWithCampaign(utm_campaign,share_url,null);
		}

		private static RegisterForShareURLCallback mGrowthCallback;
		public void GetShareUrlForAppDownload(string utm_campaign, string share_url, string utm_source, string utm_medium, string utm_term, string campaign_content, string default_url, RegisterForShareURLCallback cb)
		{
			if (null == cb)
			{
				Growth.GetInstance(mActivity).GetShareUrlForAppDownload(utm_campaign, share_url, utm_source, utm_medium, utm_term, campaign_content, default_url, null);
			}
			else {
				mGrowthCallback = cb;
				Growth.GetInstance(mActivity).GetShareUrlForAppDownload(utm_campaign, share_url, utm_source, utm_medium, utm_term, campaign_content, default_url,this);
			}
		}

		private static RegisterForDeepLinkCallback mDeepLinkCb;
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

