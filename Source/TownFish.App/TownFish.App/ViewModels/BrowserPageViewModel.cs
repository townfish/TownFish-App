using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Diagnostics;
using System.Windows.Input;

using Xamarin.Forms;

using Newtonsoft.Json;

using TownFish.App.Models;
using StreetHawkCrossplatform;

namespace TownFish.App.ViewModels
{
	public class BrowserPageViewModel: ViewModelBase
	{
		#region Nested Types

		public class MenuIconModel
		{
			#region Construction

			public MenuIconModel (TownFishMenuItem item)
			{
				mItem = item ?? throw new ArgumentNullException ("item can not be null");
			}

			#endregion Construction

			#region Methods

			string GetSource()
			{
				if (!IsVisible || string.IsNullOrEmpty (mItem.IconUrl))
					return null;

				var url = mItem.IconUrl;

				url = url.Replace ("{size}", "hdpi"); // HACK: force sizes for now; should be item.Size);
				url = url.Replace ("{color}", mItem.Color);

				return App.BaseUrl + url;
			}

			#endregion Methods

			#region Properties

			public bool IsVisible => mItem.Kind == "icon";

			public string Source => GetSource();

			#endregion Properties

			#region Fields

			TownFishMenuItem mItem;

			#endregion Fields
		}

		public class MenuLabelModel
		{
			#region Construction

			public MenuLabelModel (TownFishMenuItem item)
			{
				mItem = item ?? throw new ArgumentNullException ("item can not be null");
			}

			#endregion Construction

			#region Methods
			#endregion Methods

			#region Properties

			public bool IsVisible => mItem.Kind == "text";

			public string Text => mItem.Value;

			#endregion Properties

			#region Fields

			TownFishMenuItem mItem;

			#endregion Fields
		}

		public class CallbackInfo
		{
			#region Properties

			public string Name { get; set; }

			public bool IsNative { get; set; }

			#endregion Properties

			#region Fields

			public const string Info = "info";

			#endregion Fields
		}

		#endregion Nested Types

		#region Properties and Events

		public event EventHandler MenusLoaded;

		public event EventHandler LocationTapped;

		public event EventHandler<CallbackInfo> CallbackRequested;

		public event EventHandler<string> NavigateRequested;

		public string SourceUrl
		{
			get { return Get<string>(); }
			set { Set (value); }
		}

		public bool IsLoading
		{
			get { return Get<bool>(); }

			set
			{
				// allows us to clear loading content when not loading
				if (Set (value))
					OnPropertyChanged (() => LoadingWebViewSource);
			}
		}

		public HtmlWebViewSource LoadingWebViewSource =>
				new HtmlWebViewSource { Html = LoadingHtml };

		public string LoadingHtml
		{
			get
			{
				// if not loading, no need for expensive animations to be running
				if (!IsLoading)
					return "";

				using (var stm = App.Assembly.GetManifestResourceStream (
						"TownFish.App.Resources.LoadingAnimation.html"))
				using (var reader = new StreamReader (stm))
					return reader.ReadToEnd();
			}
		}

		public string SyncToken { get; private set; }

		#region Colours

		public Color MenuBarBackgroundColour
		{
			get { return Get (() => Color.FromHex ("#484848")); }
			private set { Set (value); }
		}

		public Color MenuBarTextColour
		{
			get { return Get (() => Color.FromHex ("#ffffff")); }
			private set { Set (value); }
		}

		public Color SubMenuBackgroundColour
		{
			get { return Get (() => Color.FromHex ("#e8e8e8")); }
			private set { Set (value); }
		}

		public Color BottomMenuBackgroundColour
		{
			get { return Get (() => Color.FromHex ("#f2f2f2")); }
			private set { Set (value); }
		}

		public Color BottomMenuCounterColour
		{
			get { return Get (() => Color.FromHex ("#d62327")); }
			private set { Set (value); }
		}

