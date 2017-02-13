using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;


namespace TownFish.App.Models
{
	public class FeedItemViewModel
	{
		public string PictureUrl { get; set; }

		public string LinkUrl { get; set; }

		public string Title { get; set; }

		public string Text { get; set; }

		public DateTime TimeStamp { get; set; }

		public string Group { get; set; }

		public ImageSource PictureSource => // URL may actually be URL or resource file name
				mPictureSource ?? (mPictureSource = PictureUrl.Contains ("//") ?
						(ImageSource) PictureUrl : ImageSource.FromResource (PictureUrl));

		public string FormattedTimeStamp =>
				mFormattedTimeStamp ?? (mFormattedTimeStamp = TimeStamp.ToString ("d MMM HH.mm"));

		#region Fields

		ImageSource mPictureSource;
		string mFormattedTimeStamp;

		#endregion Fields
	}
}
