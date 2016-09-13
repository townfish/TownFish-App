using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Windows.Input;

using Xamarin.Forms;

using Newtonsoft.Json;

using TownFish.App.Models;


namespace TownFish.App.ViewModels
{
	public class BrowserPageViewModel : ViewModelBase
	{
		#region Properties and Events

		public event EventHandler<string> CallbackRequested;

		public event EventHandler<string> MenusLoaded;

		public string SourceUrl
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		public bool IsTopBarVisible
		{
			get { return Get<bool>(); }
			set { Set (value); }
		}

		public bool IsLoading
		{
			get { return Get<bool>(); }
			set { Set (value); }
		}

		#region Colours

		public Color MenuBarBackgroundColour
		{
			get { return Get<Color> (() => Color.FromHex ("#484848")); }
			set { Set (value); }
		}

		public Color MenuBarTextColour
		{
			get { return Get<Color> (() => Color.FromHex ("#ffffff")); }
			set { Set (value); }
		}

		public Color SubMenuBackgroundColour
		{
			get { return Get<Color> (() => Color.FromHex ("#e8e8e8")); }
			set { Set (value); }
		}

		public Color BottomMenuBackgroundColour
		{
			get { return Get<Color> (() => Color.FromHex ("#f2f2f2")); }
			set { Set (value); }
		}

		public Color BottomMenuCounterColour
		{
			get { return Get<Color> (() => Color.FromHex ("#d62327")); }
			set { Set (value); }
		}

		public Color LocationsBackgroundColour
		{
			get { return Get<Color> (() => Color.FromHex ("#444444")); }
			set { Set (value); }
		}

		public Color LocationsContrastBackgroundColour
		{
			get { return Get<Color> (() => Color.FromHex ("#333333")); }
			set { Set (value); }
		}

		public Color MyLocationsTextColour
		{
			get { return Get<Color> (() => Color.FromHex ("#c8c8c8")); }
			set { Set (value); }
		}

		public Color LocationsTextColour
		{
			get { return Get<Color> (() => Color.FromHex ("#ececec")); }
			set { Set (value); }
		}

		public Color LocationsLinkColour
		{
			get { return Get<Color> (() => Color.FromHex ("#4f92eb")); }
			set { Set (value); }
		}

		public Color LocationsSearchInputTextColour
		{
			get { return Get<Color> (() => Color.FromHex ("#b9b9b9")); }
			set { Set (value); }
		}

		public Color LocationsSearchResultsTextColour
		{
			get { return Get<Color> (() => Color.FromHex ("#989898")); }
			set { Set (value); }
		}

		#endregion Colours

		#region Bottom Bar

		#region Bottom Action 1

		public string BottomAction1Label
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		public ICommand BottomAction1Command
		{
			get { return Get<ICommand>(); }
			set { Set (value); }
		}

		#endregion Bottom Action 1

		#region Bottom Action 2

		public string BottomAction2Label
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		public ICommand BottomAction2Command
		{
			get { return Get<ICommand>(); }
			set { Set (value); }
		}

		public bool BottomAction2HasNumber
		{
			get { return Get<bool>(); }
			set { Set (value); }
		}

		public string BottomAction2Number
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		#endregion Bottom Action 2

		#region Bottom Action 3

		public string BottomAction3Label
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		public ICommand BottomAction3Command
		{
			get { return Get<ICommand>(); }
			set { Set (value); }
		}

		public bool BottomAction3HasNumber
		{
			get { return Get<bool>(); }
			set { Set (value); }
		}

		public string BottomAction3Number
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		#endregion Bottom Action 3

		#region Bottom Action 4

		public string BottomAction4Label
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		public ICommand BottomAction4Command
		{
			get { return Get<ICommand>(); }
			set { Set (value); }
		}

		#endregion Bottom Action 4

		#region Bottom Action 5

		public string BottomAction5Label
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		public ICommand BottomAction5Command
		{
			get { return Get<ICommand>(); }
			set { Set (value); }
		}

		#endregion Bottom Action 5

		public bool IsBottomBarVisible
		{
			get { return Get<bool>(); }
			set { Set (value); }
		}

		#endregion Bottom Bar

		#region Top Bar

