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
			var vm = BrowserPageViewModel.Create(new MenuMap
			{
				BottomMenuItems = new List<NavMenuItem>
					 {
						new NavMenuItem { Name = "Home", ImageSource = "buildingsdark.png", Url = "http://dev.townfish.tk/discussions?mode=app", OrderNo = 0 },
						new NavMenuItem { Name = "Notifications", ImageSource = "belldark.png", Url = "http://dev.townfish.tk/profile/notifications/?mode=app", OrderNo = 1 },
						new NavMenuItem { Name = "Messages", ImageSource = "messagesdark.png", Url = "http://dev.townfish.tk/messages/inbox?mode=app", OrderNo = 2 },
						new NavMenuItem { Name = "Compose", ImageSource = "questiondark.png", Url = "http://dev.townfish.tk/discussions?mode=app", OrderNo = 3 },
						new NavMenuItem { Name = "Pin", ImageSource = "pindark.png", Url = "http://dev.townfish.tk/discussions?mode=app", OrderNo = 4 }
					 },
				TopPrimaryButtonText = "Greater London",
				TopSecondaryMenuItems = new List<NavMenuItem>
					{
						new NavMenuItem { Name = "All", Url = "http://dev.townfish.tk/discussions/type/question?mode=app", OrderNo = 0 },
						new NavMenuItem { Name = "Questions", Url = "http://dev.townfish.tk/discussions/type/question?mode=app", OrderNo = 1 },
						new NavMenuItem { Name = "News", Url = "http://dev.townfish.tk/discussions/type/discussion?mode=app", OrderNo = 2 },
						new NavMenuItem { Name = "Categories", Url = "http://dev.townfish.tk/discussions/type/categories?mode=app", OrderNo = 3 },
					}
			});

			vm.Source = "http://dev.townfish.tk/discussions?mode=app";

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
