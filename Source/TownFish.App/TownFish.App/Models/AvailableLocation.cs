using System;

using Xamarin.Forms;

using TownFish.App.ViewModels;


namespace TownFish.App.Models
{
	public class AvailableLocation : ViewModelBase
	{		
		#region Properties

		public string id { get; set; }

		public string name { get; set; }

		public string url { get; set; }

		public bool IsSelected { get; set; }

		public bool IsLocked { get; set; }

		public string LeftImage
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		public string RightImage
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		public Color Colour
		{
			get { return Get<Color>(); }
			set {  Set (value); }
		}

		#endregion Properties
	}

	public class LocationIcons
	{
		#region Properties

		public string Lock { get; set; }

		public string Pin { get; set; }

		public string Tick { get; set; }

		public string Info { get; set; }

		#endregion Properties
	}
}
