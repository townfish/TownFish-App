using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace TownFish.App.Models
{
	public class TownFishMenuMap
	{
		#region Methods

		/// <summary>
		/// Sets the menus' visibility.
		/// </summary>
		/// <param name="menuNames">The names.</param>
		public void SetMenuVisibility (string[] menuNames)
		{
			// if we don't have any, we can't make them visible!
			if (Menus == null || menuNames == null)
				return;

			foreach (var menuKvp in Menus)
				menuKvp.Value.Display = Array.IndexOf (menuNames, menuKvp.Key)> -1;
		}

		#endregion Methods

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

		public string SyncToken { get; set; }

		public string[] WhitelistUrls { get; set; }

		#endregion
	}

	public class UberWebViewMessage
	{
        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        [JsonProperty(PropertyName = "result")]
        public string Result { get; set;}

        public UberWebViewMessage()
        {

        }
    }
}
