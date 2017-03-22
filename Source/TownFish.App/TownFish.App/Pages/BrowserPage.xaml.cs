//#define EMPTY_TOP_MENU_WHEN_LOADING

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;

using Newtonsoft.Json;

using Apptelic.UberWebViewLib;

using TownFish.App.Models;
using TownFish.App.ViewModels;


namespace TownFish.App.Pages
{
	public partial class BrowserPage : ContentPage
	{
		#region Construction

		public BrowserPage()
		{
			InitializeComponent();

			// set my user agent string
			wbvContent.CustomUserAgent = cCustomUserAgent;

			// set up JS bridge action scripts
			wbvContent.PageLoadedScript = cPageLoadScript;

			wbvContent.Actions = new Dictionary<string, string>
			{
				{ cBeginLoadingAction, cBeginLoadingValue },
				{ cSchemaAction, cSchemaMethod },
				{ cMenusVisibleAction, cMenusVisibleMethod },
				{ cPopupMenuAction, cPopupMenuMethod },
				{ cPushAction, cPushMethod },
				{ cFeedShowAction, cFeedShowValue },
				{ cFeedHideAction, cFeedHideValue }
			};

			wbvContent.ScriptMessageReceived += UberWebView_ScriptMessageReceived;
			wbvContent.NavigationStarting += UberWebView_NavigationStarting;
			wbvContent.NavigationStarted += UberWebView_NavigationStarted;
			wbvContent.NavigationFinished += UberWebView_NavigationFinished;
			wbvContent.NavigationFailed += UberWebView_NavigationFailed;

			wbvContent.QueryParamDomain = App.SiteDomain;
			wbvContent.QueryParam = App.QueryParam;

			// on iOS we have to allow for the overlapping status bar at top of layout
			if (Device.RuntimePlatform == Device.iOS)
			{
				var pad = pnlTopMenuBar.Padding;
				pad.Top += cTopPaddingiOS;
				pnlTopMenuBar.Padding = pad;

				pad = pnlTopForm.Padding;
				pad.Top += cTopPaddingiOS;
				pnlTopForm.Padding = pad;
			}
		}

		#endregion Construction

		#region Methods

		protected override void OnAppearing()
		{
			App.Current.BackButtonPressed += App_BackButtonPressed;
			App.Current.PushUrlReceived += App_PushUrlReceived;
			App.Current.SHFeedItemsAvailable += App_SHFeedItemsAvailable;
			App.Current.AppResumed += App_Resumed;

			base.OnAppearing();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			App.Current.BackButtonPressed -= App_BackButtonPressed;
			App.Current.PushUrlReceived -= App_PushUrlReceived;
			App.Current.SHFeedItemsAvailable -= App_SHFeedItemsAvailable;
			App.Current.AppResumed -= App_Resumed;
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			if (ViewModel == null)
				return;

			// manually set this as span isn't bindable
			spnMemberAgreementLink.ForegroundColor = ViewModel.FeedLinkColour;

			// Because we use DisplayActionSheet
			ViewModel.TopActionMoreCommand = new Command (async _ =>
				{
					var selection = await DisplayActionSheet (cMoreActions, cCancel, null,
							ViewModel.OverflowImages.Select (i => i.Value).ToArray<string>());

					var url = ViewModel.OverflowImages.FirstOrDefault (i => i.Value == selection)?.Href;
					if (!string.IsNullOrWhiteSpace (url))
						Navigate (App.BaseUrl + url + App.QueryString);
				});

			ViewModel.LocationTapped += ViewModel_LocationTapped;
			ViewModel.CallbackRequested += ViewModel_CallbackRequested;
			ViewModel.MenusLoaded += ViewModel_MenusLoaded;
			ViewModel.NavigateRequested += ViewModel_NavigateRequested;

			// now start at the specified source URL
			Navigate (ViewModel.SourceUrl);
		}

		protected override bool OnBackButtonPressed()
		{
			// Hello! When did this appear?!

			return base.OnBackButtonPressed();
		}