		public string TopBarLeftLabel
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		public ICommand TopBarLeftCommand
		{
			get { return Get<ICommand>(); }
			set { Set (value); }
		}

		public string TopBarRightLabel
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		public ICommand TopBarRightCommand
		{
			get { return Get<ICommand>(); }
			set { Set (value); }
		}

		public string TopBarRight1Label
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		public ICommand TopBarRight1Command
		{
			get { return Get<ICommand>(); }
			set { Set (value); }
		}

		public string PageTitle
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		public bool LeftActionIsLocationPin { get; set; }

		public string LocationName
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		#endregion Top Bar

		#region Top Sub

		public string TopAction1Label
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		public ICommand TopAction1Command
		{
			get { return Get<ICommand>(); }
			set { Set (value); }
		}

		public FontAttributes TopAction1Bold
		{
			get { return Get<FontAttributes>(); }
			set { Set (value); }
		}

		public string TopAction2Label
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		public FontAttributes TopAction2Bold
		{
			get { return Get<FontAttributes>(); }
			set { Set (value); }
		}

		public ICommand TopAction2Command
		{
			get { return Get<ICommand>(); }
			set { Set (value); }
		}

		public string TopAction3Label
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		public ICommand TopAction3Command
		{
			get { return Get<ICommand>(); }
			set { Set (value); }
		}

		public FontAttributes TopAction3Bold
		{
			get { return Get<FontAttributes>(); }
			set { Set (value); }
		} 

		public string TopAction4Label
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		public ICommand TopAction4Command
		{
			get { return Get<ICommand>(); }
			set { Set (value); }
		}

		public FontAttributes TopAction4Bold
		{
			get { return Get<FontAttributes>(); }
			set { Set (value); }
		}

		public ICommand TopActionMoreCommand
		{
			get { return Get<ICommand>(); }
			set { Set (value); }
		}

		public string TopActionMoreLabel
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		public bool IsTopSubBarVisible
		{
			get { return Get<bool>(); }
			set { Set (value); }
		}

		public List<TownFishMenuItem> OverflowImages { get; set; }

		#endregion Top Sub

		#region Top Form

		public string TopFormLeftActionLabel
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		public ICommand TopFormLeftAction
		{
			get { return Get<ICommand>(); }
			set { Set (value); }
		}

		public string TopFormRightActionLabel
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		public ICommand TopFormRightAction
		{
			get { return Get<ICommand>(); }
			set { Set (value); }
		}

		public bool IsTopFormBarVisible
		{
			get { return Get<bool>(); }
			set { Set (value); }
		}

		#endregion Top Form

		#region Search

		public ObservableCollection<TownfishLocationItem> LocationSearchItems
		{
			get { return Get<ObservableCollection<TownfishLocationItem>>(); }

			set
			{
				// if collection changes, we may or may not have any items in the new one
				if (Set (value))
					OnPropertyChanged (() => SearchLocationHasNoItems);
			}
		}

		public bool SearchLocationHasResults
		{
			get { return Get<bool> () && !string.IsNullOrWhiteSpace (SearchTerm) && SearchTerm.Length > 2; }
			set { Set (value); }
		}

		public bool SearchLocationHasNoItems
		{
			get { return LocationSearchItems?.Count == 0; }
		}

		public ICommand CancelSearchCommand
			{ get { return new Command (_ => CancelLocationSearch()); } }

		public bool SearchHasContent
		{
			get { return Get<bool>(); }
			set { Set (value); }
		}

		public string SearchTerm
		{
			get { return Get<string>(); }

			set
			{
				// if text changes, we may want to hide search results
				if (Set (value))
					OnPropertyChanged (() => SearchLocationHasResults);
			}
		}

		#endregion Search

		#region Locations

		public ObservableCollection<AvailableLocation> AvailableLocations
		{
			get { return Get<ObservableCollection<AvailableLocation>>(); }
			set { Set (value); }
		}

		public AvailableLocation CurrentLocation
		{
			get { return Get<AvailableLocation>(); }
			set { Set (value); }
		}

		public string InfoLocationIcon
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		#endregion Locations

		#endregion Properties and Events

		#region Methods

