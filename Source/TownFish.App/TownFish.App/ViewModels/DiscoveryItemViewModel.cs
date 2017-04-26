using System;

using Xamarin.Forms;


namespace TownFish.App.ViewModels
{
	public class DiscoveryItemViewModel
	{
		#region Methods

		static string FormatTimeStamp (DateTime created, DateTime expires)
		{
			string Ago (int ago) => ago == 0 ? "Today" :
					ago == 1 ? "Yesterday" : $"{ago} days ago";

			string ToGo (int toGo) => toGo == 0 ? "Today" :
					toGo == 1 ? "Tomorrow" : $"in {toGo} days";

			var dateNow = DateTime.Now.Date;
			var daysAgo = (int) ((dateNow - created.Date).TotalDays);
			var daysToGo = (int) ((expires.Date - dateNow).TotalDays);

			var agoString = daysAgo < 0 || daysAgo >= 7 ? created.ToString ("d MMM") :
					$"{Ago (daysAgo)} at {created.ToString ("HH.mm")}";

			var toGoString = daysToGo < 0 || daysToGo >= 7 ? expires.ToString ("d MMM") :
					$"{ToGo (daysToGo)} at {expires.ToString ("HH.mm")}";

			return $"{agoString} | Expires {toGoString}";
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