		void App_BackButtonPressed (object sender, EventArgs e)
		{
			if (mIsSearchVisible || mShowingSearch)
				HideSearchPanel();
			else if (ViewModel.IsFeedVisible)
				HideFeed();
			else if (wbvContent.CanGoBack)
				wbvContent.GoBack();
			else
				App.Current.CloseApp();
		}

		void App_PushUrlReceived (object sender, string url)
		{
			Navigate (url);
		}

		private void App_Resumed (object sender, EventArgs e)
		{
			// in case we're returning from a remote URL, remember what our last URL really was
			if (!string.IsNullOrEmpty (mLastSourceUrl))
				ViewModel.SourceUrl = mLastSourceUrl; // don't use Navigate() here!

			// and make sure menu is correct
			//RestoreTopMenu();
		}

		async void UberWebView_ScriptMessageReceived (object sender, string msg)
		{
			string action = null;
			string result = null;

			try
			{
				var model = JsonConvert.DeserializeObject<UberWebViewMessage>(msg);

				action = model.Action;
				result = model.Result;

				Debug.WriteLine ("WebViewAction: Parsing {0}:\n{1}", action, result);

				switch (action)
				{
					case cBeginLoadingAction:
						ShowLoading();
						break;

					case cSchemaAction:
						mCurrentMenuMap = JsonConvert.DeserializeObject<TownFishMenuMap> (result);
						ViewModel.LoadMenuMap (mCurrentMenuMap);

						break;

					case cMenusVisibleAction:
						if (mCurrentMenuMap == null)
							break;

						var visibleMenus = JsonConvert.DeserializeObject<string[]> (result);
						mCurrentMenuMap.SetMenuVisibility (visibleMenus);

						// now reload with new visibility settings
						ViewModel.LoadMenuMap (mCurrentMenuMap);

						break;

					case cPopupMenuAction:
						var menu = JsonConvert.DeserializeObject<List<TownFishMenuItem>> (result);

						var selection = await DisplayActionSheet (cMoreActions, cCancel, null,
								menu.Select (i => i.Value).ToArray());

						var selectedItem = menu.FirstOrDefault (i => i.Value == selection);
						if (selectedItem?.Type == cCallbackType)
							ViewModel_CallbackRequested (this, selectedItem.Name);

						break;

					case cPushAction:
						break;

					case cFeedShowAction:
						try { ShowFeed(); }
						catch (Exception ex) {}

						break;

					case cFeedHideAction:
						// HACK: ignore hide feed action to 'fix' Item 20 in Basecamp bug list
						//HideFeed();
						break;
				}
			}
			catch (Exception e)
			{
				// TODO: parse failed, so use a default? go to offline page?
				Debug.WriteLine ("WebViewAction: Parse of:\n{0}\nfailed with error:\n{1}", msg, e.Message);

				// if it was the schema, just hide everything as it might contain rubbish
				if (action == cSchemaAction)
				{
					ViewModel.IsTopBarVisible = false;
					ViewModel.IsTopSubBarVisible = false;
					ViewModel.IsTopFormBarVisible = false;
					ViewModel.IsBottomBarVisible = false;

					HideLoading();
				}
			}
		}

		void UberWebView_NavigationStarting (object sender, WebNavigatingEventArgs e)
		{
			// launch non-TownFish URLs in external browser
			var uri = new Uri (e.Url);
			if (uri.Host != App.SiteDomain && uri.Host != App.TwitterApiDomain)
			{
				e.Cancel = true;

				Device.OpenUri (uri);

				return;
			}
		}

		void UberWebView_NavigationStarted (object sender, string url)
		{
			ShowLoading();
		}

