using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TownFish.App.Models;
using TownFish.App.Pages;
using TownFish.App.ViewModels;
using Xamarin.Forms;

namespace TownFish.App
{
	public class App : Application
	{
		public App()
		{
			var vm = BrowserPageViewModel.Create("http://dev.townfish.tk", null);
			vm.Source = "http://dev.townfish.tk/discussions";
			vm.AppModeParam = "?mode=app";

			// The root page of your application
			MainPage = new BrowserPage() { BindingContext = vm };
		}

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
	}
}
