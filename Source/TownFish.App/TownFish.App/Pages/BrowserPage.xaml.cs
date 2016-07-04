using System;
using System.Collections.Generic;
using System.Linq;

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
			webView.NavigationStarted += WebView_Navigating;
			webView.NavigationFinished += WebView_Navigated;
			webView.AffixedUrlParam = "mode=app";
			webView.FragmentActions = new Dictionary<string, string>();

			var popupJs = "";

			if (Device.OS == TargetPlatform.iOS)
			{
				webView.OnLoadJSScript = "window.webkit.messageHandlers.invokeAction.postMessage(JSON.stringify({ key: 'schema', value: JSON.stringify(twnfsh.getSchema())}));";
				popupJs = "window.webkit.messageHandlers.invokeAction.postMessage(JSON.stringify({ key: 'menu', value: JSON.stringify(twnfsh.getPopupMenuSchema())}));";
			}
			else
			{
				webView.OnLoadJSScript = "jsBridge.invokeAction(JSON.stringify({ key: 'schema', value: JSON.stringify(twnfsh.getSchema())}))";
				popupJs = "jsBridge.invokeAction(JSON.stringify({ key: 'menu', value: JSON.stringify(twnfsh.getPopupMenuSchema())}))";
				(App.Current as App).BackButtonPressed += BrowserPage_BackButtonPressed;
			}

			webView.FragmentActions.Add("#app_schema_reload", webView.OnLoadJSScript);
			webView.FragmentActions.Add("#app_schema_popup_menu", popupJs);

			webView.RegisterAction(async (s) =>
			{
				System.Diagnostics.Debug.WriteLine(s);
				try
				{
					var model = JsonConvert.DeserializeObject<TownFishTopLevelMenu>(s);

					if (model.Key == "schema")
					{
						var schema = JsonConvert.DeserializeObject<TownFishMenuMap>(model.Value);
						ViewModel.LoadMenuMap(BrowserPageViewModel.cBaseUri, schema);
					}
					else if (model.Key == "menu")
					{
						var menu = JsonConvert.DeserializeObject<List<TownFishMenuItem>>(model.Value);

						var action = await DisplayActionSheet("More Actions", "Cancel", null,
							menu.Select(x => x.value).ToArray<string>());

						var item = menu.FirstOrDefault(x => x.value == action);

						if (item != null && item.type == "callback")
						{
							webView.InvokeJS("twnfsh.runCallback('" + item.name.ToLower() + "')");
						}
					}
				}
				catch (Exception e)
				{
					// TODO: menu failed, use a default? go to offline page?
					System.Diagnostics.Debug.WriteLine(e.Message);
					ViewModel.IsBottomBarVisible = false;
					ViewModel.IsTopBarSubVisible = false;
					ViewModel.IsTopBarVisible = false;
					ViewModel.IsLoading = false;
				}
			});

			BindingContextChanged += BrowserPage_BindingContextChanged;

			if (Device.OS == TargetPlatform.iOS)
				rootGrid.Padding = new Thickness(0, 22, 0, 0);

			LocationBtn.Tapped += LocationTapped;
			LocationLbl.Tapped += LocationTapped;
			//SearchPanelCloseBtn.Tapped += SearchPanelCloseBtn_Tapped;
		}

		#endregion

		#region Properties

		public BrowserPageViewModel ViewModel
		{
			get { return BindingContext as BrowserPageViewModel; }
		}

		#endregion

		#region Methods

		async void WebView_Navigated(object sender, EventArgs e)
		{
			//await loadingPanel.FadeTo(0);
			//ViewModel.IsLoading = false;
		}

		void WebView_Navigating(object sender, EventArgs e)
		{
			if (ViewModel.IsLoading == false)
			{
				ViewModel.IsLoading = true;
				loadingPanel.Opacity = 1;
				loadingPanel.TranslationX = loadingPanel.Width;
				loadingPanel.TranslateTo(0, 0);
			}
		}

		void BrowserPage_BindingContextChanged(object sender, EventArgs e)
		{
			if (ViewModel != null)
			{
				// Because we use DisplayActionSheet
				ViewModel.TopActionMoreCommand = new Command(async () =>
				{
					var action = await DisplayActionSheet("More Actions", "Cancel", null,
						ViewModel.OverflowImages.Select(x => x.value).ToArray<string>());

					ViewModel.Source = ViewModel.OverflowImages.First(i => i.value == action).href + BrowserPageViewModel.cBaseUriParam;
				});

				ViewModel.CallbackRequested += (reqstSender, callback) =>
				{
					webView.InvokeJS("twnfsh.runCallback('" + callback.ToLower() + "')");
				};

				SearchPanelInput.TextChanged += SearchPanelInput_TextChanged;
				SearchPanelInput.Completed += SearchPanelInput_Completed;
				LocationResultsListView.ItemTapped += LocationResultsListView_ItemTapped;
				ViewModel.MenuRendered += ViewModel_MenuRendered;
				AvalLocationsListview.ItemTapped += AvalLocationsListview_ItemTapped;
			}
		}

		void AvalLocationsListview_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			ViewModel.SearchPanelVisible = false;
			ViewModel.SetLocation((e.Item as AvailableLocation).id);
		}

		void SearchPanelInput_Completed(object sender, EventArgs e)
		{
			if (SearchPanelInput.Text.Length > 2)
				ViewModel.UpdateLocationList(SearchPanelInput.Text);
		}

		void LocationResultsListView_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			ViewModel.SearchLocationHasResults = false;
			ViewModel.SearchPanelVisible = false;
			ViewModel.SetLocation((e.Item as TownfishLocationItem).CityID);
		}

		void SearchPanelInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (e.NewTextValue.Length > 0)
				ViewModel.SearchHasContent = true;
			else
				ViewModel.SearchHasContent = false;

			if (e.NewTextValue.Length > 2)
				ViewModel.UpdateLocationList(e.NewTextValue);
		}

		void BrowserPage_BackButtonPressed(object sender, EventArgs e)
		{
			if (webView.CanGoBack)
				webView.GoBack();
			else
				(App.Current as App).OnAndroidCloseApp();
		}

		async void SearchPanelCloseBtn_Tapped(object sender, EventArgs e)
		{

		}

		async void OnMemAgreeClicked(object sender, EventArgs e)
		{
			ViewModel.Source = "http://www.townfish.com/terms-of-use/";
			var offScreenHeight = (LocationPanel.Height + TopSearchPanel.Height);

			// Silly IOS we have to consider the status bar!
			if (Device.OS == TargetPlatform.iOS)
				offScreenHeight += 22;

			await LocationPanel.TranslateTo(0, offScreenHeight * -1);
			await TopSearchPanel.TranslateTo(-420, 0);
			ViewModel.SearchPanelVisible = false;
		}

		async void LocationTapped(object sender, EventArgs e)
		{
			var height = TopSearchPanelParent.Height - (TopSearchPanelParent.Padding.Top + TopSearchPanelParent.Padding.Bottom);
			if (ViewModel.LeftActionIsLocationPin)
			{
				if (!ViewModel.SearchPanelVisible)
				{
					LocationPanel.TranslationY = (LocationPanel.Height + TopSearchPanel.Height) * -1;
					ViewModel.SearchPanelVisible = true;
					LocationPanel.TranslateTo(0, 0, 250);

					TopSearchPanelParent.IsVisible = true;
					TopSearchPanel.LayoutTo(new Rectangle(TopSearchPanel.X, TopSearchPanel.Y, TopSearchPanelParent.Width - 
						(TopSearchPanelParent.Padding.Left + TopSearchPanelParent.Padding.Right), 
						height), 250, Easing.SpringIn);
				}
				else
				{
					var offScreenHeight = (LocationPanel.Height + TopSearchPanel.Height);

					// Silly IOS we have to consider the status bar!
					if (Device.OS == TargetPlatform.iOS)
						offScreenHeight += 22;

					await LocationPanel.TranslateTo(0, offScreenHeight * -1);
					ViewModel.SearchPanelVisible = false;

					await TopSearchPanel.LayoutTo(new Rectangle(TopSearchPanel.X, TopSearchPanel.Y, 1, height), 350, Easing.SpringIn);
					TopSearchPanelParent.IsVisible = false;
				}
			}
		}

		double locWidth = 0;

		void ViewModel_MenuRendered(object sender, string e)
		{
			if (ViewModel.IsLoading)
			{
				Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
				{
					await loadingPanel.FadeTo(0);
					ViewModel.IsLoading = false;
				});
			}
		}

		#endregion
	}
}