		public Color LocationsBackgroundColour
		{
			get { return Get (() => Color.FromHex ("#444444")); }
			private set { Set (value); }
		}

		public Color LocationsContrastBackgroundColour
		{
			get { return Get (() => Color.FromHex ("#333333")); }
			private set { Set (value); }
		}

		public Color MyLocationsTextColour
		{
			get { return Get (() => Color.FromHex ("#c8c8c8")); }
			private set { Set (value); }
		}

		public Color LocationsTextColour
		{
			get { return Get (() => Color.FromHex ("#ececec")); }
			private set { Set (value); }
		}

		public Color LocationLinkColour
		{
			get { return Get (() => Color.FromHex ("#4f92eb")); }
			private set { Set (value); }
		}

		public Color LocationsSearchInputTextColour
		{
			get { return Get (() => Color.FromHex ("#b9b9b9")); }
			private set { Set (value); }
		}

		public Color LocationsSearchResultsTextColour
		{
			get { return Get (() => Color.FromHex ("#989898")); }
			private set { Set (value); }
		}

		public Color DiscoveriesLinkColour
		{
			get { return Get (() => Color.FromHex ("#5099f7")); }
			private set { Set (value); }
		}

		#endregion Colours

		#region Bottom Bar

		public bool IsBottomBarVisible
		{
			get { return Get<bool>(); }
			set { Set (value); }
		}

		// I have to comment here about the very funny name this property has ended up with.
		// Fnaar fnaar.
		public ObservableCollection<BottomActionViewModel> BottomActions
		{
			get { return Get<ObservableCollection<BottomActionViewModel>>(); }
			set { Set (value); }
		}

		#endregion Bottom Bar

		#region Top Bar

		public string TopBarLeftLabel
		{
			get { return Get<string>(); }
			private set { Set (value); }
		}

		public ICommand TopBarLeftCommand
		{
			get { return Get<ICommand>(); }
			private set { Set (value); }
		}

		public MenuLabelModel TopBarRightLabel
		{
			get { return Get<MenuLabelModel>(); }
			private set { Set (value); }
		}

		public MenuIconModel TopBarRightIcon
		{
			get { return Get<MenuIconModel>(); }
			private set { Set (value); }
		}

		public ICommand TopBarRightCommand
		{
			get { return Get<ICommand>(); }
			private set { Set (value); }
		}

		public MenuLabelModel TopBarRight1Label
		{
			get { return Get<MenuLabelModel>(); }
			private set { Set (value); }
		}

		public MenuIconModel TopBarRight1Icon
		{
			get { return Get<MenuIconModel>(); }
			private set { Set (value); }
		}

		public ICommand TopBarRight1Command
		{
			get { return Get<ICommand>(); }
			private set { Set (value); }
		}

		public string PageTitle
		{
			get { return Get<string>(); }
			private set { Set (value); }
		}

		public string LocationName
		{
			get { return Get<string>(); }
			set { Set (value); } // public, as sometimes needs to be set from the view (yuck!)
		}

		public bool IsTopBarVisible
		{
			get { return Get<bool>(); }
			set { Set (value); }
		}

		// NOTE: notify this changed when those it depends on change
		public bool IsLocationNameVisible => !IsDiscoveriesVisible && !IsDiscoveriesInfoVisible;

		#endregion Top Bar

		#region Top Sub

		public string TopAction1Label
		{
			get { return Get<string>(); }
			private set { Set (value); }
		}

		public ICommand TopAction1Command
		{
			get { return Get<ICommand>(); }
			private set { Set (value); }
		}

		public FontAttributes TopAction1Bold
		{
			get { return Get<FontAttributes>(); }
			private set { Set (value); }
		}

		public string TopAction2Label
		{
			get { return Get<string>(); }
			private set { Set (value); }
		}

		public FontAttributes TopAction2Bold
		{
			get { return Get<FontAttributes>(); }
			private set { Set (value); }
		}

