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
					ViewModel.LoadMenuMap(ViewModel.BaseUri, model);
				}
				catch(Exception e)
				{
					// TODO: menu failed, use a default? go to offline page?
					System.Diagnostics.Debug.WriteLine(e.Message);
				}
			});

			BindingContextChanged += BrowserPage_BindingContextChanged;
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

					if(action != "Cancel")
						ViewModel.Source = ViewModel.OverflowImages.First(i => i.value == action).href + ViewModel.AppModeParam;
				});
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

		#endregion
	}
}
