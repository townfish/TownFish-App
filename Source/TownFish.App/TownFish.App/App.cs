﻿using System;

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
			vm.SourceUrl = "http://dev.townfish.tk/?mode=app";

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
			BackButtonPressed?.Invoke (this, new EventArgs());
		}

		public void CloseApp()
		{
			AppCloseRequested?.Invoke (this, new EventArgs());
		}

		#endregion

		#region Properties

		public static new App Current { get { return Application.Current as App; } }

		#endregion Properties

		#region Events

		public event EventHandler BackButtonPressed;
		public event EventHandler AppCloseRequested;

		#endregion
	}
}
