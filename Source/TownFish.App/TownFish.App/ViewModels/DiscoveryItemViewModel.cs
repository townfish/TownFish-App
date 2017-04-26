using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;


namespace TownFish.App.ViewModels
{
	public class FeedItemViewModel
	{
		#region Methods

		static string FormatTimeStamp (DateTime ts)
		{
			var daysAgo = (int) ((DateTime.Now.Date - ts.Date).TotalDays);
			if (daysAgo < 0 || daysAgo >= 7)
				return ts.ToString ("d MMM HH.mm");
			else
				return daysAgo == 0 ? "Today" : daysAgo == 1 ? "Yesterday" :
					$"{daysAgo} days ago";
		}

		#endregion Methods

		#region Properties

		public string PictureUrl { get; set; }

		public string LinkUrl { get; set; }

		public string Title { get; set; }

		public string Text { get; set; }

		public DateTime TimeStamp { get; set; }

		public string Group { get; set; }

		public ImageSource PictureSource => // URL may actually be URL or resource file name
				mPictureSource ?? (mPictureSource = string.IsNullOrEmpty (PictureUrl) ? null :
						PictureUrl.Contains ("//") ?
						(ImageSource) PictureUrl : ImageSource.FromResource (PictureUrl));

		public string FormattedTimeStamp =>
				mFormattedTimeStamp ?? (mFormattedTimeStamp = FormatTimeStamp (TimeStamp));

		#endregion Properties

		#region Fields

		ImageSource mPictureSource;
		string mFormattedTimeStamp;

		#endregion Fields
	}
}