		public ICommand TopAction2Command
		{
			get { return Get<ICommand>(); }
			private set { Set (value); }
		}

		public string TopAction3Label
		{
			get { return Get<string>(); }
			private set { Set (value); }
		}

		public ICommand TopAction3Command
		{
			get { return Get<ICommand>(); }
			private set { Set (value); }
		}

		public FontAttributes TopAction3Bold
		{
			get { return Get<FontAttributes>(); }
			private set { Set (value); }
		}

		public string TopAction4Label
		{
			get { return Get<string>(); }
			private set { Set (value); }
		}

		public ICommand TopAction4Command
		{
			get { return Get<ICommand>(); }
			private set { Set (value); }
		}

		public FontAttributes TopAction4Bold
		{
			get { return Get<FontAttributes>(); }
			private set { Set (value); }
		}

		public ICommand TopActionMoreCommand
		{
			get { return Get<ICommand>(); }
			set { Set (value); }
		}

		public string TopActionMoreLabel
		{
			get { return Get<string>(); }
			private set { Set (value); }
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
			private set { Set (value); }
		}

		public ICommand TopFormLeftActionCommand
		{
			get { return Get<ICommand>(); }
			private set { Set (value); }
		}

		public string TopFormRightActionLabel
		{
			get { return Get<string>(); }
			private set { Set (value); }
		}

		public ICommand TopFormRightActionCommand
		{
			get { return Get<ICommand>(); }
			private set { Set (value); }
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

			private set
			{
				// if collection changes, we may or may not have any items in the new one
				if (Set (value))
				{
					OnPropertyChanged (() => SearchLocationActive);
					OnPropertyChanged (() => SearchLocationListEmpty);
				}
			}
		}

		public bool SearchHasContent => (SearchTerm?.Length ?? 0) > 0;

		public bool SearchLocationActive => !SearchLocationListEmpty || SearchHasContent;

		public bool SearchLocationListEmpty => (LocationSearchItems?.Count ?? 0) == 0;

		public ICommand CancelSearchCommand => new Command (_ => CancelLocationSearch());

		public string SearchTerm
		{
			get { return Get<string>(); }

			set
			{
				// if text changes, update search results if enough has been entered
				if (Set (value))
				{
					// update this for SearchFormatConverter to reference
					CurrentSearchTerm = value;

					if (value == null || value.Length == 0)
						LocationSearchItems = null;
					else if (value.Length > 2)
						UpdateLocationList();
				}
			}
		}

		#endregion Search

		#region Locations

		public ObservableCollection<AvailableLocation> AvailableLocations
		{
			get { return Get<ObservableCollection<AvailableLocation>>(); }
			private set { Set (value); }
		}

		public AvailableLocation CurrentLocation
		{
			get { return Get<AvailableLocation>(); }
			private set { Set (value); }
		}

		public string InfoLocationIcon
		{
			get { return Get<string>(); }
			private set { Set (value); }
		}

		#endregion Locations

		#region Discoveries

		public bool IsDiscoveriesVisible
		{
			get { return Get<bool>(); }

			set
			{
				// NOTE: side-effect - if this changes, location name might change too
				if (Set (value))
					OnPropertyChanged (() => IsLocationNameVisible);
			}
		}

		public bool IsDiscoveriesListVisible => !IsDiscoveriesEmpty;

		public bool IsDiscoveriesInfoVisible
		{
			get { return Get<bool>(); }

			set
			{
				// NOTE: side-effect - if this changes, location name might change too
				if (Set (value))
					OnPropertyChanged (() => IsLocationNameVisible);
			}
		}

		public int DiscoveryItemsCount => DiscoveryItems?.Count ?? 0;

		public bool IsDiscoveriesEmpty => DiscoveryItemsCount == 0;

