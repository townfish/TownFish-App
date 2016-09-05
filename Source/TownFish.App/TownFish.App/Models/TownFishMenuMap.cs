using System;
using System.Collections.Generic;


namespace TownFish.App.Models
{
	public class TownFishMenuMap
	{
		#region Properties

		public string LocationsAPI { get; set; }

		public string LocationSetUrl { get; set; }

		public List<AvailableLocation> AvailableLocations { get; set; }

		public LocationIcons LocationIcons { get; set; }

		public TownFishMenuList Menus { get; set; }

		public bool DisplayStatusBar { get; set; }

		public string StatusBarBackgroundColor { get; set; }

		public string StatusBarTextColor { get; set; }

		public AvailableLocation CurrentLocation { get; set; }

		#endregion
	}

	public class UberWebViewMessage
	{
		public string Action { get; set; }

		public string Result { get; set;}
	}
}
