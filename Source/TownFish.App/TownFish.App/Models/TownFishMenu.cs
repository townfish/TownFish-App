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

		public bool display { get; set; }

		public string position { get; set; }

		public List<TownFishMenuItem> items { get; set; }

		public int limitby { get; set; }

		#endregion
	}

	public class TownFishMenuList
	{
		#region Properties

		public TownFishMenu Top { get; set; }

		public TownFishMenu TopSub { get; set; }

		public TownFishMenu Bottom { get; set; }

		#endregion
	}
}