		void UberWebView_NavigationFinished (object sender, string url)
		{
			// in case URL is changed by webview itself, save it here so we now what it is
			ViewModel.SourceUrl = url; // don't use Navigate() here!

			/* TODO: Paul said the menus should be hidden at the start of each
			   page load, but this gives a very 'flashy' experience (and not in
			   a good way), so I'm removing it for now

			// if we're showing menus, hide them all and then kill the schema
			if (mCurrentSchema != null)
			{
				mCurrentSchema.SetMenuVisibility (null);
				ViewModel.LoadMenuMap (mCurrentSchema);

				mCurrentSchema = null;
			}
			*/

			var uri = new Uri (url);
			if (uri.Host != App.SiteDomain)
			{
				// for our site, don't reveal page until menu has been rendered,
				// to avoid the 'jump' down problem; other sites show immediately as
				// obviously there's no menu on them!

				HideLoading();
			}
		}

		void UberWebView_NavigationFailed (object sender, UberWebView.NavigationException ex)
		{
			// HACK! ignore iOS -999 (nav cancelled?) error; appears harmless
			if (Device.RuntimePlatform == Device.iOS && ex.ErrorCode == -999)
				return;

			HideLoading();

#if DEBUG
			var msg = ex.Message;
#else
			var msg = "Please try again or go to a different page";
#endif
			App.Current.MainPage.DisplayAlert ("TownFish", string.Format (
					"Failed to load the page.\n\n{0}", msg), "Cancel");
		}

		void lstLocationSearchResults_ItemTapped (object sender, ItemTappedEventArgs e)
		{
			HideSearchPanel();

			ViewModel.SetLocation ((e.Item as TownfishLocationItem).CityID);
		}

		void lstAvailableLocations_ItemTapped (object sender, ItemTappedEventArgs e)
		{
			HideSearchPanel();

			ViewModel.SetLocation ((e.Item as AvailableLocation).ID);
		}

		void lstFeed_ItemTapped (object sender, EventArgs e)//ItemTappedEventArgs e)
		//async void lstFeed_ItemTapped (object sender, ItemTappedEventArgs e)
		{
			// re: Bug Item 16 - don't return to main page; stay on message feed when item tapped
			//await HideFeedAsync();

			// when using ListView:
			//var fivm = e.Item as FeedItemViewModel;

			// when using StackView:
			var feedItem = sender as FeedItem;
			var fivm = feedItem.BindingContext as FeedItemViewModel;

			if (fivm != null)
				Navigate (fivm.LinkUrl);
		}

		/// <summary>
		/// Sets SH feed item count and updates feed item list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name=""></param>
		void App_SHFeedItemsAvailable (object sender, int newCount)
		{
			ViewModel.FeedCount = newCount;

			var feedItems = App.SHFeedItems;
			var count = App.SHFeedItemCount;

			// if no items, kill previous list
			if (count == 0)
				ViewModel.FeedItems = null;
			else
				ViewModel.FeedItems = new ObservableCollection<FeedItemViewModel> (feedItems);

			// HACK: as iOS ListView isn't working, manually generate content for a StackLayout
			Device.BeginInvokeOnMainThread (() =>
				{
					var kids = lstFeed.Children;
					kids.Clear();

					for (var i = 0; i < count; i++)
					{
						var item = new FeedItem { BindingContext = feedItems [i] };
						item.Tapped += lstFeed_ItemTapped;

						kids.Add (item);
					}
				});
		}

		async void BrowserPage_EditLikesTapped (object sender, EventArgs e)
		{
			await HideFeedAsync();

			Navigate (App.EditLikesUrl + App.QueryString);
		}

		async void BrowserPage_EditProfileTapped (object sender, EventArgs e)
		{
			await HideFeedAsync();

			Navigate (App.EditProfileUrl + App.QueryString);
		}

		void BrowserPage_MemberAgreementTapped (object sender, EventArgs e)
		{
			HideSearchPanel();

			Navigate (App.TermsUrl + App.QueryString);
		}

		void ViewModel_MenusLoaded (object sender, EventArgs e)
		{
			// only hide loading if already showing it but not in the process of hiding feed
			if (ViewModel.IsLoading && !mHidingFeed)
				Device.BeginInvokeOnMainThread (() => HideLoading());

			// tell app to check sync token in case login changed
			// (i.e. user logged out, or new user logged in)
			App.CheckCuid (ViewModel.SyncToken);
		}

