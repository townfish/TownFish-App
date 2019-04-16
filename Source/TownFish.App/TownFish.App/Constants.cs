using System;
namespace TownFish.App
{
    public class Constants
    {
        // all magic URLs and paths used in this app
        public const string SiteDomain = "dev.townfish.com";
        public const string BaseUrl = "https://" + SiteDomain;
        public const string StartPath = "/";
        public const string TermsUrl = BaseUrl + "/terms-of-use/";

        public const string NotificationsSyncUrl = BaseUrl + "/profile/pushreceived/{0}/{1}";
        public const string SHSyncUrl = BaseUrl + "/profile/appsync/{0}/{1}";
        public const string EditProfileUrl = BaseUrl + "/profile/edit/generalinfo";
        public const string EditLikesUrl = BaseUrl + "/profile/edit/likes";
        public const string ShowFeedUrl = BaseUrl + "/profile/showfeed";

        public const string TwitterApiDomain = "api.twitter.com";

        public const string GcmSenderID = "7712235891";

        public const string OneSignalKey = "9c6ce461-bedc-43f8-86af-9f8ac4e251b1";


        public const string StreetHawkAppKey = "TownFish";

        public const string cLastDiscoveriesViewTimeKey = "LastDiscoveriesViewTime";

        public const string ACCEPTED = "accepted";

        public const string cCode = "Code";
        public const string cSHCuid = "shcuid";
        public const string cSynced = "synced";
        public const int cSHSyncRetries = 3;
    }
}
