using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TownFish.App.Models
{
	public class TownFishMenuMap
	{
		#region Properties

		public string LocationsAPI { get; set; }

		public string LocationSetUrl { get; set; }

		public List<AvailableLocation> AvailableLocations { get; set; }

		public string ActiveLocation { get; set; }

		public LocationIcons LocationIcons { get; set; }

		public TownFishMenuList Menus { get; set; }

		public bool displayStatsBar { get; set; }

		public string statusBarBackgroundColor { get; set; }

		public string statusBarTextColor { get; set; }

		public AvailableLocation currentLocation { get; set; }

		#endregion
	}

	public class TownFishTopLevelMenu
	{
		public string Key { get; set; }

		public string Value { get; set;}
	}
}
