using System;

using Xamarin.Forms;


namespace TownFish.App
{
	public partial class FeedItem: Grid
	{
		#region Construction

		public FeedItem()
		{
			InitializeComponent();
		}

		#endregion Construction

		#region Properties

		public Image Image => imgIcon;

		public ImageSource ImageSource => imgIcon.Source;

		public Color TitleColor => lblTitle.TextColor;

		public Color TextColor => lblText.TextColor;

		public Color TimeStampColor => lblTimeStamp.TextColor;

		public Color GroupColor => lblGroup.TextColor;

		#endregion Properties
	}
}
