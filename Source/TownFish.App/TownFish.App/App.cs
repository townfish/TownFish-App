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
			var vm = BrowserPageViewModel.Create(null);			
			vm.Source = "http://dev.townfish.tk/discussions?mode=app";
			// The root page of your application
			MainPage = new BrowserPage() { BindingContext = vm };
		}

		#endregion

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
			if(BackButtonPressed != null)
				BackButtonPressed.Invoke(this, new EventArgs());
		}

		public void OnAndroidCloseApp()
		{
			if(AndroidAppCloseRequested != null)
				AndroidAppCloseRequested.Invoke(this, new EventArgs());
		}

		#endregion

		#region Events

		public event EventHandler BackButtonPressed;
		public event EventHandler AndroidAppCloseRequested;

		#endregion
	}
}
