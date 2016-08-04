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
			var vm = BrowserPageViewModel.Create (null);			
			vm.SourceUrl = BaseUrl + cStartPath + BaseUrlParam;

			MainPage = new BrowserPage { BindingContext = vm };
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

		// HACK: This should not be necessary!
		const string cStartPath = ""; //"discussions";

		public const string BaseUrl = "http://dev.townfish.com/";
		public const string BaseUrlParam = "?mode=app";

		#endregion Fields
	}
}
