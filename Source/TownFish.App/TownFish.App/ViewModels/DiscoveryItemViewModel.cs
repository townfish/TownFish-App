using System;
using System.ComponentModel;
using Xamarin.Forms;


namespace TownFish.App.ViewModels
{
	public class DiscoveryItemViewModel : INotifyPropertyChanged
	{
		#region Methods

        static string FormatExpiresTimeStamp(DateTime? expires)
        {
            if (expires == null || !expires.HasValue)
                return "";

            var expiresIn = expires.Value.Subtract(DateTime.Now);

            string DaysAgo(int ago) => ago == 0 ? "Today" :
                    ago == 1 ? "Yesterday" : $"{ago} days ago";

            string Ago(DateTime date, int days) => days < 0 || days >= 7 ?
                    date.ToString("d MMM") :
                    $"{DaysAgo(days)} at {date.ToString("HH.mm")}";

            var daysToGo = (int)((expires.Value.Date - DateTime.Now.Date).TotalDays);

            if (expiresIn.TotalSeconds < 0)
            {
                return $"Expired {Ago(expires.Value, -daysToGo)}";
            }
            else
            {
                var expiresString = expiresIn.ToString("d'd 'h'h 'm'm 's's'");
                if (expiresString.StartsWith("0d "))
                    expiresString = expiresString.Substring(3);
                if (expiresString.StartsWith("0h "))
                    expiresString = expiresString.Substring(3);
                if (expiresString.StartsWith("0m "))
                    expiresString = expiresString.Substring(3);

                return "Ends in " + expiresString;
            }
        }

        static string FormatCreatedTimeStamp(DateTime? created)
        {
            if (created == null || !created.HasValue)
                return "";

            string DaysAgo(int ago) => ago == 0 ? "Today" :
                    ago == 1 ? "Yesterday" : $"{ago} days ago";

            string Ago(DateTime date, int days) => days < 0 || days >= 7 ?
                    date.ToString("d MMM") :
                    $"{DaysAgo(days)} at {date.ToString("HH.mm")}";

            var now = DateTime.Now;
            var daysAgo = (int)((now.Date - created.Value.Date).TotalDays);

            var createdString = Ago(created.Value, daysAgo);
            return $"{createdString}";
        }

        internal void RecalculateExpiry()
        {
            Device.BeginInvokeOnMainThread(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FormattedExpiresTime")));
        }

        #endregion Methods

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public string PictureUrl { get; set; }

		public string LinkUrl { get; set; }

		public string Title { get; set; }

		public string Text { get; set; }

		public DateTime? Created { get; set; }

		public DateTime? Modified { get; set; }

		public DateTime? Expires { get; set; }

		public string Group { get; set; }

		public ImageSource PictureSource => // URL may actually be URL or resource file name
				mPictureSource ?? (mPictureSource = string.IsNullOrEmpty (PictureUrl) ? null :
						PictureUrl.Contains ("//") ? PictureUrl :
						ImageSource.FromResource (PictureUrl));

        public string FormattedCreatedTime
        {
            get { return FormatCreatedTimeStamp(Modified); }
        }

        public string FormattedExpiresTime
        {
            get { return FormatExpiresTimeStamp(Expires); }
        }

        #endregion Properties

        #region Fields

        ImageSource mPictureSource;

        #endregion Fields
    }
}
