using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Xamarin.Forms;

using TownFish.App.Models;
using TownFish.App.ViewModels;
using System.Threading.Tasks;

namespace TownFish.App.Pages
{
	public partial class BrowserPage : ContentPage
	{
		#region Construction

		public BrowserPage()
		{
			InitializeComponent();

			App.Current.BackButtonPressed += BrowserPage_BackButtonPressed;

			wbvContent.NavigationStarted += WebView_Navigating;
			wbvContent.NavigationFinished += WebView_Navigated;
			//wbvContent.LoadFailed // TODO: implement error handling!

			wbvContent.AffixedUrlParam = cAppMode;
			wbvContent.FragmentActions = new Dictionary<string, string>();

			// set up JS bridge callbacks and fragment notifications
			var invokeMethod = Device.OS == TargetPlatform.iOS ? cJSInvokeMethodiOS : cJSInvokeMethodDroid;

			var invokeReloadJS = string.Format (cStringifyFormat, cSchemaKey, cSchemaMethod);
			var invokePopupJS = string.Format (cStringifyFormat, cMenuKey, cMenuMethod);
			var invokePushJS = string.Format (cStringifyFormat, cPushKey, cPushMethod);

			var reloadJS = string.Format (cInvokeFormat, invokeMethod, invokeReloadJS);
			var popupJS = string.Format (cInvokeFormat, invokeMethod, invokePopupJS);
			var pushJS = string.Format (cInvokeFormat, invokeMethod, invokePushJS);

			wbvContent.FragmentActions.Add (cSchemaAction, reloadJS);
			wbvContent.FragmentActions.Add (cMenuAction, popupJS);
			wbvContent.FragmentActions.Add (cPushAction, pushJS);

			wbvContent.OnLoadJSScript = reloadJS;
			wbvContent.RegisterAction (WebViewAction);

			// on iOS we have to allow for the overlapping status bar at top of layout
			if (Device.OS == TargetPlatform.iOS)
			{
				var pad = pnlTopMenuBar.Padding;
				pad.Top += cTopPaddingiOS;
				pnlTopMenuBar.Padding = pad;
			}

			btnLocation.Tapped += LocationTapped;
			gstLocation.Tapped += LocationTapped;

			// continue initialisation below once caller has set binding context
			BindingContextChanged += BrowserPage_BindingContextChanged;
		}

		#endregion Construction

		#region Methods

