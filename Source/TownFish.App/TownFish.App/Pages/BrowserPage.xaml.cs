using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Xamarin.Forms;

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

			App.Current.BackButtonPressed += BrowserPage_BackButtonPressed;

			// set up JS bridge action scripts
			wbvContent.PageLoadedScript = cPageLoadScript;

			wbvContent.Actions = new Dictionary<string, string>
			{
				{ cSchemaAction, cSchemaMethod },
				{ cMenuAction, cMenuMethod },
				{ cPushAction, cPushMethod }
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

			// continue initialisation below once caller has set binding context
			BindingContextChanged += BrowserPage_BindingContextChanged;
		}

		#endregion Construction

		#region Methods

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
					case cSchemaAction:
						var schema = JsonConvert.DeserializeObject<TownFishMenuMap> (result);
						ViewModel.LoadMenuMap (schema);
						break;

					case cMenuAction:
						var menu = JsonConvert.DeserializeObject<List<TownFishMenuItem>> (result);

						var selection = await DisplayActionSheet (cMoreActions, cCancel, null,
								menu.Select (i => i.Value).ToArray());

						var selectedItem = menu.FirstOrDefault (i => i.Value == selection);
						if (selectedItem?.Type == cCallbackType)
							OnCallback (selectedItem.Name);

						break;

					case cPushAction:
						// TODO: implement push!
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
					ViewModel.IsBottomBarVisible = false;
					ViewModel.IsTopSubBarVisible = false;
					ViewModel.IsTopBarVisible = false;
					ViewModel.IsLoading = false;
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
			var uri = new Uri (url);
			if (!ViewModel.IsLoading)
			{
				ViewModel.IsLoading = true;

				pnlLoading.Opacity = 1;
				pnlLoading.TranslationX = pnlLoading.Width;
				pnlLoading.TranslateTo(0, 0);
			}
		}

		async void UberWebView_NavigationFinished (object sender, string url)
		{
			var uri = new Uri (url);
			if (uri.Host != App.SiteDomain)
			{
				// for our site, don't reveal page until menu has been rendered,
				// to avoid the 'jump' down problem; other sites show now as
				// obviously there's no menu on them!

				await pnlLoading.FadeTo (0);
				ViewModel.IsLoading = false;
			}
		}

		void UberWebView_NavigationFailed (object sender, Exception ex)
		{
			ViewModel.IsLoading = false;

#if DEBUG
			var msg = ex.Message;
#else
			var msg = "Please try again or go to a different page";
#endif
			App.Current.MainPage.DisplayAlert ("TownFish", string.Format (
					"Failed to load the page.\n\n{0}", msg), "Cancel");
		}

		void BrowserPage_BindingContextChanged (object sender, EventArgs e)
		{
			if (ViewModel == null)
				return;

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

			ebxSearch.TextChanged += ebxSearch_TextChanged;
			ebxSearch.Completed += ebxSearch_Completed;
			lstLocationSearchResults.ItemTapped += lstLocationSearchResults_ItemTapped;
			lstAvailableLocations.ItemTapped += lstAvailableLocations_ItemTapped;

			ViewModel.PropertyChanged += ViewModel_PropertyChanged;
		}

		void ebxSearch_TextChanged (object sender, TextChangedEventArgs e)
		{
			if (e.NewTextValue.Length > 0)
				ViewModel.SearchHasContent = true;
			else
				ViewModel.SearchHasContent = false;

			if (e.NewTextValue.Length > 2)
				ViewModel.UpdateLocationList(e.NewTextValue);
		}

		void ebxSearch_Completed (object sender, EventArgs e)
		{
			if (ebxSearch.Text.Length > 2)
				ViewModel.UpdateLocationList(ebxSearch.Text);
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

		void BrowserPage_BackButtonPressed (object sender, EventArgs e)
		{
			if (mIsSearchVisible || mShowingSearch)
				HideSearchPanel();
			else if (wbvContent.CanGoBack)
				wbvContent.GoBack();
			else
				App.Current.CloseApp();
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
			HideSearchPanel();

			Device.BeginInvokeOnMainThread (() =>
				wbvContent.InvokeScript (string.Format (cCallbackFormat, name)));
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

			if (ViewModel.SearchLocationHasResults)
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

		void ViewModel_MenusLoaded (object sender, string e)
		{
			if (ViewModel.IsLoading)
			{
				Device.BeginInvokeOnMainThread (async () =>
					{
						await pnlLoading.FadeTo (0);
						ViewModel.IsLoading = false;
					});
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
#else
		const uint cLocationPanelAnimationTime = 250;
#endif

		// apparently iOS status bar height is always 20 in XF (apparently, I said)
		const double cTopPaddingiOS = 20;

		const string cPageLoadScript = "UberWebView.onAction ('" + cSchemaAction + "');";

		const string cSchemaAction = "app_schema_reload";
		const string cSchemaMethod = "twnfsh.getSchema()";

		const string cMenuAction = "app_schema_popup_menu";
		const string cMenuMethod = "twnfsh.getPopupMenuSchema()";

		const string cPushAction = "app_schema_push_messages";
		const string cPushMethod = "twnfsh.getPushMessages()";

		const string cCallbackFormat = "twnfsh.runCallback('{0}')";
		const string cCallbackType = "callback";

		const string cMoreActions = null;//Please Select:";
		const string cCancel = "Cancel";

		bool mFirstShowing = true;
		bool mIsSearchVisible;
		bool mShowingSearch;
		bool mHidingSearch;

		#endregion Fields
	}
}
