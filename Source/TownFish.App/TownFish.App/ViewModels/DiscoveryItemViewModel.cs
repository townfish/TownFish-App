using System;

using Xamarin.Forms;


namespace TownFish.App.ViewModels
{
	public class DiscoveryItemViewModel
	{
		#region Methods

		static string FormatTimeStamp (DateTime created, DateTime expires)
		{
			string DaysAgo (int ago) => ago == 0 ? "Today" :
					ago == 1 ? "Yesterday" : $"{ago} days ago";

			string Ago (DateTime date, int days) => days < 0 || days >= 7 ?
					date.ToString ("d MMM") :
					$"{DaysAgo (days)} at {date.ToString ("HH.mm")}";

			string DaysToGo (int toGo) => toGo == 0 ? "Today" :
					toGo == 1 ? "Tomorrow" : $"in {toGo} days";

			string ToGo (DateTime date, int days) => days < 0 || days >= 7 ?
					date.ToString ("d MMM") :
					$"{DaysToGo (days)} at {date.ToString ("HH.mm")}";

			var now = DateTime.Now;
			var daysAgo = (int) ((now.Date - created.Date).TotalDays);
			var daysToGo = (int) ((expires.Date - now.Date).TotalDays);

			var createdString = Ago (created, daysAgo);
			var expiresString = (expires < now) ?
					$"Expired {Ago (expires, -daysToGo)}" :
					$"Expires {ToGo (expires, daysToGo)}";

			return $"{createdString} | {expiresString}";
		}

		#endregion Methods

		#region Properties

		public string PictureUrl { get; set; }

		public string LinkUrl { get; set; }

		public string Title { get; set; }

		public string Text { get; set; }

		public DateTime Created { get; set; }

		public DateTime Modified { get; set; }

		public DateTime Expires { get; set; }

		public string Group { get; set; }

		public ImageSource PictureSource => // URL may actually be URL or resource file name
				mPictureSource ?? (mPictureSource = string.IsNullOrEmpty (PictureUrl) ? null :
						PictureUrl.Contains ("//") ? PictureUrl :
						ImageSource.FromResource (PictureUrl));

		public string FormattedTimeStamp => mFormattedTimeStamp ??
				(mFormattedTimeStamp = FormatTimeStamp (Created, Expires));

		#endregion Properties

		#region Fields

		ImageSource mPictureSource;
		string mFormattedTimeStamp;

		#endregion Fields
	}
}