		void ViewModel_LocationTapped (object sender, EventArgs e)
		{
			if (!mIsSearchVisible && !mShowingSearch)
				ShowSearchPanel();
			else
				HideSearchPanel();
		}

		void ViewModel_CallbackRequested (object sender, string name)
		{
			// special-case search within SH Message Feed
			//if (name == cFeedSearch)
			//{
			//	// TODO: search in message feed
			//	return;
			//}

			// special-case info icon within SH Message Feed
			if (name == cFeedInfo)
			{
				ViewModel.IsFeedInfoVisible = ViewModel.IsFeedEmpty ||
						!ViewModel.IsFeedInfoVisible;

				return;
			}

			HideSearchPanel();

			// every non-feed callback action results in feed being hidden and a new page loading
			//var feedAction = name == cFeedButton || name == cFeedInfo;
			// TODO: is this necessary when all bottom icons (except feed) are links, not
			// callbacks? Currently, it doesn't seem so.
			//if (!feedAction)
			//	HideFeed (showLoading: ViewModel.IsFeedVisible);

			Device.BeginInvokeOnMainThread (() =>
				wbvContent.InvokeScript (string.Format (cCallbackFormat, name)));
		}

		void ViewModel_NavigateRequested (object sender, string url)
		{
			Navigate (url);
		}

		void Navigate (string url)
		{
			// if it's a new TF URL hide the feed and save URL for returning to later
			if (url.StartsWith (App.BaseUrl))
			{
				// hide feed and, if it's a new URL, show loading
				HideFeed (showLoading: url != ViewModel.SourceUrl);

				// save this in case we come back later from an external URL
				mLastSourceUrl = url;
			}

			// this should be enough on its own...
			ViewModel.SourceUrl = url;

			// ... but WebView Source binding is unreliable, so we also do it manually
			//wbvContent.Source = url;
		}

		void ShowFeed()
		{
			if (ViewModel.IsFeedVisible)
				return;

			// TODO: schema should really give us this on show feed callback
			// set top menu for feed
			mCurrentTopMenu = mCurrentMenuMap.Menus.Top;
			mCurrentFormMenu = mCurrentMenuMap.Menus.TopForm;
			mCurrentMenuMap.Menus.Top = sFeedTopMenu;
			mCurrentMenuMap.Menus.TopForm = null;

			ViewModel.LoadMenuMap (mCurrentMenuMap);

			ViewModel.IsFeedInfoVisible = ViewModel.IsFeedEmpty;
			ViewModel.IsFeedVisible = true;

			// now we've shown it, reset count
			ViewModel.FeedCount = 0;
		}

		/// <summary>
		/// Fire-and-forget feed hider.
		/// </summary>
		async void HideFeed (bool showLoading = true)
		{
			await HideFeedAsync (showLoading);
		}

		/// <summary>
		/// Awaitable feed hider.
		/// </summary>
		/// <returns></returns>
		async Task HideFeedAsync (bool showLoading = true)
		{
			if (ViewModel.IsFeedVisible && !mHidingFeed)
			{
				mHidingFeed = true;

				if (showLoading)
					await ShowLoadingAsync(); // wait for it to show before hiding feed

				// clear & hide feed
				ViewModel.IsFeedVisible = false;
				ViewModel.FeedItems = null; // side-effect: updates visibility properties!

				// if we're not showing loading we're leaving feed, so just restore menu
				if (!showLoading)
					RestoreTopMenu();

				mHidingFeed = false;
			}
		}

		void RestoreTopMenu()
		{
			if (mCurrentMenuMap != null && (mCurrentTopMenu != null || mCurrentFormMenu != null))
			{
				mCurrentMenuMap.Menus.Top = mCurrentTopMenu;
				mCurrentMenuMap.Menus.TopForm = mCurrentFormMenu;
				ViewModel.LoadMenuMap (mCurrentMenuMap);
			}
		}

