using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TownFish.App.Models;

namespace TownFish.App.ViewModels
{
	public class BrowserPageViewModel : BaseViewModel
	{
		#region Properties

		public string Source
		{
			get { return mSource; }
			set
			{
				if (value.Equals(mSource, StringComparison.Ordinal))
					return;

				IsLoading = true;

				mSource = value;
				OnPropertyChanged();
			}
		}

		#region Bottom Action 1

		public string BottomAction1Label
		{
			get { return mBottomAction1Label; }
			set
			{
				if (value.Equals(mBottomAction1Label, StringComparison.Ordinal))
					return;

				mBottomAction1Label = value;
				OnPropertyChanged();
			}
		}

		public ICommand BottomAction1Command { get; set; }

		#endregion

		#region Bottom Action 2

		public string BottomAction2Label
		{
			get { return mBottomAction2Label; }
			set
			{
				if (value.Equals(mBottomAction2Label, StringComparison.Ordinal))
					return;

				mBottomAction2Label = value;
				OnPropertyChanged();
			}
		}

		public ICommand BottomAction2Command { get; set; }

		#endregion

		#region Bottom Action 3

		public string BottomAction3Label
		{
			get { return mBottomAction3Label; }
			set
			{
				if (value.Equals(mBottomAction3Label, StringComparison.Ordinal))
					return;

				mBottomAction3Label = value;
				OnPropertyChanged();
			}
		}

		public ICommand BottomAction3Command { get; set; }

		#endregion

		#region Bottom Action 4

		public string BottomAction4Label
		{
			get { return mBottomAction4Label; }
			set
			{
				if (value.Equals(mBottomAction4Label, StringComparison.Ordinal))
					return;

				mBottomAction4Label = value;
				OnPropertyChanged();
			}
		}

		public ICommand BottomAction4Command { get; set; }

		#endregion

		#region Bottom Action 5

		public string BottomAction5Label
		{
			get { return mBottomAction5Label; }
			set
			{
				if (value.Equals(mBottomAction5Label, StringComparison.Ordinal))
					return;

				mBottomAction5Label = value;
				OnPropertyChanged();
			}
		}

		public ICommand BottomAction5Command { get; set; }

		#endregion

		public string PageTitle
		{
			get { return mPageTitle; }
			set
			{
				if (value.Equals(mPageTitle, StringComparison.Ordinal))
					return;

				mPageTitle = value;
				OnPropertyChanged();
			}
		}

		#region Top Action 1

		public string TopAction1Label
		{
			get { return mTopAction1Label; }
			set
			{
				if (value.Equals(mTopAction1Label, StringComparison.Ordinal))
					return;

				mTopAction1Label = value;
				OnPropertyChanged();
			}
		}

		public ICommand TopAction1Command { get; set; }

		#endregion

		#region Top Action 2

		public string TopAction2Label
		{
			get { return mTopAction2Label; }
			set
			{
				if (value.Equals(mTopAction2Label, StringComparison.Ordinal))
					return;

				mTopAction2Label = value;
				OnPropertyChanged();
			}
		}

		public ICommand TopAction2Command { get; set; }

		#endregion

		#region Top Action 3

		public string TopAction3Label
		{
			get { return mTopAction3Label; }
			set
			{
				if (value.Equals(mTopAction3Label, StringComparison.Ordinal))
					return;

				mTopAction3Label = value;
				OnPropertyChanged();
			}
		}

		public ICommand TopAction3Command { get; set; }

		#endregion

		#region Top Action 4

		public string TopAction4Label
		{
			get { return mTopAction4Label; }
			set
			{
				if (value.Equals(mTopAction4Label, StringComparison.Ordinal))
					return;

				mTopAction4Label = value;
				OnPropertyChanged();
			}
		}

		public ICommand TopAction4Command { get; set; }

		#endregion

		public ICommand TopActionMoreCommand { get; set; }

		public bool IsLoading
		{
			get { return mIsLoading; }
			set
			{
				if (value.Equals(mIsLoading))
					return;

				mIsLoading = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Methods

		public static BrowserPageViewModel Create(MenuMap map)
		{
			var viewModel = new BrowserPageViewModel
			{
				PageTitle = map.TopPrimaryButtonText,
			};

			// Bottom Menu
			if (map.BottomMenuItems.Count > 0 && map.BottomMenuItems[0] != null)
			{
				viewModel.BottomAction1Label = map.BottomMenuItems[0].ImageSource;
				viewModel.BottomAction1Command = new Command(() => { viewModel.Source = map.BottomMenuItems[0].Url; });
			}

			if (map.BottomMenuItems.Count > 0 && map.BottomMenuItems[1] != null)
			{
				viewModel.BottomAction2Label = map.BottomMenuItems[1].ImageSource;
				viewModel.BottomAction2Command = new Command(() => { viewModel.Source = map.BottomMenuItems[1].Url; });
			}

			if (map.BottomMenuItems.Count > 0 && map.BottomMenuItems[2] != null)
			{
				viewModel.BottomAction3Label = map.BottomMenuItems[2].ImageSource;
				viewModel.BottomAction3Command = new Command(() => { viewModel.Source = map.BottomMenuItems[2].Url; });
			}

			if (map.BottomMenuItems.Count > 0 && map.BottomMenuItems[3] != null)
			{
				viewModel.BottomAction4Label = map.BottomMenuItems[3].ImageSource;
				viewModel.BottomAction4Command = new Command(() => { viewModel.Source = map.BottomMenuItems[3].Url; });
			}

			if (map.BottomMenuItems.Count > 0 && map.BottomMenuItems[4] != null)
			{
				viewModel.BottomAction5Label = map.BottomMenuItems[4].ImageSource;
				viewModel.BottomAction5Command = new Command(() => { viewModel.Source = map.BottomMenuItems[4].Url; });
			}

			// Top Menu
			if (map.TopSecondaryMenuItems.Count > 0 && map.TopSecondaryMenuItems[0] != null)
			{
				viewModel.TopAction1Label = map.TopSecondaryMenuItems[0].Name;
				viewModel.TopAction1Command = new Command(() => { viewModel.Source = map.TopSecondaryMenuItems[0].Url; });
			}

			if (map.TopSecondaryMenuItems.Count > 0 && map.TopSecondaryMenuItems[1] != null)
			{
				viewModel.TopAction2Label = map.TopSecondaryMenuItems[1].Name;
				viewModel.TopAction2Command = new Command(() => { viewModel.Source = map.TopSecondaryMenuItems[1].Url; });
			}

			if (map.TopSecondaryMenuItems.Count > 0 && map.TopSecondaryMenuItems[2] != null)
			{
				viewModel.TopAction3Label = map.TopSecondaryMenuItems[2].Name;
				viewModel.TopAction3Command = new Command(() => { viewModel.Source = map.TopSecondaryMenuItems[2].Url; });
			}

			if (map.TopSecondaryMenuItems.Count > 0 && map.TopSecondaryMenuItems[3] != null)
			{
				viewModel.TopAction4Label = map.TopSecondaryMenuItems[3].Name;
				viewModel.TopAction4Command = new Command(() => { viewModel.Source = map.TopSecondaryMenuItems[3].Url; });
			}

			return viewModel;
		}

		#endregion

		#region Fields

		string mPageTitle;
		string mSource;
		string mTopAction1Label;
		string mTopAction3Label;
		string mTopAction2Label;
		string mTopAction4Label;
		string mBottomAction1Label;
		string mBottomAction2Label;
		string mBottomAction3Label;
		string mBottomAction4Label;
		string mBottomAction5Label;

		bool mIsLoading;

		#endregion
	}
}
