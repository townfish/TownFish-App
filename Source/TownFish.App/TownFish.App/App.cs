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
			var json = "{ \"locationsAPI\": \"\", \"menus\": { \"top\": { \"display\": true, \"position\": \"top\", \"items\": [{ \"type\": \"link\", \"kind\": \"icon\", \"value\": \"Home\", \"iconurl\": \"https://i.ytimg.com/vi/tntOCGkgt98/maxresdefault.jpg\", \"size\": \"mdpi\", \"href\": \"http://dev.townfish.tk/\", \"align\": \"left\" }, { \"type\": \"location\", \"value\": \"\", \"align\": \"left\" }, { \"type\": \"link\", \"kind\": \"icon\", \"value\": \"Hamburger\", \"iconurl\": \"https://i.ytimg.com/vi/tntOCGkgt98/maxresdefault.jpg\", \"size\": \"mdpi\", \"href \": \"http://dev.townfish.tk/messages/inbox\", \"align\": \"right\" }] }, \"topsub\": { \"display\": true, \"position\": \"topsub\", \"items\": [{ \"type\": \"link\", \"kind\": \"text\", \"value\": \"All\", \"href\": \"/\", }, { \"type\": \"link\", \"kind\": \"text\", \"value\": \"Questions\", \"href\": \"/\", }, { \"type\": \"link\", \"kind\": \"text\", \"value\": \"News\", \"href\": \"/\", }, { \"type\": \"link\", \"kind\": \"text\", \"value\": \"Categories\", \"href \": \"/\", } ], \"limitby\": 3 }, \"bottom\": { \"display\": true, \"position\": \"bottom\", \"items\": [{ \"type\": \"link\", \"kind\": \"icon\", \"value\": \"Home\", \"iconurl\": \"https://i.ytimg.com/vi/tntOCGkgt98/maxresdefault.jpg\", \"size\": \"mdpi\", \"href\": \"/\", \"align\": \"middle\", \"highlight\": true }, { \"type\": \"link\", \"kind\": \"icon\", \"value\": \"Notifications\", \"iconurl\": \"https://i.ytimg.com/vi/tntOCGkgt98/maxresdefault.jpg\", \"size\": \"mdpi\", \"href\": \"/profile/notifications/\", \"align\": \"middle\", \"hightlight\": false }, { \"type\": \"link\", \"kind\": \"icon\", \"value\": \"Messages\", \"iconurl\": \"https://i.ytimg.com/vi/tntOCGkgt98/maxresdefault.jpg\", \"size\": \"mdpi\", \"href\": \"/messages/inbox\", \"align\": \"middle\", \"highlight\": false }, { \"type\": \"link\", \"kind\": \"icon\", \"value\": \"Questions\", \"iconurl\": \"https://i.ytimg.com/vi/tntOCGkgt98/maxresdefault.jpg\", \"size\": \"mdpi\", \"href\": \"/discussions/type/question\", \"align\": \"middle\", \"highlight\": false }, { \"type\": \"link\", \"kind\": \"icon\", \"value\": \"Newsfeed\", \"iconurl\": \"https://i.ytimg.com/vi/tntOCGkgt98/maxresdefault.jpg\", \"size\": \"mdpi\", \"href\": \"/discussions/type/discussion\", \"align\": \"middle\", \"highlight\": false }] } } }";

			var model = JsonConvert.DeserializeObject<TownFishMenuMap>(json);

			var vm = BrowserPageViewModel.Create("http://dev.townfish.tk", model);
			vm.Source = "http://dev.townfish.tk/discussions?mode=app";
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
