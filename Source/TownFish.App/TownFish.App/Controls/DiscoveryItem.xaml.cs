using System;

using Xamarin.Forms;

//using TownFish.App.Helpers;


namespace TownFish.App.Controls
{
	public partial class DiscoveryItem: Grid //StackLayout
	{
		#region Construction

		public DiscoveryItem()
		{
			InitializeComponent();

			// HACK: iOS doesn't handle ListView item height changing (such as when image loads
			// in background, its height starts at 0 and ends up at the image height), so if
			// we're in a ViewCell we need to force its size to be updated after image loaded

			//imgPicture.On<Xamarin.Forms.PlatformConfiguration.Android>().

			//Device.OnPlatform (iOS: () =>
			//{
			//	imgPicture.PropertyChanged += (s, pcea) =>
			//		{
			//			if (pcea.PropertyName == "Source" && imgPicture.Source != null)
			//			{
			//				ImageSourceWatcher.SetImageLoadedHandler (imgPicture.Source,
			//					(s2, loaded) =>
			//					{
			//						if (loaded)
			//						{
			//							var itemCell = imgPicture.Parent?.Parent as ViewCell;
			//							itemCell?.ForceUpdateSize();
			//						}
			//					});
			//			}
			//		};
			//});
		}

		#endregion Construction

		#region Methods

		public void OnTapped (object sender, EventArgs e)
		{
			Tapped?.Invoke (this, EventArgs.Empty);
		}

		#endregion Methods

		#region Properties

		public event EventHandler Tapped;

		public Image Image => imgPicture;

		public ImageSource ImageSource => imgPicture.Source;

		public Color TitleColor => lblTitle.TextColor;

		public Color TextColor => lblText.TextColor;

		public Color TimeStampColor => lblTimeStamp.TextColor;

		//public Color GroupColor => lblGroup.TextColor;

		#endregion Properties
	}
}
