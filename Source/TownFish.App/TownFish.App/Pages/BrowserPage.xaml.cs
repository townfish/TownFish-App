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

			// set up JS bridge fragment actions
			var schemaJS = string.Format (cKVFormat, cSchemaKey, cSchemaMethod);
			var menuJS = string.Format (cKVFormat, cMenuKey, cMenuMethod);
			var pushJS = string.Format (cKVFormat, cPushKey, cPushMethod);

			wbvContent.FragmentActions = new Dictionary<string, string>();
			wbvContent.FragmentActions.Add (cSchemaAction, schemaJS);
			wbvContent.FragmentActions.Add (cMenuAction, menuJS);
			wbvContent.FragmentActions.Add (cPushAction, pushJS);

			wbvContent.PageLoadedScript = schemaJS;

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
			string key = null;
			string value = null;

			try
			{
				var model = JsonConvert.DeserializeObject<TownFishTopLevelMenu>(msg);

				key = model.Key;
				value = model.Value;

				Debug.WriteLine ("WebViewAction: Parsing {0}:\n{1}", key, value);

				switch (key)
				{
					case cSchemaKey:
						var schema = JsonConvert.DeserializeObject<TownFishMenuMap> (value);
						ViewModel.LoadMenuMap (schema);
						break;

					case cMenuKey:
						var menu = JsonConvert.DeserializeObject<List<TownFishMenuItem>> (value);

						var action = await DisplayActionSheet (cMoreActions, cCancel, null,
								menu.Select (i => i.Value).ToArray());

						var selectedItem = menu.FirstOrDefault (i => i.Value == action);
						if (selectedItem?.Type == cCallbackType)
							OnCallback (selectedItem.Name);

						break;

					case cPushKey:
						// TODO: implement push!
						break;
				}
			}
			catch (Exception e)
			{
				// TODO: parse failed, so use a default? go to offline page?
				Debug.WriteLine ("WebViewAction: Parse of:\n{0}\nfailed with error:\n{1}", msg, e.Message);

				// if it was the schema, just hide everything as it might contain rubbish
				if (key == cSchemaKey)
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
					var action = await DisplayActionSheet (cMoreActions, cCancel, null,
							ViewModel.OverflowImages.Select (i => i.Value).ToArray<string>());

					var url = ViewModel.OverflowImages.FirstOrDefault (i => i.Value == action)?.Href;
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
				wbvContent.InvokeScript (string.Format (cCallbackFormat, name.ToLower())));
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
			if (ViewModel.SearchLocationHasResults)
			{
				ViewModel.CancelLocationSearch();
				return;
			}

			if (!mIsSearchVisible || mHidingSearch)
				return;

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

		const string cKVFormat = "JSON.stringify({{ key: '{0}', value: JSON.stringify({1})}})";

		const string cSchemaKey = "schema";
		const string cSchemaMethod = "twnfsh.getSchema()";
		const string cSchemaAction = "#app_schema_reload";

		const string cMenuKey = "menu";
		const string cMenuMethod = "twnfsh.getPopupMenuSchema()";
		const string cMenuAction = "#app_schema_popup_menu";

		const string cPushKey = "push";
		const string cPushMethod = "twnfsh.getPushMessages()";
		const string cPushAction = "#app_schema_push_messages";

		const string cCallbackFormat = "twnfsh.runCallback('{0}')";
		const string cCallbackType = "callback";

		const string cMoreActions = "More Actions";
		const string cCancel = "Cancel";

		bool mFirstShowing = true;
		bool mIsSearchVisible;
		bool mShowingSearch;
		bool mHidingSearch;

		#endregion Fields
	}
}
