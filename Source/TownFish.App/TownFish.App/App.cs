using System;

using Xamarin.Forms;

using TownFish.App.Pages;
using TownFish.App.ViewModels;

namespace TownFish.App
{
	public class App : Application
	{
		#region Construction

		public App()
		{
			var vm = new BrowserPageViewModel();
			MainPage = new BrowserPage { BindingContext = vm };

			// now everything's wired up, kick off initial page load
			vm.SourceUrl = BaseUrl + StartPath + QueryString;
		}

		#endregion Construction

		#region Methods

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}

		public void OnBackButtonPressed()
		{
			BackButtonPressed?.Invoke (this, new EventArgs());
		}

		public void CloseApp()
		{
			AppCloseRequested?.Invoke (this, new EventArgs());
		}

		#endregion Methods

		#region Properties and Events

		public event EventHandler BackButtonPressed;

		public event EventHandler AppCloseRequested;

		public static new App Current { get { return Application.Current as App; } }

		#endregion Properties and Events

		#region Fields

		// all magic URLs and paths used in this app
		public const string SiteDomain = "dev.townfish.com";
		public const string BaseUrl = "http://" + SiteDomain;
		public const string TermsUrl = BaseUrl + "/terms-of-use/";
		public const string StartPath = "/";

		public const string QueryParam = "mode=app"; // parameter added to every request
		public const string QueryString = "?" + QueryParam;

		public const string TwitterApiDomain = "api.twitter.com";

		#endregion Fields
	}
}
