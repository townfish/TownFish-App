﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;

using Newtonsoft.Json;

using Apptelic.UberWebViewLib;

using TownFish.App.Controls;
using TownFish.App.Helpers;
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
				{ cShowDiscoveriesAction, cShowDiscoveriesValue },
				{ cHideDiscoveriesAction, cHideDiscoveriesValue }
			};

			wbvContent.ScriptMessageReceived += UberWebView_ScriptMessageReceived;
			wbvContent.NavigationStarting += UberWebView_NavigationStarting;
			wbvContent.NavigationStarted += UberWebView_NavigationStarted;
			wbvContent.NavigationFinished += UberWebView_NavigationFinished;
			wbvContent.NavigationFailed += UberWebView_NavigationFailed;

			wbvContent.QueryParamDomain = App.SiteDomain;
			wbvContent.QueryParam = App.QueryParam;

			// get bottom action frames so we can manually set the background,
			// as binding background colour crashes on Android (thanks, Xamarin!)
			//mBottomActionFrames = new Frame[] { frmSuper0, frmSuper1, frmSuper2, frmSuper3, frmSuper4, frmSuper5 };

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
			App.Current.DiscoveriesUpdated += App_DiscoveriesUpdated;
			App.Current.BackgroundDiscoveriesReceived += App_BackgroundDiscoveriesReceived;
			App.Current.AppResumed += App_Resumed;

			base.OnAppearing();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			App.Current.BackButtonPressed -= App_BackButtonPressed;
			App.Current.PushUrlReceived -= App_PushUrlReceived;
			App.Current.DiscoveriesUpdated -= App_DiscoveriesUpdated;
			App.Current.BackgroundDiscoveriesReceived -= App_BackgroundDiscoveriesReceived;
			App.Current.AppResumed -= App_Resumed;
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			if (ViewModel == null)
				return;

			// manually set this as span isn't bindable
			spnMemberAgreementLink.ForegroundColor = ViewModel.DiscoveriesLinkColour;

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

			// always start in loading state
			ShowLoading();

			// now start at the specified source URL, clearing it first to indicate we're not there yet
			var source = ViewModel.SourceUrl;
			ViewModel.SourceUrl = null;

			Navigate (source);
		}

		protected override bool OnBackButtonPressed()
		{
			// Hello! When did this appear?!

			return base.OnBackButtonPressed();
		}

		void App_BackButtonPressed (object sender, EventArgs e)
        {
            if (wbvContent.CanGoBack)
            {
				// hackish - fake a back callback, to handle back with discoveries/info showing
				// or to let JS code handle actually going back in webview if necessary

                ViewModel_CallbackRequested (this,
						new BrowserPageViewModel.CallbackInfo
						{
							IsNative = false,
							Name = BrowserPageViewModel.CallbackInfo.Back
						});
            }
            else
            {
                App.Current.CloseApp();
            }
		}

		void App_PushUrlReceived (object sender, (string url, bool wasBackgrounded) args)
		{
			// Basecamp Item 32: only navigate if we're not logged in; if logged
			// in the Vanilla code takes care of its own notifications

			// Basecamp Item 40: navigate anyway if we were backgrounded

			if (string.IsNullOrEmpty (ViewModel.SyncToken) || args.wasBackgrounded)
				Navigate (args.url);
		}

		void App_Resumed (object sender, EventArgs e)
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
			string data = null;

			try
			{
				var model = JsonConvert.DeserializeObject<UberWebViewMessage>(msg);

				action = model.Action;
				data = model.Result;
			}
			catch (Exception ex)
			{
				// parse failed, fail gracefully

				Debug.WriteLine (
						"BrowserPage.UberWebView_ScriptMessageReceived: " +
						$"Parse of:\n{msg}\nfailed with error:\n{ex.Message}");

				// if it was the schema, just hide everything as it might contain rubbish
				if (action == cSchemaAction)
				{
					ViewModel.IsTopBarVisible = false;
					ViewModel.IsTopSubBarVisible = false;
					ViewModel.IsTopFormBarVisible = false;
					ViewModel.IsBottomBarVisible = false;

					// hide loading so user can see what's going on
					HideLoading();

					return;
				}
			}

			try
			{
				Debug.WriteLine (
						"BrowserPage.UberWebView_ScriptMessageReceived: " +
						$"Performing {action} on:\n{data}");

				switch (action)
				{
					case cBeginLoadingAction:
						ShowLoading();
						break;

					case cSchemaAction:
						mCurrentMenuMap = JsonConvert.DeserializeObject<TownFishMenuMap> (data);
						ViewModel.LoadMenuMap (mCurrentMenuMap);

						break;

					case cMenusVisibleAction:
						if (mCurrentMenuMap == null)
							break;

						var visibleMenus = JsonConvert.DeserializeObject<string[]> (data);
						mCurrentMenuMap.SetMenuVisibility (visibleMenus);

						// now reload with new visibility settings
						ViewModel.LoadMenuMap (mCurrentMenuMap);

						break;

					case cPopupMenuAction:
						var menu = JsonConvert.DeserializeObject<List<TownFishMenuItem>> (data);
						var items = menu.Select (i => i.Value).ToArray();
						var selection = await DisplayActionSheet (cMoreActions, cCancel, null, items);

						var menuItem = menu.FirstOrDefault (i => i.Value == selection);
						if (menuItem != null)
							ViewModel.GenerateMenuAction (menuItem)?.Execute (null);

						break;

					case cPushAction:
						break;

					case cShowDiscoveriesAction:
						ShowDiscoveries();
						break;

					case cHideDiscoveriesAction:
						// when being told to hide discoveries, that includes info too
                        mDiscoveriesInfoActive = false;

						HideDiscoveries();
						break;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine (
						$"BrowserPage.UberWebView_ScriptMessageReceived: {ex.Message}");
			}
		}

		void UberWebView_NavigationStarting (object sender, WebNavigatingEventArgs e)
		{
			// launch non-TownFish URLs in external browser
			var uri = new Uri (e.Url);
			if (uri.Host != App.SiteDomain && uri.Host != App.TwitterApiDomain)
			{
				e.Cancel = true;

				PlatformHelper.OpenUri (uri);

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

#pragma warning disable IDE1006 // Naming Styles ("lst" is Hungarian Notation prefix used for XAML controls)

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

		void lstDiscoveries_ItemTapped (object sender, EventArgs e)//ItemTappedEventArgs e)
		//async void lstDiscoveries_ItemTapped (object sender, ItemTappedEventArgs e)
		{
			// when using ListView:
			//var fivm = e.Item as DiscoveryItemViewModel;

			// when using StackView:
			var discoveryItem = sender as DiscoveryItem;
			if (discoveryItem.BindingContext is DiscoveryItemViewModel fivm)
				Navigate (fivm.LinkUrl);
		}

#pragma warning restore IDE1006 // Naming Styles

		void App_BackgroundDiscoveriesReceived (object sender, EventArgs e)
		{
			// if already showing discoveries we can ignore this
			if (ViewModel.IsDiscoveriesVisible)
				return;

			// can't just call ShowDiscoveries here as the menu won't be set;
			// we have to ask the schema to tell us to show discoveries, then it
			// sets the menu for us, but as we don't yet have a callback for that...

			// HACK: replace with a callback to schema
			Navigate (App.ShowFeedUrl + App.QueryString);
		}

		/// <summary>
		/// Sets discoveries count and updates the discoveries list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name=""></param>
		void App_DiscoveriesUpdated (object sender, EventArgs e)
		{
			// cache discovery items so it shows quickly when revealed
			UpdateDiscoveryItems();

			// if discoveries already open, just update last viewed time;
			// otherwise update count to show new arrival(s)

			if (ViewModel.IsDiscoveriesVisible)
			{
				App.LastDiscoveriesViewTime = DateTime.Now;

				// show or hide this based on whether we have any now
				ViewModel.IsDiscoveriesInfoVisible = ViewModel.IsDiscoveriesEmpty;
			}
			else
			{
				ViewModel.NewDiscoveriesCount = App.NewDiscoveriesCount;
			}
		}

		async void BrowserPage_EditLikesTapped (object sender, EventArgs e)
		{
			await HideDiscoveriesAsync();

			Navigate (App.EditLikesUrl + App.QueryString);
		}

		async void BrowserPage_EditProfileTapped (object sender, EventArgs e)
		{
			await HideDiscoveriesAsync();

			Navigate (App.EditProfileUrl + App.QueryString);
		}

		void BrowserPage_MemberAgreementTapped (object sender, EventArgs e)
		{
			HideSearchPanel();

			Navigate (App.TermsUrl + App.QueryString);
		}

		void ViewModel_MenusLoaded (object sender, EventArgs e)
		{
			// now update bottom action superscript frame background colours, in case they changed
			//if (ViewModel.IsBottomBarVisible)
			//	for (var i = 0; i < 6; i++)
			//		mBottomActionFrames [i].BackgroundColor =
			//				ViewModel.BottomActions [i].SuperCountBackgroundColour;

			// only hide loading if already showing it but not in the process of hiding discoveries
			if (ViewModel.IsLoading && !mHidingDiscoveries)
				HideLoading();

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

		async void ViewModel_CallbackRequested (object sender, BrowserPageViewModel.CallbackInfo info)
		{
			// special-case user pressing discoveries info button, as web code knows
			// nothing about it

			if (info.IsNative && info.Name == BrowserPageViewModel.CallbackInfo.Info)
			{
				// if already open, don't open again!
				if (ViewModel.IsDiscoveriesInfoVisible)
					return;

                await ShowLoadingAsync();

				mDiscoveriesInfoActive = true;
				ViewModel.IsDiscoveriesInfoVisible = true;

                HideLoading();

				return;
			}

			// special-case back button on info page too, to show discoveries if there are
			// any, otherwise do default back handling

            if (info.Name == BrowserPageViewModel.CallbackInfo.Back &&
					ViewModel.IsDiscoveriesVisible && ViewModel.IsDiscoveriesInfoVisible &&
					!ViewModel.IsDiscoveriesEmpty)
            {
                await ShowLoadingAsync();

                mDiscoveriesInfoActive = false;
                ViewModel.IsDiscoveriesInfoVisible = false;

                HideLoading();

                return;
            }

            // make sure this is closed so location name shows
            ViewModel.IsDiscoveriesInfoVisible = false;

            HideSearchPanel();

            Device.BeginInvokeOnMainThread(() =>
                wbvContent.InvokeScript(string.Format(cCallbackFormat, info.Name)));
        }

		void ViewModel_NavigateRequested (object sender, string url)
		{
			Navigate (url);
		}

		void Navigate (string url)
		{
			if (string.IsNullOrEmpty (url))
                return;

            // if it's a TF URL hide the discoveries and save URL for returning to later
            if (url.StartsWith (App.BaseUrl))
			{
				HideDiscoveries();

				// save this in case we come back later from an external URL
				mLastSourceUrl = url;
            }

            // remember where we're going...
            ViewModel.SourceUrl = url;

			// ... and as WebView isn't bound to ViewModel.SourceUrl, tell it now
			wbvContent.Source = url;
		}

		void UpdateDiscoveryItems()
		{
			var discoveryItems = App.Discoveries;
			var count = App.DiscoveriesCount;

			// if no items, kill previous list
			if (count == 0)
				ViewModel.DiscoveryItems = null;
			else
				ViewModel.DiscoveryItems = new ObservableCollection<DiscoveryItemViewModel> (discoveryItems);

			// HACK: as iOS ListView isn't working, manually generate content for a StackLayout
			Device.BeginInvokeOnMainThread (() =>
				{
					var kids = lstDiscoveries.Children;
					kids.Clear();

					for (var i = 0; i < count; i++)
					{
						var item = new DiscoveryItem { BindingContext = discoveryItems [i] };
						item.Tapped += lstDiscoveries_ItemTapped;

						kids.Add (item);
					}
				});
		}

		void ShowDiscoveries()
		{
			bool UpdateDiscoveryExpiry()
			{
				if (!ViewModel.IsDiscoveriesVisible || ViewModel.DiscoveryItems == null)
					return false;

				foreach (var item in ViewModel.DiscoveryItems)
					item.RecalculateExpiry();

				return true;
			}

			if (ViewModel.IsDiscoveriesVisible)
				return;

			// make sure we have latest discovery items in our list
			if (ViewModel.IsDiscoveriesEmpty)
				UpdateDiscoveryItems();

			ViewModel.IsDiscoveriesVisible = true;
			ViewModel.IsDiscoveriesInfoVisible = ViewModel.IsDiscoveriesEmpty || mDiscoveriesInfoActive;

			if (ViewModel.IsDiscoveriesEmpty)
				return;

            Device.StartTimer (TimeSpan.FromSeconds (1), UpdateDiscoveryExpiry);

			// now we've shown discoveries, reset count & last view time
			ViewModel.NewDiscoveriesCount = 0;
			App.LastDiscoveriesViewTime = DateTime.Now;
		}

		/// <summary>
		/// Fire-and-forget discoveries hider.
		/// </summary>
		async void HideDiscoveries (bool showLoading = true)
		{
			await HideDiscoveriesAsync (showLoading);
		}

		/// <summary>
		/// Awaitable discoveries hider.
		/// </summary>
		/// <returns></returns>
		async Task HideDiscoveriesAsync (bool showLoading = true)
		{
			if (ViewModel.IsDiscoveriesVisible && !mHidingDiscoveries)
			{
				mHidingDiscoveries = true;

				if (showLoading)
					await ShowLoadingAsync(); // wait for it to show before hiding discoveries

				// clear & hide discoveries
				ViewModel.IsDiscoveriesVisible = false;
				ViewModel.DiscoveryItems = null; // side-effect: updates visibility properties!

				// bitofahack - if returning to locations page it takes a while for the menu to
				// be reloaded due to server round-trip, during which time the location name will
				// be visible as well as the discoveries menu, so just clear it here as it will
				// be set again anyway on completion of page load
				ViewModel.LocationName = "";

				mHidingDiscoveries = false;
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

			// NOTE: This goes BEFORE cancelling search as it causes ebxSearch (which loses
			// focus) to be updated with the search term (we don't konw how, but only if
			// the case-insensitive value matches the current search term - yes, really!)
			// which, due to binding, in turn causes the search results to be updated one
			// more time, which results in the search results showing up again AFTER the
			// location switch has occurred, thus hiding the main page. This resulted in
			// Basecamp Issue 72.
			//
			// remove focus from search input
			wbvContent.Focus();

			if (ViewModel.SearchLocationActive)
				ViewModel.CancelLocationSearch();

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

			// set this here in case page loads faster than loading animation completes,
			// which could result in loading never being removed

			ViewModel.IsLoading = true;

			if (!mFirstLoading)
			{
				mOpenedLoadingTime = DateTime.Now;

				pnlLoading.Opacity = 1;
				pnlLoading.TranslationX = pnlLoading.Width;
				await pnlLoading.TranslateTo (0, 0, cLoadingPanelAnimationTime, Easing.CubicInOut);
			}
		}

		void HideLoading()
		{
			async void Fade()
			{
				await pnlLoading.FadeTo (0, cLoadingPanelAnimationTime, Easing.CubicIn);
				ViewModel.IsLoading = false;

				// make sure we only show splash loading once
				if (mFirstLoading)
				{
					mFirstLoading = false;
					imgSplash.IsVisible = false;
				}
			}

			if (!ViewModel.IsLoading)
				return;

			// if called too quickly, wait around a bit so it doesn't flash
			var t = Math.Max (0, cLoadingDelayTime -
					(int) (DateTime.Now - mOpenedLoadingTime).TotalMilliseconds);

			// do it in a bit, in case we're on UI thread, so UI can get back to work first
			Device.StartTimer (TimeSpan.FromMilliseconds (cLoadingPanelAnimationTime + t),
					() => { Device.BeginInvokeOnMainThread (Fade); return false; });
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

		// minimum time to show loading for
        const int cLoadingDelayTime = 1000;

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

		const string cShowDiscoveriesAction = "app_schema_show_feed";
		const string cShowDiscoveriesValue = "true";
		const string cHideDiscoveriesAction = "app_schema_hide_feed";
		const string cHideDiscoveriesValue = "true";

		const string cCallbackFormat = "twnfsh.runCallback('{0}')";

		const string cMoreActions = null; // e.g. "Please Select:";
		const string cCancel = "Cancel";

		const string cCustomUserAgent = "com.townfish.app";

		TownFishMenuMap mCurrentMenuMap;

		bool mFirstLoading = true;
		bool mFirstShowing = true;
		DateTime mOpenedLoadingTime;

		bool mIsSearchVisible;
		bool mShowingSearch;
		bool mHidingSearch;
		bool mHidingDiscoveries;

		string mLastSourceUrl;

        private bool mDiscoveriesInfoActive = false;

        #endregion Fields
    }
}
