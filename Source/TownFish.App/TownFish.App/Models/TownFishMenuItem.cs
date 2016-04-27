using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TownFish.App.Models
{
	public class TownFishMenuItem
	{
		#region Properties

		public string type { get; set; }
		public string kind { get; set; }
		public string value { get; set; }
		public string iconurl { get; set; }
		public string size { get; set; }
		public string color { get; set; }
		public string href { get; set; }
		public string align { get; set; }
		public bool highlight { get; set; }

		#endregion
	}
}