		async void WebViewAction (string json)
		{
			System.Diagnostics.Debug.WriteLine (json);

			string key = null;
			string value = null;

			try
			{
				var model = JsonConvert.DeserializeObject<TownFishTopLevelMenu>(json);

				key = model.Key;
				value = model.Value;

				switch (key)
				{
					case cSchemaKey:
						var schema = JsonConvert.DeserializeObject<TownFishMenuMap> (value);
						ViewModel.LoadMenuMap (BrowserPageViewModel.cBaseUri, schema);
						break;

					case cMenuKey:
						var menu = JsonConvert.DeserializeObject<List<TownFishMenuItem>> (value);

						var action = await DisplayActionSheet (cMoreActions, cCancel, null,
										menu.Select (i => i.Value).ToArray());

						var selectedItem = menu.FirstOrDefault (i => i.Value == action);
						if (selectedItem.Type == cCallbackType)
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
				System.Diagnostics.Debug.WriteLine (e.Message);

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

		void WebView_Navigated (object sender, EventArgs e)
		{
			// don't reveal page until menu has been rendered, to avoid the 'jump' down problem
			//await pnlLoading.FadeTo (0);
			//ViewModel.IsLoading = false;
		}

		void WebView_Navigating (object sender, EventArgs e)
		{
			if (ViewModel.IsLoading == false)
			{
				ViewModel.IsLoading = true;

				pnlLoading.Opacity = 1;
				pnlLoading.TranslationX = pnlLoading.Width;
				pnlLoading.TranslateTo(0, 0);
			}
		}

		void BrowserPage_BindingContextChanged (object sender, EventArgs e)
		{
			if (ViewModel == null)
				return;

			// Because we use DisplayActionSheet
			ViewModel.TopActionMoreCommand = new Command (async _ =>
				{
					var action = await DisplayActionSheet (cMoreActions, cCancel, null,
							ViewModel.OverflowImages.Select (i => i.Value).ToArray<string>());

					ViewModel.SourceUrl = ViewModel.OverflowImages.First (i =>
							i.Value == action).Href + BrowserPageViewModel.cBaseUriParam;
				});

			ViewModel.CallbackRequested += (s, name) =>
					OnCallback (name);

			ViewModel.MenusLoaded += ViewModel_MenusLoaded;

			inpSearch.TextChanged += inpSearch_TextChanged;
			inpSearch.Completed += inpSearch_Completed;
			lstLocationSearchResults.ItemTapped += lstLocationSearchResults_ItemTapped;
			lstAvailableLocations.ItemTapped += lstAvailableLocations_ItemTapped;
		}

		void lstAvailableLocations_ItemTapped (object sender, ItemTappedEventArgs e)
		{
			HideSearchPanel();

			ViewModel.SetLocation ((e.Item as AvailableLocation).ID);
		}

		void inpSearch_Completed (object sender, EventArgs e)
		{
			if (inpSearch.Text.Length > 2)
				ViewModel.UpdateLocationList(inpSearch.Text);
		}

		void lstLocationSearchResults_ItemTapped (object sender, ItemTappedEventArgs e)
		{
			HideSearchPanel();

			ViewModel.SearchLocationHasResults = false;
			ViewModel.SetLocation ((e.Item as TownfishLocationItem).CityID);
		}

		void inpSearch_TextChanged (object sender, TextChangedEventArgs e)
		{
			if (e.NewTextValue.Length > 0)
				ViewModel.SearchHasContent = true;
			else
				ViewModel.SearchHasContent = false;

			if (e.NewTextValue.Length > 2)
				ViewModel.UpdateLocationList(e.NewTextValue);
		}

		void BrowserPage_BackButtonPressed (object sender, EventArgs e)
		{
			if (ViewModel.IsSearchPanelVisible || mShowingSearch)
				HideSearchPanel();
			else if (wbvContent.CanGoBack)
				wbvContent.GoBack();
			else
				App.Current.CloseApp();
		}

		void OnMemAgreeClicked (object sender, EventArgs e)
		{
			HideSearchPanel();

			ViewModel.SourceUrl = cTermsUrl;
		}

		void LocationTapped (object sender, EventArgs e)
		{
			if (ViewModel.LeftActionIsLocationPin)
			{
				if (!ViewModel.IsSearchPanelVisible && !mShowingSearch)
					ShowSearchPanel();
				else
					HideSearchPanel();
			}
		}

		void OnCallback (string name)
		{
			HideSearchPanel();

			wbvContent.InvokeJS (string.Format (cCallbackFormat, name.ToLower()));
		}

		async void ShowSearchPanel()
		{
			if (ViewModel.IsSearchPanelVisible || mShowingSearch)
				return;

			mShowingSearch = true;

			// first, kill any existing animations
			ViewExtensions.CancelAnimations (pnlLocations);
			ViewExtensions.CancelAnimations (pnlTopSearchReveal);

			var tsp = pnlTopSearchPlaceholder;

			// first time through we need to put panels in their initial (concealed) positions
			if (mFirstShowing)
			{
				mFirstShowing = false;

				pnlLocations.TranslationY = -pnlLocationsPlaceholder.Height;

				var tspRC = new Rectangle (tsp.X, tsp.Y, 0, tsp.Height);
				await pnlTopSearchReveal.LayoutTo (tspRC, 0);

				// now we have them properly concealed, we can reveal all below!
			}

			ViewModel.IsSearchPanelVisible = true;

			var locXlate = pnlLocations.TranslateTo (0, 0, cLocationPanelAnimationTime, Easing.CubicInOut);
			var tsrLayout = pnlTopSearchReveal.LayoutTo (tsp.Bounds, cLocationPanelAnimationTime, Easing.CubicInOut);

			await Task.WhenAll (locXlate, tsrLayout);

			mShowingSearch = false;
		}

		async void HideSearchPanel()
		{
			if (!ViewModel.IsSearchPanelVisible || mHidingSearch)
				return;

			mHidingSearch = true;

			// first, kill any existing animations
			ViewExtensions.CancelAnimations (pnlLocations);
			ViewExtensions.CancelAnimations (pnlTopSearchReveal);

			var tsp = pnlTopSearchPlaceholder;
			var tspRC = new Rectangle (tsp.X, tsp.Y, 0, tsp.Height);

			var locXlate = pnlLocations.TranslateTo (0, -pnlLocationsPlaceholder.Height, cLocationPanelAnimationTime, Easing.CubicInOut);
			var tsrLayout = pnlTopSearchReveal.LayoutTo (tspRC, cLocationPanelAnimationTime, Easing.CubicInOut);

			await Task.WhenAll (locXlate, tsrLayout);

			ViewModel.IsSearchPanelVisible = false;
			mHidingSearch = false;
		}

		void ViewModel_MenusLoaded (object sender, string e)
		{
			if (ViewModel.IsLoading)
			{
				Device.BeginInvokeOnMainThread (async () =>
					{
						// manually set this as span isn't bindable
						spnMemberAgreementLink.ForegroundColor = ViewModel.LocationsLinkColour;

						await pnlLoading.FadeTo(0);
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

		const string cJSInvokeMethodiOS = "window.webkit.messageHandlers.invokeAction.postMessage";
		const string cJSInvokeMethodDroid = "jsBridge.invokeAction";

		const string cInvokeFormat = "{0}({1});";
		const string cStringifyFormat = "JSON.stringify({{ key: '{0}', value: JSON.stringify({1})}})";

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

		const string cAppMode = "mode=app";
		const string cMoreActions = "More Actions";
		const string cCancel = "Cancel";

		const string cTermsUrl = "http://www.townfish.com/terms-of-use/";

		bool mFirstShowing = true;
		bool mShowingSearch;
		bool mHidingSearch;

		#endregion Fields
	}
}
