using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows.Input;

using Xamarin.Forms;

using Newtonsoft.Json;

using TownFish.App.Models;


namespace TownFish.App.ViewModels
{
	public class BrowserPageViewModel: ViewModelBase
	{
		#region Helper Types

		public class MenuIconModel
		{
			#region Construction

			public MenuIconModel (TownFishMenuItem item)
			{
				if (item == null)
					throw new ArgumentNullException ("item can not be null");

				mItem = item;
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
				if (item == null)
					throw new ArgumentNullException ("item can not be null");

				mItem = item;
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

		#endregion Helper Types

		#region Properties and Events

		public event EventHandler LocationTapped;

		public event EventHandler<string> CallbackRequested;

		public event EventHandler<string> MenusLoaded;

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

		public Color FeedLinkColour
		{
			get { return Get (() => Color.FromHex ("#5099f7")); }
			private set { Set (value); }
		}

		public int FeedCount
		{
			get { return Get<int>(); }

			set
			{
				// if feed count changes and we have a feed menu item in play, update its count
				if (Set (value) && mFeedMenuItemViewModel != null)
					mFeedMenuItemViewModel.SuperCount = value;
			}
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
			private set { Set (value); }
		}

		public bool IsTopBarVisible
		{
			get { return Get<bool>(); }
			set { Set (value); }
		}

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

		#region Feed

		public bool IsFeedVisible
		{
			get { return Get<bool>(); }
			set { Set (value); }
		}

		public bool IsFeedListVisible => !IsFeedEmpty;

		public bool IsFeedInfoVisible
		{
			get { return Get<bool>(); }
			set { Set (value); }
		}

		public bool IsFeedEmpty => (FeedItems?.Count ?? 0) == 0;

		public ObservableCollection<FeedItemViewModel> FeedItems
		{
			get { return Get<ObservableCollection<FeedItemViewModel>>(); }

			set
			{
				if (Set (value))
				{
					OnPropertyChanged (() => IsFeedListVisible);
					OnPropertyChanged (() => IsFeedEmpty);
				}
			}
		}

		#endregion Feed

		#endregion Properties and Events

		#region Methods

		public async void UpdateLocationList()
		{
			try
			{
				using (var client = new HttpClient())
				{
					var resultJson = await client.GetStringAsync (
							App.BaseUrl + mLocationApiFormat.Replace ("{term}", SearchTerm));

					if (!string.IsNullOrEmpty (resultJson))
					{
						var model = JsonConvert.DeserializeObject<TownFishLocationList> (resultJson);
						if (model?.Items != null)
							LocationSearchItems = new ObservableCollection<TownfishLocationItem> (model.Items);
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

		public void ClearMenus()
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
				var locationMenuItem = map.Menus.Top.items.FirstOrDefault (i => i.Type == "locationpin");
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

		void OnMenusLoaded()
		{
			MenusLoaded?.Invoke (this, "");
		}

		void OnLocationTapped()
		{
			CancelLocationSearch();

			LocationTapped?.Invoke (this, EventArgs.Empty);
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

				var isHeadingItem = top.items.FirstOrDefault (i => i.Type == "heading");
				if (isHeadingItem != null)
					PageTitle = GenerateMenuItem (top.items [1]);

				var rightItems = top.items.Where (i => i.Align == "right").ToList();
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

				if (topSub.items.Count > 1 && topSub.items [1] != null)
				{
					TopAction2Label = GenerateMenuItem (topSub.items [1]);
					TopAction2Command = GenerateMenuAction (topSub.items [1]);

					if (topSub.items [1].Highlight)
						TopAction2Bold = FontAttributes.Bold;
					else
						TopAction2Bold = FontAttributes.None;
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

				if (topSub.items.Count > 3 && topSub.items [3] != null)
				{
					TopAction4Label = GenerateMenuItem (topSub.items [3]);
					TopAction4Command = GenerateMenuAction (topSub.items [3]);

					if (topSub.items [3].Highlight)
						TopAction4Bold = FontAttributes.Bold;
					else
						TopAction4Bold = FontAttributes.None;
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
			}

			var topForm = menus.TopForm;
			IsTopFormBarVisible = topForm != null && topForm.display && topForm.items.Count > 0;

			if (IsTopFormBarVisible)
			{
				TopFormLeftActionLabel = GenerateMenuItem (topForm.items [0]);
				TopFormLeftActionCommand = GenerateMenuAction (topForm.items [0]);

				var topFormRightItem = topForm.items.FirstOrDefault (i => i.Align == "right");
				if (topFormRightItem != null)
				{
					TopFormRightActionLabel = GenerateMenuItem (topFormRightItem);
					TopFormRightActionCommand = GenerateMenuAction (topFormRightItem);
				}

				// if 3 items, item 1 is title (we hope!)
				if (topForm.items.Count > 2 && topForm.items [1] != null)
					PageTitle = GenerateMenuItem (topForm.items [1]);
			}

			var bottom = menus.Bottom;
			IsBottomBarVisible = bottom != null && bottom.display && bottom.items?.Count > 0;

			if (IsBottomBarVisible)
			{
				var actions = new ObservableCollection<BottomActionViewModel>();

				for (var i = 0; i < bottom.items.Count; i++)
				{
					var item = bottom.items [i];
					var action = new BottomActionViewModel
					{
						MainViewModel = this,
						Icon = GenerateMenuItem (item),
						Command = GenerateMenuAction (item),
						SuperCount = GetSuperNumber (item),
						SuperColour = item.SuperColor,
					};

					// remember this in case we need to update its number
					if (item.SuperFormat == cFeedCountFormat)
						mFeedMenuItemViewModel = action;

					actions.Add (action);
				}

				// view expects 6 items (hard-coded in a grid!!), so pad as necessary
				for (var i = bottom.items.Count; i++ < 6; )
					actions.Add (new BottomActionViewModel());

				BottomActions = actions;
			}
		}

		int GetSuperNumber (TownFishMenuItem item)
		{
			var number = 0;

			if (item.SuperFormat == cFeedCountFormat)
				number = FeedCount;
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

		ICommand GenerateMenuAction (TownFishMenuItem item)
		{
			switch (item.Type)
			{
				case "locationpin":
					return new Command (_ =>
						OnLocationTapped());

				case "callback":
					return new Command (_ =>
						OnCallbackRequested (item.Name));

				// # date stamp no longer needed, so link is now same as back
				//case "link":
				//	return new Command (_ =>
				//		{ SourceUrl = App.BaseUrl + item.Href + App.QueryString + "#" + DateTime.Now.Ticks; }); // TODO: remove nav hash!

				case "link":
				case "back":
					return new Command (_ =>
						{ SourceUrl = App.BaseUrl + item.Href + App.QueryString; });

				case "noop":
					return sNoOpCommand;

				default:
					return null;
			}
		}

		#endregion Methods

		#region Fields

		const string cFeedCountFormat = "{DiscoveriesCount}";

		static Command sNoOpCommand = new Command (_ => {});

		public static string CurrentSearchTerm = "";

		string mLocationApiFormat = "";
		string mLocationSetFormat = "";

		BottomActionViewModel mFeedMenuItemViewModel;

		#endregion Fields
	}
}