		public async void UpdateLocationList (string searchTerm)
		{
			sSearchTerm = searchTerm;

			try
			{
				using (var client = new HttpClient())
				{
					var resultJson = await client.GetStringAsync (
							App.BaseUrl + mLocationApiFormat.Replace ("{term}", searchTerm));

					if (!string.IsNullOrEmpty (resultJson))
					{
						var model = JsonConvert.DeserializeObject<TownFishLocationList> (resultJson);
						if (model?.Items != null)
						{
							LocationSearchItems = new ObservableCollection<TownfishLocationItem> (model.Items);
							SearchLocationHasResults = true;
						}
					}
				}
			}
			catch (Exception)
			{
				// TODO Alert the user of this crap up the wall
				CancelLocationSearch();
			}
		}

		public void SetLocation (string cityID)
		{
			SourceUrl = App.BaseUrl + mLocationSetFormat.Replace ("{id}", cityID) + App.QueryString;
		}

		public void LoadMenuMap (TownFishMenuMap map)
		{
			mLocationApiFormat = map.LocationsAPI;
			mLocationSetFormat = map.LocationSetUrl;

			IsTopBarVisible = false;
			IsTopSubBarVisible = false;
			IsTopFormBarVisible = false;
			IsBottomBarVisible = false;

			LeftActionIsLocationPin = false;

			LocationName = "";

			try
			{
				MenuBarBackgroundColour = Color.FromHex ("#" + map.StatusBarBackgroundColor);
				MenuBarTextColour = Color.FromHex ("#" + map.StatusBarTextColor);
			}
			catch {} // don't care if/why it fails as we'll just use default property values

			LoadMenus (map.Menus);

			// now we've parsed the menu map we can set location properties, if needed
			if (IsTopBarVisible)
			{
				var locationMenuItem = map.Menus.Top.items.FirstOrDefault (i => i.Type == "locationpin");
				if (locationMenuItem != null)
				{
					var size = "mdpi"; // HACK: force sizes for now; should be locationMenuItem.Size;
					var colour = locationMenuItem.Color;

					CurrentLocation = map.CurrentLocation;

					// set this now that we know it
					LocationName = CurrentLocation.Name;

					InfoLocationIcon = App.BaseUrl + map.LocationIcons.Info
							.Replace ("{size}", size)
							.Replace ("{color}", colour);

					var locs = new List<AvailableLocation>();
					foreach (var loc in map.AvailableLocations)
					{
						loc.Colour = LocationsTextColour;

						loc.LeftImage = App.BaseUrl + map.LocationIcons.Pin
							.Replace ("{size}", size)
							.Replace ("{color}", colour);

						if (loc.ID == CurrentLocation.ID)
						{
							loc.IsSelected = true;
							loc.RightImage = App.BaseUrl + map.LocationIcons.Tick
									.Replace ("{size}", size)
									.Replace ("{color}", colour);
						}

						loc.LockLocationIcon = App.BaseUrl + map.LocationIcons.Lock
								.Replace ("{size}", size)
								.Replace ("{color}", colour);

						locs.Add (loc);
					}

					// now we have them all we can trigger (re-)bindings
					AvailableLocations = new ObservableCollection<AvailableLocation> (locs);
				}
			}

			OnMenusLoaded();
		}

		public void CancelLocationSearch()
		{
			SearchLocationHasResults = false;
			SearchTerm = "";
		}

		void OnMenusLoaded()
		{
			MenusLoaded?.Invoke (this, "");
		}

		void OnCallbackRequested (string callbackName)
		{
			CallbackRequested?.Invoke (this, callbackName);
		}

