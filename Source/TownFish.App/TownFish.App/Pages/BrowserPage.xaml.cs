using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownFish.App.Models;
using TownFish.App.ViewModels;
using Xamarin.Forms;

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

			if (Device.OS == TargetPlatform.iOS)
				webView.OnLoadJSScript = "window.webkit.messageHandlers.invokeAction.postMessage(JSON.stringify(twnfsh.getSchema()));";
			else
				webView.OnLoadJSScript = "jsBridge.invokeAction(JSON.stringify(twnfsh.getSchema()))";

			webView.RegisterAction((s) =>
			{
				System.Diagnostics.Debug.WriteLine(s);
				try
				{
					var model = JsonConvert.DeserializeObject<TownFishMenuMap>(s);
					ViewModel.LoadMenuMap(BrowserPageViewModel.cBaseUri, model);
				}
				catch(Exception e)
				{
					// TODO: menu failed, use a default? go to offline page?
					System.Diagnostics.Debug.WriteLine(e.Message);
					ViewModel.IsBottomBarVisible = false;
					ViewModel.IsTopBarSubVisible = false;
					ViewModel.IsTopBarVisible = false;
				}
			});

			BindingContextChanged += BrowserPage_BindingContextChanged;

			if (Device.OS == TargetPlatform.iOS)
			{
				topBar.Padding = new Thickness(10, 25, 10, 10);
				TopSearchPanel.Padding = new Thickness(20, 25, 20, 10);
			}
		}

		#endregion

		#region Properties

		public BrowserPageViewModel ViewModel
		{
			get { return BindingContext as BrowserPageViewModel; }
		}

		#endregion

		#region Methods

		void WebView_Navigated(object sender, EventArgs e)
		{
			ViewModel.IsLoading = false;
		}

		void WebView_Navigating(object sender, EventArgs e)
		{
			ViewModel.IsLoading = true;
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
			}
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
			if (e.NewTextValue.Length > 2)
				ViewModel.UpdateLocationList(e.NewTextValue);
		}

		#endregion
	}
}
