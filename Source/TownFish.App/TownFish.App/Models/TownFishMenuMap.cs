using System;
using System.Collections.Generic;


namespace TownFish.App.Models
{
	public class TownFishMenuMap
	{
		#region Methods

		/// <summary>
		/// Sets the menus' visibility.
		/// </summary>
		/// <remarks>
		/// I can honestly say this method has the most null checks I have ever in my
		/// entire life had to put into a single method.
		/// </remarks>
		/// <param name="names">The names.</param>
		public void SetMenuVisibility (string[] names)
		{
			if (Menus == null)
				return;

			// only menus in names should be visible, so hide all to start with
			if (Menus.Top != null)
				Menus.Top.display = false;

			if (Menus.TopSub != null)
				Menus.TopSub.display = false;

			if (Menus.TopForm != null)
				Menus.TopForm.display = false;

			// TODO: implement
			//if (Menus.TopSlide != null)
			//	Menus.TopSlide.display = false;

			if (Menus.Bottom != null)
				Menus.Bottom.display = false;

			if (names == null || names.Length == 0)
				return;

			foreach (var item in names)
			{
				var menu = item.ToLower();

				if (menu == "top" && Menus.Top != null)
					Menus.Top.display = true;

				if (menu == "topsub" && Menus.TopSub != null)
					Menus.TopSub.display = true;

				if (menu == "topform" && Menus.TopForm != null)
					Menus.TopForm.display = true;

				// TODO: implement
				//if (menu == "topslide" && Menus.TopSlide != null)
				//	Menus.TopSlide.display = true;

				if (menu == "bottom" && Menus.Bottom != null)
					Menus.Bottom.display = true;
			}
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

		#endregion
	}

	public class UberWebViewMessage
	{
		public string Action { get; set; }

		public string Result { get; set;}
	}
}
