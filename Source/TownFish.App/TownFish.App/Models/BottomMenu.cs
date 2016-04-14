using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TownFish.App.Models
{
	public class MenuMap
	{
		#region Properties

		public List<NavMenuItem> BottomMenuItems { get; set; }

		public string TopPrimaryButtonText { get; set; }

		public List<NavMenuItem> TopSecondaryMenuItems { get; set; }

		#endregion
	}
}