		async void ShowSearchPanel()
		{
			if (mIsSearchVisible || mShowingSearch)
				return;

			mShowingSearch = true;

			// first, kill any existing animations
			ViewExtensions.CancelAnimations (pnlLocations);
			ViewExtensions.CancelAnimations (pnlTopSearch);

			// first time through we need to put panels in their initial positions
			if (mFirstShowing)
			{
				mFirstShowing = false;

				pnlLocations.TranslationY = -pnlLocations.Height;
				pnlTopSearch.TranslationX = -pnlTopSearch.Width;

				// now we have them properly positioned, we can reveal all below!
			}

			mIsSearchVisible = true;

			var locXlate = pnlLocations.TranslateTo (0, 0, cLocationPanelAnimationTime, Easing.CubicInOut);
			var tsXlate = pnlTopSearch.TranslateTo (0, 0, cLocationPanelAnimationTime, Easing.CubicInOut);

			await Task.WhenAll (locXlate, tsXlate);

			mShowingSearch = false;
		}

		async void HideSearchPanel()
		{
			if (!mIsSearchVisible || mHidingSearch)
				return;

			if (ViewModel.SearchLocationActive)
				ViewModel.CancelLocationSearch();

			// remove focus from search input
			wbvContent.Focus();

			mHidingSearch = true;

			// first, kill any existing animations
			ViewExtensions.CancelAnimations (pnlLocations);
			ViewExtensions.CancelAnimations (pnlTopSearch);

			var locXlate = pnlLocations.TranslateTo (0, -pnlLocations.Height, cLocationPanelAnimationTime, Easing.CubicInOut);
			var tsXlate = pnlTopSearch.TranslateTo (-pnlTopSearch.Width, 0, cLocationPanelAnimationTime, Easing.CubicInOut);

			await Task.WhenAll (locXlate, tsXlate);

			mIsSearchVisible = false;
			mHidingSearch = false;
		}

		/// <summary>
		/// Fire-and-forget loading revealer.
		/// </summary>
		async void ShowLoading()
		{
			await ShowLoadingAsync();
		}

		/// <summary>
		/// Awaitable loading revealer.
		/// </summary>
		/// <returns></returns>
		async Task ShowLoadingAsync()
		{
			if (ViewModel.IsLoading)
				return;

#if EMPTY_TOP_MENU_WHEN_LOADING

			// show empty top menu
			if (mCurrentMenuMap != null)
			{
				mCurrentMenuMap.Menus.Top = sEmptyTopMenu;
				ViewModel.LoadMenuMap (mCurrentMenuMap);
			}
#endif

			// set this here in case page loads faster than loading animation completes,
			// which could result in loading never being removed
			ViewModel.IsLoading = true;

			if (!mFirstLoading)
			{
				pnlLoading.Opacity = 1;
				pnlLoading.TranslationX = pnlLoading.Width;
				await pnlLoading.TranslateTo (0, 0, cLoadingPanelAnimationTime, Easing.CubicInOut);
			}
		}

		async void HideLoading()
		{
			if (!ViewModel.IsLoading)
				return;

			await pnlLoading.FadeTo (0, cLoadingPanelAnimationTime, Easing.CubicIn);
			ViewModel.IsLoading = false;

			// make sure we only show splash loading once
			if (mFirstLoading)
			{
				mFirstLoading = false;
				imgSplash.IsVisible = false;
			}
		}

		#endregion Methods

		#region Properties

		public BrowserPageViewModel ViewModel => BindingContext as BrowserPageViewModel;

		#endregion Properties

		#region Fields

#if DEBUG
		const uint cLocationPanelAnimationTime = 250;
		const uint cLoadingPanelAnimationTime = 250;
#else
		const uint cLocationPanelAnimationTime = 250;
		const uint cLoadingPanelAnimationTime = 250;
#endif

		// apparently iOS status bar height is always 20 in XF (apparently, I said)
		const double cTopPaddingiOS = 20;

		const string cPageLoadScript = "if (window.twnfsh && twnfsh.appReady) twnfsh.appReady();";

