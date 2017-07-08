using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;


namespace TownFish.App.ViewModels
{
	public class BottomActionViewModel: ViewModelBase
	{
		#region Properties

		public string Icon { get; set; }

		public ICommand Command { get; set; }

		public int SuperCount
		{
			get { return Get<int>(); }

			set
			{
				if (Set (value))
					OnPropertyChanged (() => HasSuperCount);
			}
		}

		public string SuperColour
		{
			get { return Get<string>(); }

			set
			{
				if (Set (value))
					OnPropertyChanged (() => SuperCountBackgroundColour);
			}
		}

		//
		// calculated bindings
		//

		public BrowserPageViewModel MainViewModel { get; set; }

		public Color BackgroundColour => MainViewModel.BottomMenuBackgroundColour;

		public Color SuperCountColour => BackgroundColour;

		public Color SuperCountBackgroundColour => Color.FromHex ("#" + (SuperColour ?? "ED1C24"));

		public bool HasSuperCount => SuperCount > 0;

		#endregion Properties
	}
}
