using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			webView.Navigating += WebView_Navigating;
			webView.Navigated += WebView_Navigated;
		}

		#endregion

		#region Properties

		public BrowserPageViewModel ViewModel
		{
			get { return BindingContext as BrowserPageViewModel; }
		}

		#endregion

		#region Methods

		void WebView_Navigated(object sender, WebNavigatedEventArgs e)
		{
			ViewModel.IsLoading = false;
		}

		void WebView_Navigating(object sender, WebNavigatingEventArgs e)
		{
			ViewModel.IsLoading = true;
		}

		#endregion
	}
}
