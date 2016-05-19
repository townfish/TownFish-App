﻿using System;
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

		public TownFishMenuList Menus { get; set; }

		#endregion
	}
}
