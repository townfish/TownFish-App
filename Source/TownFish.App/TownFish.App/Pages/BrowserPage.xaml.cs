﻿using System;
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

			if (Device.OS == TargetPlatform.iOS)
			{
				webView.OnLoadJSScript = "window.webkit.messageHandlers.invokeAction.postMessage(JSON.stringify(twnfsh.getSchema()));";
			}
			else
			{
				webView.OnLoadJSScript = "jsBridge.invokeAction(JSON.stringify(twnfsh.getSchema()))";
				(App.Current as App).BackButtonPressed += BrowserPage_BackButtonPressed;
			}
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
				rootGrid.Padding = new Thickness(0, 22, 0, 0);
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
			await loadingPanel.FadeTo(0);
			ViewModel.IsLoading = false;
		}

		void WebView_Navigating(object sender, EventArgs e)
		{
			ViewModel.IsLoading = true;
			loadingPanel.Opacity = 1;
			loadingPanel.TranslationX = loadingPanel.Width;
			loadingPanel.TranslateTo(0, 0);
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

		#endregion
	}
}
