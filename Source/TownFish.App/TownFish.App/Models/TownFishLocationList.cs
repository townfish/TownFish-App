using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TownFish.App.Models
{
	public class TownFishLocationList
	{
		public List<TownfishLocationItem> Items { get; set; }
	}

	public class TownfishLocationItem
	{
		public string CityID { get; set; }

		public string Name { get; set; }

		public string CountyID { get; set; }

		public string Type { get; set; }

		public string Slug { get; set; }

		public string PostCode { get; set; }

		public string CountryID { get; set; }

		public string CountryName { get; set; }

		public string LocationText
		{
			get { return Name + ", " + PostCode;  }
		}
	}
}
