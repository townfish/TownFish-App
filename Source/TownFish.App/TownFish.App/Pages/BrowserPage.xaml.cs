#define STREETHAWKWORKING

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

using Newtonsoft.Json;

using Xamify.UberWebViewLib;

using StreetHawkCrossplatform;

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
			if (Device.OS == TargetPlatform.iOS)
			{
				var pad = pnlTopMenuBar.Padding;
				pad.Top += cTopPaddingiOS;
				pnlTopMenuBar.Padding = pad;

				pad = pnlTopForm.Padding;
				pad.Top += cTopPaddingiOS;
				pnlTopForm.Padding = pad;
			}

			gstLocationImage.Tapped += LocationTapped;
			gstLocationLabel.Tapped += LocationTapped;
		}

		#endregion Construction

		#region Methods

		protected override void OnAppearing()
		{
			// make sure we shrink for keyboard so inputs stay visible
			App.Current.On<Android>().UseWindowSoftInputModeAdjust (WindowSoftInputModeAdjust.Resize);

			App.Current.BackButtonPressed += App_BackButtonPressed;
			App.Current.PushUrlReceived += App_PushUrlReceived;

			base.OnAppearing();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			App.Current.BackButtonPressed -= App_BackButtonPressed;
			App.Current.PushUrlReceived -= App_PushUrlReceived;
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			if (ViewModel == null)
				return;

			// start in loading state
			ShowLoading();

			// manually set this as span isn't bindable
			spnMemberAgreementLink.ForegroundColor = ViewModel.LocationsLinkColour;

			// Because we use DisplayActionSheet
			ViewModel.TopActionMoreCommand = new Command (async _ =>
				{
					var selection = await DisplayActionSheet (cMoreActions, cCancel, null,
							ViewModel.OverflowImages.Select (i => i.Value).ToArray<string>());

					var url = ViewModel.OverflowImages.FirstOrDefault (i => i.Value == selection)?.Href;
					if (!string.IsNullOrWhiteSpace (url))
						ViewModel.SourceUrl = App.BaseUrl + url + App.QueryString;
				});

			ViewModel.CallbackRequested += (s, name) => OnCallback (name);
			ViewModel.MenusLoaded += ViewModel_MenusLoaded;
			ViewModel.PropertyChanged += ViewModel_PropertyChanged;

			lstLocationSearchResults.ItemTapped += lstLocationSearchResults_ItemTapped;
			lstAvailableLocations.ItemTapped += lstAvailableLocations_ItemTapped;
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
			else if (wbvContent.CanGoBack)
				wbvContent.GoBack();
			else
				App.Current.CloseApp();
		}

		void App_PushUrlReceived (object sender, string url)
		{
			ViewModel.SourceUrl = url;
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
							OnCallback (selectedItem.Name);

						break;

					case cPushAction:
						break;

					case cFeedShowAction:
						try
						{
							var feedItems = await App.GetSHFeed();
							ShowFeed (feedItems);
						}
						catch //(Exception ex)
						{
							ShowFeed (null);
						}
						break;

					case cFeedHideAction:
						HideFeed();
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
			if (Device.OS == TargetPlatform.iOS && ex.ErrorCode == -999)
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

		void ViewModel_PropertyChanged (object sender, PropertyChangedEventArgs pcea)
		{ 
			// WebView Source binding is unreliable, so we do it manually here
			if (pcea.PropertyName == "SourceUrl")
				wbvContent.Source = ViewModel.SourceUrl;
		}

		void OnMemAgreeClicked (object sender, EventArgs e)
		{
			HideSearchPanel();

			ViewModel.SourceUrl = App.TermsUrl + App.QueryString;
		}

		void LocationTapped (object sender, EventArgs e)
		{
			if (ViewModel.LeftActionIsLocationPin)
			{
				if (!mIsSearchVisible && !mShowingSearch)
					ShowSearchPanel();
				else
					HideSearchPanel();
			}
		}

		void OnCallback (string name)
		{
			// special-case search within SH Message Feed
			if (name == cFeedSearch)
			{
				// TODO:
				return;
			}

			HideSearchPanel();
			HideFeed();

			Device.BeginInvokeOnMainThread (() =>
				wbvContent.InvokeScript (string.Format (cCallbackFormat, name)));
		}

		void ShowFeed (IList<SHFeedObject> feedItems)
		{
			if (ViewModel.IsFeedVisible)
				return;

			// create feed menus
			ViewModel.ClearMenus();

			// TODO: schema should really push us this on show feed callback
			ViewModel.LoadMenuMap (new TownFishMenuMap
			{
				Menus = new Models.TownFishMenuList
				{
					Top = new TownFishMenu
					{
						display = true,
						items = new List<TownFishMenuItem>
									{
										// empty left icon
										new TownFishMenuItem
										{
											Align = "left",
											IconUrl = "/applications/profiles/design/menuicons/clear-hdpi.png",
											Kind = "icon",
											Name = "doNothing",
											Size = "ldpi",
											Type = "callback",
											Value = ""
										},
										new TownFishMenuItem
										{
											Align = "middle",
											Main = "Discoveries",
											Type = "heading"
										},
										new TownFishMenuItem
										{
											Align = "right",
											IconUrl = "/applications/profiles/design/menuicons/magnifying-search-hdpi-ffffff.png",
											Kind = "icon",
											Name = cFeedSearch,
											Size = "ldpi",
											Type = "callback",
											Value = ""
										}
									}
					},
					Bottom = mCurrentMenuMap.Menus.Bottom
				}
			});

#if STREETHAWKWORKING
			if ((feedItems?.Count ?? 0) == 0)
			{
				// create dummy model to show no feed available
				ViewModel.FeedItems = new ObservableCollection<FeedItemViewModel> (
					new List<FeedItemViewModel>
					{
						new FeedItemViewModel
						{
							Title = "Sorry, nothing to see here!"
						}
					});
			}
			else
#endif
			{
				var items = new List<FeedItemViewModel>();
				foreach (var item in feedItems)
				{
					string imgUrl = null;

					var lines = item.content.Split ('\n');
					foreach (var line in lines)
					{
						var aLine = line.Split ('=');
						if (aLine.Length > 1 && aLine [0].Trim() == "img")
						{
							imgUrl = aLine [1].Trim (' ', '"',';');
							break;
						}
					}

					items.Add (new FeedItemViewModel
					{
						ImageSource = imgUrl,
						Title = item.title,
						Text = item.message,
						TimeStamp = DateTime.Parse (item.created).ToString ("d MMM HH.mm"),
						Group = ""
					});

					ViewModel.FeedItems = new ObservableCollection<FeedItemViewModel> (items);
				}

#if !STREETHAWKWORKING
				// create dummy feed model
				ViewModel.FeedItems = new ObservableCollection<FeedItemViewModel> (
					new List<FeedItemViewModel>
					{
						new FeedItemViewModel
						{
							ImageSource = ImageSource.FromResource (
									"TownFish.App.Images.Dummy.muffin.png"),
							Title = "Free Sausage & Egg Muffin",
							Text = "Bill's Brekkie Bar, Camden. Valid for Today Only!",
							TimeStamp = "2 May 11.40",
							Group = "Restaurants & Dining"
						},
						new FeedItemViewModel
						{
							ImageSource = ImageSource.FromResource (
									"TownFish.App.Images.Dummy.meat.png"),
							Title = "Côte de Boeuf for two people with sides or only £20!",
							Text = "Hawksmoor Steak house in Covent Garden",
							TimeStamp = "2 May 11.40",
							Group = "Restaurants & Dining"
						}
					});
#endif
			}

			ViewModel.IsFeedVisible = true;
		}

		void HideFeed()
		{
			if (ViewModel.IsFeedVisible)
			{
				// clear & hide feed
				ViewModel.IsFeedVisible = false;
				ViewModel.FeedItems = null;

				// restore last menus
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

		void ShowLoading()
		{
			if (ViewModel.IsLoading)
				return;

			if (!mFirstLoading)
			{
				pnlLoading.Opacity = 1;
				pnlLoading.TranslationX = pnlLoading.Width;
				pnlLoading.TranslateTo (0, 0, cLoadingPanelAnimationTime, Easing.CubicInOut);
			}

			ViewModel.IsLoading = true;
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

		void ViewModel_MenusLoaded (object sender, string e)
		{
			Device.BeginInvokeOnMainThread (() => HideLoading());

			// if no sync token, nobody is logged in so don't bother getting ID
			if (string.IsNullOrEmpty (ViewModel.SyncToken))
			{
				App.Current.CheckedCuid = false; // in case we switch users
				return;
			}

			// if we haven't checked CUID and we have a sync token, do so now
			if (!App.Current.CheckedCuid && !mCheckingCuid)
				CheckCuid();
		}

		async void CheckCuid()
		{
			try
			{
				mCheckingCuid = true;

				var cli = new HttpClient();
				var devID = App.Current.DeviceID;
				var url = string.Format (App.SHCuidUrl, devID, ViewModel.SyncToken);

				var result = await cli.GetStringAsync (url);
				var shcuid = JsonConvert.DeserializeObject<Dictionary<string, string>> (result);

				string code, newID;
				if (!shcuid.TryGetValue (cCode, out code) &&
						shcuid.TryGetValue (cSHCuid, out newID))
				{
					var props = App.Current.Properties;
					object obj;
					string userID = null;

					if (props.TryGetValue (cSHCuid, out obj))
						userID = obj as string;

#if DEBUG
					Device.BeginInvokeOnMainThread (() =>
					{
						var message = string.Format (
								"TownFish: userID = {0}; newID = {1}",
								userID ?? "null", newID ?? "null");

						App.Current.MainPage.DisplayAlert (
								"StreetHawk Registration", message, "Continue");
					});
#endif
					if (userID != newID)
					{
						userID = newID;

						var shAnalytics = DependencyService.Get<IStreetHawkAnalytics>();
						shAnalytics.TagCuid (userID);

						url = string.Format (App.SHSyncUrl, devID, ViewModel.SyncToken);

						for (var i = 0; i++ <= cSHSyncRetries; )
						{
							// give SH time to process it
							await Task.Delay (cSHSyncDelay);

							result = await cli.GetStringAsync (url);
							var syncResult = JsonConvert.DeserializeObject<Dictionary<string, string>> (result);

							string sync;
							if (!shcuid.TryGetValue (cCode, out code) &&
									syncResult.TryGetValue (cSynced, out sync) &&
									sync == "true")
							{
								props [cSHCuid] = userID;

								// got it, so stop trying
								break;
							}
						}
					}
				}
			}
			catch {} // if it fails, we don't care
			finally
			{
				App.Current.CheckedCuid = true;
				mCheckingCuid = false;
			}
		}

#endregion Methods

#region Properties

		public BrowserPageViewModel ViewModel
				{ get { return BindingContext as BrowserPageViewModel; } }

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

		const string cCode = "Code";
		const string cSHCuid = "shcuid";
		const string cSynced = "synced";
		const int cSHSyncDelay = 3000;
		const int cSHSyncRetries = 3;

		const string cFeedSearch = "FeedSearch";

		bool mFirstLoading = true;
		bool mFirstShowing = true;

		bool mIsSearchVisible;
		bool mShowingSearch;
		bool mHidingSearch;

		bool mCheckingCuid;

		TownFishMenuMap mCurrentMenuMap;

#endregion Fields
	}
}
