using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;


namespace TownFish.App.Models
{
	public class FeedItemViewModel
	{
		public ImageSource ImageSource { get; set; }

		public string Title { get; set; }

		public string Text { get; set; }

		public string TimeStamp { get; set; }

		public string Group { get; set; }
	}
}