		const string cBeginLoadingAction = "app_schema_loading";
		const string cBeginLoadingValue = "true";

		const string cSchemaAction = "app_schema_reload";
		const string cSchemaMethod = "twnfsh.getSchema()";

		const string cMenusVisibleAction = "app_schema_visible";
		const string cMenusVisibleMethod = "twnfsh.visibleMenus()";

		const string cPopupMenuAction = "app_schema_popup_menu";
		const string cPopupMenuMethod = "twnfsh.getPopupMenuSchema()";

		const string cPushAction = "app_schema_push_messages";
		const string cPushMethod = "twnfsh.getPushMessages()";

		const string cFeedShowAction = "app_schema_show_feed";
		const string cFeedShowValue = "true";
		const string cFeedHideAction = "app_schema_hide_feed";
		const string cFeedHideValue = "true";

		const string cCallbackFormat = "twnfsh.runCallback('{0}')";
		const string cCallbackType = "callback";

		const string cMoreActions = null; // e.g. "Please Select:";
		const string cCancel = "Cancel";

		const string cCustomUserAgent = "com.townfish.app";

		const string cFeedSearch = "FeedSearch";
		const string cFeedInfo = "FeedInfo";
		const string cFeedButton = "pushFeed";

		static List<TownFishMenuItem> sFeedTopMenuItems = new List<TownFishMenuItem>
		{
			new TownFishMenuItem // empty left icon
			{
				Align = "left",
				IconUrl = "/applications/profiles/design/menuicons/clear-hdpi.png",
				Kind = "icon",
				Size = "ldpi",
				Type = "noop",
				Value = ""
			},
			new TownFishMenuItem // title
			{
				Align = "middle",
				Main = "Discoveries",
				Type = "heading"
			},
			/*new TownFishMenuItem // search icon
			{
				Align = "right",
				IconUrl = "/applications/profiles/design/menuicons/magnifying-search-hdpi-ffffff.png",
				Kind = "icon",
				Name = cFeedSearch,
				Size = "ldpi",
				Type = "callback",
				Value = ""
			},*/
			new TownFishMenuItem // info icon
			{
				Align = "right",
				IconUrl = "/applications/profiles/design/menuicons/information-hdpi-ffffff.png",
				Kind = "icon",
				Name = cFeedInfo,
				Size = "ldpi",
				Type = "callback",
				Value = ""
			}
		};

		static TownFishMenu sFeedTopMenu = new TownFishMenu
		{
			display = true,
			items = sFeedTopMenuItems
		};

#if EMPTY_TOP_MENU_WHEN_LOADING

		static List<TownFishMenuItem> sEmptyTopMenuItems = new List<TownFishMenuItem>
		{
			new TownFishMenuItem // empty left icon
			{
				Align = "left",
				IconUrl = "/applications/profiles/design/menuicons/clear-hdpi.png",
				Kind = "icon",
				Size = "ldpi",
				Type = "noop",
				Value = ""
			},
			new TownFishMenuItem // title
			{
				Align = "middle",
				Main = " ",
				Type = "heading"
			},
			new TownFishMenuItem // info icon
			{
				Align = "right",
				IconUrl = "/applications/profiles/design/menuicons/clear-hdpi.png",
				Kind = "icon",
				Size = "ldpi",
				Type = "noop",
				Value = ""
			}
		};

		static TownFishMenu sEmptyTopMenu = new TownFishMenu
		{
			display = true,
			items= sEmptyTopMenuItems
		};

#endif //EMPTY_TOP_MENU_WHEN_LOADING

		TownFishMenuMap mCurrentMenuMap;
		TownFishMenu mCurrentTopMenu;
		TownFishMenu mCurrentFormMenu;

		bool mFirstLoading = true;
		bool mFirstShowing = true;

		bool mIsSearchVisible;
		bool mShowingSearch;
		bool mHidingSearch;
		bool mHidingFeed;

		string mLastSourceUrl;

		#endregion Fields
	}
}
