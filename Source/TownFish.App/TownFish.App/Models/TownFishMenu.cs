using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TownFish.App.Models
{
	public class TownFishMenu
	{
		#region Properties

		public bool Display { get; set; }

		public string Position { get; set; }

		public List<TownFishMenuItem> Items { get; set; }

		public int Limitby { get; set; }

		public bool IsVisible => Display && Items?.Count > 0;

		#endregion Properties
	}

	public class TownFishMenuList: Dictionary<string, TownFishMenu>
	{
		#region Properties

		public TownFishMenu Top => (from kvp in this
									where kvp.Value.Display &&
											kvp.Value.Position == "top" &&
											!kvp.Key.EndsWith ("form")
									select kvp.Value).FirstOrDefault();

		public TownFishMenu TopSub => (from kvp in this
									   where kvp.Value.Display &&
											kvp.Value.Position == "topsub"
									   select kvp.Value).FirstOrDefault();

		public TownFishMenu TopForm => (from kvp in this
										where kvp.Value.Display &&
												kvp.Value.Position == "top" &&
												kvp.Key.EndsWith ("form")
										select kvp.Value).FirstOrDefault();

		public TownFishMenu Bottom => (from m in Values
									   where m.Position == "bottom"
									   select m).FirstOrDefault();

		#endregion Properties
	}
}