		void LoadMenus (TownFishMenuList menus)
		{
			if (menus == null)
				return;

			var top = menus.Top;
			IsTopBarVisible = top != null && top.display;

			if (IsTopBarVisible)
			{
				var topLeftItem = top.items.FirstOrDefault (i => i.Align == "left");
				if (topLeftItem != null)
				{
					TopBarLeftLabel = GenerateMenuItem (top.items [0]);
					TopBarLeftCommand = GenerateMenuAction (top.items [0]);
				}
				else
				{
					TopBarLeftLabel = "";
					TopBarLeftCommand = null;
				}

				var isHeadingItem = top.items.FirstOrDefault (i => i.Type == "heading");
				if (isHeadingItem != null)
					PageTitle = GenerateMenuItem (top.items [1]);
				else
					PageTitle = "";

				var rightItems = top.items.Where (i => i.Align == "right").ToList();
				if (rightItems.Count == 1)
				{
					TopBarRightLabel = "";
					TopBarRightCommand = null;
					TopBarRight1Label = GenerateMenuItem (rightItems [0]);
					TopBarRight1Command = GenerateMenuAction (rightItems [0]);
				}
				else if (rightItems.Count == 2)
				{
					TopBarRightLabel = GenerateMenuItem (rightItems [0]);
					TopBarRightCommand = GenerateMenuAction (rightItems [0]);
					TopBarRight1Label = GenerateMenuItem (rightItems [1]);
					TopBarRight1Command = GenerateMenuAction (rightItems [1]);
				}
				else
				{
					TopBarRightLabel = "";
					TopBarRightCommand = null;
					TopBarRight1Label = "";
					TopBarRight1Command = null;
				}
			}
			else
			{
				TopBarLeftLabel = "";
				TopBarLeftCommand = null;
				TopBarRight1Label = "";
				TopBarRightCommand = null;
				TopBarRight1Label = "";
				TopBarRight1Command = null;
			}

			var topSub = menus.TopSub;
			IsTopSubBarVisible = topSub != null && topSub.display;

			if (IsTopSubBarVisible)
			{
				// Top Menu
				if (topSub.items.Count > 0 && topSub.items [0] != null)
				{
					TopAction1Label = GenerateMenuItem (topSub.items [0]);
					TopAction1Command = GenerateMenuAction (topSub.items [0]);

					if (topSub.items [0].Highlight)
						TopAction1Bold = FontAttributes.Bold;
					else
						TopAction1Bold = FontAttributes.None;
				}
				else
				{
					TopAction1Label = "";
					TopAction1Command = null;
				}

				if (topSub.items.Count > 1 && topSub.items [1] != null)
				{
					TopAction2Label = GenerateMenuItem (topSub.items [1]);
					TopAction2Command = GenerateMenuAction (topSub.items [1]);

					if (topSub.items [1].Highlight)
						TopAction2Bold = FontAttributes.Bold;
					else
						TopAction2Bold = FontAttributes.None;
				}
				else
				{
					TopAction2Label = "";
					TopAction2Command = null;
				}

				if (topSub.items.Count > 2 && topSub.items [2] != null)
				{
					TopAction3Label = GenerateMenuItem (topSub.items [2]);
					TopAction3Command = GenerateMenuAction (topSub.items [2]);

					if (topSub.items [2].Highlight)
						TopAction3Bold = FontAttributes.Bold;
					else
						TopAction3Bold = FontAttributes.None;
				}
				else
				{
					TopAction3Label = "";
					TopAction3Command = null;
				}

				if (topSub.items.Count > 3 && topSub.items [3] != null)
				{
					TopAction4Label = GenerateMenuItem (topSub.items [3]);
					TopAction4Command = GenerateMenuAction (topSub.items [3]);

					if (topSub.items [3].Highlight)
						TopAction4Bold = FontAttributes.Bold;
					else
						TopAction4Bold = FontAttributes.None;
				}
				else
				{
					TopAction4Label = "";
					TopAction4Command = null;
				}

				if (topSub.items.Count > 4)
				{
					var moreIcon = topSub.items.FirstOrDefault (i => i.Type == "limitby");
					if (moreIcon != null)
					{
						TopActionMoreLabel = GenerateMenuItem (moreIcon);
						OverflowImages = topSub.items.Skip (3).Where (i => i.Type != "limitby").ToList();
					}
				}
				else
				{
					TopActionMoreLabel = "";
					OverflowImages = null;
				}
			}

			var topForm = menus.TopForm;
			IsTopFormBarVisible = topForm != null && topForm.display && topForm.items.Count > 0;

			if (IsTopFormBarVisible)
			{
				TopFormLeftActionLabel = GenerateMenuItem (topForm.items [0]);
				TopFormLeftAction = GenerateMenuAction (topForm.items [0]);

				var topFormRightItem = topForm.items.FirstOrDefault (i => i.Align == "right");
				if (topFormRightItem != null)
				{
					TopFormRightActionLabel = GenerateMenuItem (topFormRightItem);
					TopFormRightAction = GenerateMenuAction (topFormRightItem);
				}

				// if 3 items, item 1 is title (we hope!)
				if (topForm.items.Count > 2 && topForm.items [1] != null)
					PageTitle = GenerateMenuItem (topForm.items [1]);
			}

			var bottom = menus.Bottom;
			IsBottomBarVisible = bottom != null && bottom.display;

			if (IsBottomBarVisible)
			{
				// Bottom Menu
				if (bottom.items.Count > 0 && bottom.items [0] != null)
				{
					BottomAction1Label = GenerateMenuItem (bottom.items [0]);
					BottomAction1Command = GenerateMenuAction (bottom.items [0]);
				}
				else
				{
					BottomAction1Label = "";
					BottomAction1Command = null;
				}

				if (bottom.items.Count > 0 && bottom.items [1] != null)
				{
					BottomAction2Label = GenerateMenuItem (bottom.items [1]);
					BottomAction2Command = GenerateMenuAction (bottom.items [1]);

					if (!string.IsNullOrEmpty (bottom.items [1].Super))
					{
						BottomAction2HasNumber = true;
						BottomAction2Number = bottom.items [1].Super;
					}
				}
				else
				{
					BottomAction2Label = "";
					BottomAction2Command = null;
					BottomAction2HasNumber = false;
				}

				if (bottom.items.Count > 0 && bottom.items [2] != null)
				{
					BottomAction3Label = GenerateMenuItem (bottom.items [2]);
					BottomAction3Command = GenerateMenuAction (bottom.items [2]); ;

					if (!string.IsNullOrEmpty (bottom.items [2].Super))
					{
						BottomAction3HasNumber = true;
						BottomAction3Number = bottom.items [2].Super;
					}
				}
				else
				{
					BottomAction3Label = "";
					BottomAction3Command = null;
					BottomAction3HasNumber = false;
				}

				if (bottom.items.Count > 0 && bottom.items [3] != null)
				{
					BottomAction4Label = GenerateMenuItem (bottom.items [3]);
					BottomAction4Command = GenerateMenuAction (bottom.items [3]);
				}
				else
				{
					BottomAction4Label = "";
					BottomAction4Command = null;
				}

				if (bottom.items.Count > 0 && bottom.items [4] != null)
				{
					BottomAction5Label = GenerateMenuItem (bottom.items [4]);
					BottomAction5Command = GenerateMenuAction (bottom.items [4]);
				}
				else
				{
					BottomAction5Label = "";
					BottomAction5Command = null;
				}
			}
			else
			{
				BottomAction1Label = "";
				BottomAction1Command = null;
				BottomAction2Label = "";
				BottomAction2Command = null;
				BottomAction2HasNumber = false;
				BottomAction3Label = "";
				BottomAction3HasNumber = false;
				BottomAction3Command = null;
				BottomAction4Label = "";
				BottomAction4Command = null;
				BottomAction5Label = "";
				BottomAction5Command = null;
			}
		}

