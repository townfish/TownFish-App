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
			set { Set (value); }
		}

		public bool IsSearchPanelVisible
		{
			get { return Get<bool>(); }
			set { Set (value); }
		}

		public bool SearchLocationHasResults
		{
			get { return Get<bool>(); }
			set { Set (value); }
		}

		public ICommand CancelSearchCommand
		{
			get
			{
				return new Command (_ =>
					{
						SearchLocationHasResults = false;
						SearchTerm = "";
					});
			}
		}

		public bool SearchHasContent
		{
			get { return Get<bool>(); }
			set { Set (value); }
		}

		public string SearchTerm
		{
			get { return Get<string>(); }
			set { Set (value); }
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

		public void LoadMenuMap (TownFishMenuMap map)
		{
			mLocationApiFormat = map.LocationsAPI;
			mLocationSetFormat = map.LocationSetUrl;

			IsBottomBarVisible = false;
			IsTopBarVisible = false;
			IsTopSubBarVisible = false;
			IsTopFormBarVisible = false;
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
					CurrentLocation = map.CurrentLocation;

					InfoLocationIcon = App.BaseUrl + map.LocationIcons.Info
							.Replace ("{size}", locationMenuItem.Size)
							.Replace ("{color}", locationMenuItem.Color);

					var locs = new List<AvailableLocation>();
					foreach (var loc in map.AvailableLocations)
					{
						loc.Colour = LocationsTextColour;

						loc.LeftImage = App.BaseUrl + map.LocationIcons.Pin
							.Replace ("{size}", locationMenuItem.Size)
							.Replace ("{color}", locationMenuItem.Color);

						if (loc.ID == CurrentLocation.ID)
						{
							// set this now that we know it
							LocationName = loc.Name;

							loc.IsSelected = true;
							loc.RightImage = App.BaseUrl + map.LocationIcons.Tick
									.Replace ("{size}", locationMenuItem.Size)
									.Replace ("{color}", locationMenuItem.Color);
						}

						loc.LockLocationIcon = App.BaseUrl + map.LocationIcons.Lock
								.Replace ("{size}", locationMenuItem.Size)
								.Replace ("{color}", locationMenuItem.Color);

						locs.Add (loc);
					}

					// now we have them all we can trigger (re-)bindings
					AvailableLocations = new ObservableCollection<AvailableLocation> (locs);
				}
			}

			OnMenusLoaded();
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

			var bottom = menus.Bottom;
			IsBottomBarVisible = bottom != null && bottom.display;

			if (IsBottomBarVisible)
			{
				// Bottom Menu
				if (bottom.items.Count > 0 && bottom.items [0] != null)
				{
					BottomAction1Label = GenerateLabel (bottom.items [0]);
					BottomAction1Command = GenerateAction (bottom.items [0]);
				}
				else
				{
					BottomAction1Label = "";
					BottomAction1Command = null;
				}

				if (bottom.items.Count > 0 && bottom.items [1] != null)
				{
					BottomAction2Label = GenerateLabel (bottom.items [1]);
					BottomAction2Command = GenerateAction (bottom.items [1]);

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
					BottomAction3Label = GenerateLabel (bottom.items [2]);
					BottomAction3Command = GenerateAction (bottom.items [2]); ;

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
					BottomAction4Label = GenerateLabel (bottom.items [3]);
					BottomAction4Command = GenerateAction (bottom.items [3]);
				}
				else
				{
					BottomAction4Label = "";
					BottomAction4Command = null;
				}

				if (bottom.items.Count > 0 && bottom.items [4] != null)
				{
					BottomAction5Label = GenerateLabel (bottom.items [4]);
					BottomAction5Command = GenerateAction (bottom.items [4]);
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

			var top = menus.Top;
			IsTopBarVisible = top != null && top.display;

			if (IsTopBarVisible)
			{
				var topLeftItem = top.items.FirstOrDefault (i => i.Align == "left");
				if (topLeftItem != null)
				{
					TopBarLeftLabel = GenerateLabel (top.items [0]);
					TopBarLeftCommand = GenerateAction (top.items [0]);
				}
				else
				{
					TopBarLeftLabel = "";
					TopBarLeftCommand = null;
				}

				var isHeadingItem = top.items.FirstOrDefault (i => i.Type == "heading");
				if (isHeadingItem != null)
					PageTitle = GenerateLabel (top.items [1]);
				else
					PageTitle = "";

				var rightItems = top.items.Where (i => i.Align == "right");
				if (rightItems.Count() == 1)
				{
					TopBarRightLabel = "";
					TopBarRightCommand = null;
					TopBarRight1Label = GenerateLabel (rightItems.First());
					TopBarRight1Command = GenerateAction (rightItems.First());
				}
				else if (rightItems.Count() == 2)
				{
					TopBarRightLabel = GenerateLabel (rightItems.First());
					TopBarRightCommand = GenerateAction (rightItems.First());
					TopBarRight1Label = GenerateLabel (rightItems.Last());
					TopBarRight1Command = GenerateAction (rightItems.Last());
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
					TopAction1Label = GenerateLabel (topSub.items [0]);
					TopAction1Command = GenerateAction (topSub.items [0]);

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
					TopAction2Label = GenerateLabel (topSub.items [1]);
					TopAction2Command = GenerateAction (topSub.items [1]);

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
					TopAction3Label = GenerateLabel (topSub.items [2]);
					TopAction3Command = GenerateAction (topSub.items [2]);

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
					TopAction4Label = GenerateLabel (topSub.items [3]);
					TopAction4Command = GenerateAction (topSub.items [3]);

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
						TopActionMoreLabel = GenerateLabel (moreIcon);
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
				TopFormLeftActionLabel = GenerateLabel (topForm.items [0]);
				TopFormLeftAction = GenerateAction (topForm.items [0]);

				if (topForm.items.Count > 1)
					PageTitle = GenerateLabel (topForm.items [1]);

				if (topForm.items.Count > 2)
				{
					TopFormRightActionLabel = GenerateLabel (topForm.items [2]);
					TopFormRightAction = GenerateAction (topForm.items [2]);
				}
			}
		}

		string GenerateLabel(TownFishMenuItem item)
		{
			if (item.Kind == "icon")
			{
				var url = item.IconUrl;

				url = url.Replace("{size}", item.Size);
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

		ICommand GenerateAction(TownFishMenuItem item)
		{
			switch (item.Type)
			{
				case "link":
					return new Command (_ =>
						{ SourceUrl = App.BaseUrl + item.Href + App.BaseUrlParam + "#" + DateTime.Now.Ticks; });

				case "callback":
					return new Command (_ =>
						OnCallbackRequested (item.Name));

				case "back":
					return new Command (_ =>
						{ SourceUrl = App.BaseUrl + item.Href + App.BaseUrlParam; });

				case "locationpin":
					LeftActionIsLocationPin = true;
					return new Command (_ =>
						{ SearchLocationHasResults = false; });

				default:
					return null;
			}
		}

		public static BrowserPageViewModel Create(TownFishMenuMap map)
		{
			var viewModel = new BrowserPageViewModel
			{
				OverflowImages = new List<TownFishMenuItem>(),
				SourceUrl = App.BaseUrl + App.BaseUrlParam
			};

			if (map != null)
				viewModel.LoadMenuMap (map);

			return viewModel;
		}

		public async void UpdateLocationList(string searchTerm)
		{
			sSearchTerm = searchTerm;

			try
			{
				using (var client = new HttpClient())
				{
					var resultJson = await client.GetStringAsync(App.BaseUrl + mLocationApiFormat.Replace("{term}", searchTerm));

					if (!string.IsNullOrEmpty(resultJson))
					{
						var model = JsonConvert.DeserializeObject<TownFishLocationList>(resultJson);
						if (model != null)
						{
							LocationSearchItems = new ObservableCollection<TownfishLocationItem> (model.items);
							SearchLocationHasResults = true;
						}
					}
				}
			}
			catch (Exception)
			{
				// TODO Alert the user of this crap up the wall
				SearchLocationHasResults = false;
			}
		}

		public void SetLocation(string cityId)
		{
			SourceUrl = App.BaseUrl + mLocationSetFormat.Replace("{id}", cityId) + App.BaseUrlParam;
		}

		#endregion Methods

		#region Fields

		public static string sSearchTerm = "";

		string mLocationApiFormat = "";
		string mLocationSetFormat = "";

		#endregion Fields
	}
}