		public ObservableCollection<DiscoveryItemViewModel> DiscoveryItems
		{
			get { return Get<ObservableCollection<DiscoveryItemViewModel>>(); }

			set
			{
				if (Set (value))
				{
					OnPropertyChanged (() => IsDiscoveriesListVisible);
					OnPropertyChanged (() => IsDiscoveriesEmpty);
				}
			}
		}

		public int NewDiscoveriesCount
		{
			get { return Get<int>(); }

			set
			{
				// if discoveries count changes and we have a discoveries menu item in play, update its count
				if (Set (value) && mDiscoveriesMenuItemViewModel != null)
					mDiscoveriesMenuItemViewModel.SuperCount = value;
			}
		}

		#endregion Discoveries

		#endregion Properties and Events

		#region Methods

		public async void UpdateLocationList()
		{
			try
			{
				using (var client = new HttpClient())
				{
                    var resultJson = await client.GetStringAsync(
                            App.BaseUrl + mLocationApiFormat.Replace("{term}", SearchTerm));

                    if (!string.IsNullOrEmpty (resultJson))
					{
						var model = JsonConvert.DeserializeObject<TownFishLocationList> (resultJson);
						if (model?.Items != null)
							LocationSearchItems = new ObservableCollection<TownfishLocationItem> (model.Items);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine ($"BrowserPageViewModel.UpdateLocationList: {ex.Message}");

				CancelLocationSearch();
			}
		}

		public void SetLocation (string cityID)
		{
			OnNavigateRequested (App.BaseUrl + mLocationSetFormat.Replace ("{id}", cityID) + App.QueryString);
		}

		public void LoadMenuMap (TownFishMenuMap map)
		{
			// clear everything so we only need to populate what's present
			ClearMenus();

			mLocationApiFormat = map.LocationsAPI;
			mLocationSetFormat = map.LocationSetUrl;

			SyncToken = map.SyncToken;

			try
			{
				MenuBarBackgroundColour = Color.FromHex ("#" + map.StatusBarBackgroundColor);
				MenuBarTextColour = Color.FromHex ("#" + map.StatusBarTextColor);
			}
			catch { } // don't care if/why it fails as we'll just use default property values

			LoadMenus (map.Menus);

			// now we've parsed the menu map we can set location properties, if needed
			if (IsTopBarVisible)
			{
				var locationMenuItem = (from m in map.Menus.Values
										where m.Position == "top"
										select m.Items.FirstOrDefault (i => i.Type == "locationpin"))
										.FirstOrDefault();

				if (locationMenuItem != null)
				{
					var size = "mdpi"; // HACK: force sizes for now; should be locationMenuItem.Size;
					var colour = locationMenuItem.Color;

					CurrentLocation = map.CurrentLocation;

					// set this now that we know it
					LocationName = WebUtility.HtmlDecode (CurrentLocation.Name);

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
			SearchTerm = "";
		}

		public ICommand GenerateMenuAction (TownFishMenuItem item)
		{
			switch (item.Type)
			{
				case "locationpin":
					return new Command (_ =>
						OnLocationTapped());

				case "callback":
				case "nativeCallback":
                    return new Command(_ =>
                        OnCallbackRequested(item.Name, item.Type == "nativeCallback"));
				// # date stamp no longer needed, so link is now same as back
				//case "link":
				//	return new Command (_ =>
				//		{ OnNavigateRequested ( App.BaseUrl + item.Href + App.QueryString + "#" + DateTime.Now.Ticks;) }); // TODO: remove nav hash!

				case "link":
				case "back":
					return new Command (_ =>
						{ OnNavigateRequested (App.BaseUrl + item.Href + App.QueryString); });

				case "noop":
					return sNoOpCommand;

				default:
					return null;
			}
		}

		void OnMenusLoaded()
		{
			MenusLoaded?.Invoke (this, EventArgs.Empty);
		}

		void OnLocationTapped()
		{
			CancelLocationSearch();

			LocationTapped?.Invoke (this, EventArgs.Empty);
		}

		void OnCallbackRequested (string callbackName, bool isNative)
		{
			CallbackRequested?.Invoke (this, new CallbackInfo
					{ Name = callbackName, IsNative = isNative });
		}

		/// <summary>
		/// Informs listeners that a navigation is requested.
		/// </summary>
		/// <remarks>
		/// Listeners MUST set SourceUrl to the given URL in order for navigation to happen.
		/// </remarks>
		/// <param name="url">The URL.</param>
		void OnNavigateRequested (string url)
		{
			NavigateRequested?.Invoke (this, url);
		}

		void ClearMenus()
		{
			LocationName = "";
			PageTitle = "";

			TopBarLeftLabel = "";
			TopBarLeftCommand = null;
			TopBarRightLabel = null;
			TopBarRightCommand = null;
			TopBarRightIcon = null;
			TopBarRight1Label = null;
			TopBarRight1Command = null;
			TopBarRight1Icon = null;

			TopAction1Label = "";
			TopAction1Command = null;
			TopAction2Label = "";
			TopAction2Command = null;
			TopAction3Label = "";
			TopAction3Command = null;
			TopAction4Label = "";
			TopAction4Command = null;
			TopActionMoreLabel = "";
			OverflowImages = null;

			TopFormLeftActionLabel = "";
			TopFormLeftActionCommand = null;
			TopFormRightActionLabel = "";
			TopFormRightActionCommand = null;
		}

		void LoadMenus (TownFishMenuList menus)
		{
			if (menus == null)
				return;

			var top = menus.Top;
			IsTopBarVisible = top?.IsVisible ?? false;

			if (IsTopBarVisible)
			{
				var topLeftItem = top.Items.FirstOrDefault (i => i.Align == "left");
				if (topLeftItem != null)
				{
					TopBarLeftLabel = GenerateMenuItem (top.Items [0]);
					TopBarLeftCommand = GenerateMenuAction (top.Items [0]);
				}

				var isHeadingItem = top.Items.FirstOrDefault (i => i.Type == "heading");
				if (isHeadingItem != null)
					PageTitle = GenerateMenuItem (top.Items [1]);

				var rightItems = top.Items.Where (i => i.Align == "right").ToList();
				if (rightItems.Count > 0)
				{
					TopBarRight1Label = new MenuLabelModel (rightItems [0]);
					TopBarRight1Command = GenerateMenuAction (rightItems [0]);
					TopBarRight1Icon = new MenuIconModel (rightItems [0]);
				}

				if (rightItems.Count > 1)
				{
					TopBarRightLabel = TopBarRight1Label;
					TopBarRightCommand = TopBarRight1Command;
					TopBarRightIcon = TopBarRight1Icon;

					TopBarRight1Label = new MenuLabelModel (rightItems [1]);
					TopBarRight1Command = GenerateMenuAction (rightItems [1]);
					TopBarRight1Icon = new MenuIconModel (rightItems [1]);
				}
			}

			var topSub = menus.TopSub;
			IsTopSubBarVisible = topSub?.IsVisible ?? false;

			if (IsTopSubBarVisible)
			{
				// Top Menu
				if (topSub.Items.Count > 0 && topSub.Items [0] != null)
				{
					TopAction1Label = GenerateMenuItem (topSub.Items [0]);
					TopAction1Command = GenerateMenuAction (topSub.Items [0]);

					if (topSub.Items [0].Highlight)
						TopAction1Bold = FontAttributes.Bold;
					else
						TopAction1Bold = FontAttributes.None;
				}

				if (topSub.Items.Count > 1 && topSub.Items [1] != null)
				{
					TopAction2Label = GenerateMenuItem (topSub.Items [1]);
					TopAction2Command = GenerateMenuAction (topSub.Items [1]);

					if (topSub.Items [1].Highlight)
						TopAction2Bold = FontAttributes.Bold;
					else
						TopAction2Bold = FontAttributes.None;
				}

				if (topSub.Items.Count > 2 && topSub.Items [2] != null)
				{
					TopAction3Label = GenerateMenuItem (topSub.Items [2]);
					TopAction3Command = GenerateMenuAction (topSub.Items [2]);

					if (topSub.Items [2].Highlight)
						TopAction3Bold = FontAttributes.Bold;
					else
						TopAction3Bold = FontAttributes.None;
				}

				if (topSub.Items.Count > 3 && topSub.Items [3] != null)
				{
					TopAction4Label = GenerateMenuItem (topSub.Items [3]);
					TopAction4Command = GenerateMenuAction (topSub.Items [3]);

					if (topSub.Items [3].Highlight)
						TopAction4Bold = FontAttributes.Bold;
					else
						TopAction4Bold = FontAttributes.None;
				}

				if (topSub.Items.Count > 4)
				{
					var moreIcon = topSub.Items.FirstOrDefault (i => i.Type == "limitby");
					if (moreIcon != null)
					{
						TopActionMoreLabel = GenerateMenuItem (moreIcon);
						OverflowImages = topSub.Items.Skip (3).Where (i => i.Type != "limitby").ToList();
					}
				}
			}

			var topForm = menus.TopForm;
			IsTopFormBarVisible = topForm?.IsVisible ?? false;

			if (IsTopFormBarVisible)
			{
				TopFormLeftActionLabel = GenerateMenuItem (topForm.Items [0]);
				TopFormLeftActionCommand = GenerateMenuAction (topForm.Items [0]);

				var topFormRightItem = topForm.Items.FirstOrDefault (i => i.Align == "right");
				if (topFormRightItem != null)
				{
					TopFormRightActionLabel = GenerateMenuItem (topFormRightItem);
					TopFormRightActionCommand = GenerateMenuAction (topFormRightItem);
				}

				// if 3 items, item 1 is title (we hope!)
				if (topForm.Items.Count > 2 && topForm.Items [1] != null)
					PageTitle = GenerateMenuItem (topForm.Items [1]);
			}

			var bottom = menus.Bottom;
			IsBottomBarVisible = bottom?.IsVisible ?? false;

			if (IsBottomBarVisible)
			{
				var actions = new ObservableCollection<BottomActionViewModel>();

                int totalSuperCount = 0;

				foreach (var item in bottom.Items)
				{
                    int superCount = GetSuperNumber(item);
                    totalSuperCount += superCount;
					var action = new BottomActionViewModel
					{
						MainViewModel = this,
						Icon = GenerateMenuItem (item),
						Command = GenerateMenuAction (item),
						SuperCount = superCount,
						SuperColour = item.SuperColor,
					};

					// remember this in case we need to update its number
					if (item.SuperFormat == cDiscoveriesCountFormat)
						mDiscoveriesMenuItemViewModel = action;

					actions.Add (action);
				}

				// HACK: view expects 6 items (hard-coded in a grid!!), so pad as necessary
				for (var i = bottom.Items.Count; i++ < 6; )
					actions.Add (new BottomActionViewModel());

                var shAnalytics = DependencyService.Get<IStreetHawkAnalytics>();

                shAnalytics.TagNumeric("sh_badge_number", totalSuperCount);

                BottomActions = actions;
			}
		}

		int GetSuperNumber (TownFishMenuItem item)
		{
			var number = 0;

			if (item.SuperFormat == cDiscoveriesCountFormat)
				number = NewDiscoveriesCount;
			else
				int.TryParse (item.Super, out number);

			return number;
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

		#endregion Methods

		#region Fields

		const string cDiscoveriesCountFormat = "{DiscoveriesCount}";

		static Command sNoOpCommand = new Command (_ => {});

		public static string CurrentSearchTerm = "";

		string mLocationApiFormat = "";
		string mLocationSetFormat = "";

		BottomActionViewModel mDiscoveriesMenuItemViewModel;

		#endregion Fields
	}
}