		string GenerateMenuItem (TownFishMenuItem item)
		{
			if (item.Kind == "icon")
			{
				var url = item.IconUrl;

				url = url.Replace("{size}", "hdpi"); // HACK: force sizes for now; should be item.Size);
				url = url.Replace("{color}", item.Color);

				return App.BaseUrl + url;
			}
			else if (item.Type == "heading")
			{
				return item.Main;
			}
			else
			{
				return item.Value;
			}
		}

		ICommand GenerateMenuAction (TownFishMenuItem item)
		{
			switch (item.Type)
			{
				case "locationpin":
					LeftActionIsLocationPin = true;
					return new Command (_ =>
						CancelLocationSearch());

				case "callback":
					return new Command (_ =>
						OnCallbackRequested (item.Name));

				case "link":
					return new Command (_ =>
						{ SourceUrl = App.BaseUrl + item.Href + App.QueryString + "#" + DateTime.Now.Ticks; }); // TODO: remove nav hash!

				case "back":
					return new Command (_ =>
						{ SourceUrl = App.BaseUrl + item.Href + App.QueryString; });

				default:
					return null;
			}
		}

		#endregion Methods

		#region Fields

		public static string sSearchTerm = "";

		string mLocationApiFormat = "";
		string mLocationSetFormat = "";

		#endregion Fields
	}
}
