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

		public string BaseUri
		{
			get; set;
		}

		public string AppModeParam
		{
			get; set;
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

		public bool IsTopBarVisible
		{
			get { return mIsTopBarVisible;  }
			set
			{
				mIsTopBarVisible = value;
				OnPropertyChanged();
			}
		}

		public bool IsBottomBarVisible
		{
			get { return mIsBottomBarVisible; }
			set
			{
				mIsBottomBarVisible = value;
				OnPropertyChanged();
			}
		}

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

		public void LoadMenuMap(string baseUri, TownFishMenuMap map)
		{
			if (map.Menus.Bottom.display)
			{
				IsBottomBarVisible = true;

				// Bottom Menu
				if (map.Menus.Bottom.items.Count > 0 && map.Menus.Bottom.items[0] != null)
				{
					BottomAction1Label = map.Menus.Bottom.items[0].iconurl.Replace("{size}", map.Menus.Bottom.items[0].size);
					BottomAction1Command = new Command(() =>
					{
						Source = BaseUri + map.Menus.Bottom.items[0].href + AppModeParam;
					});
				}

				if (map.Menus.Bottom.items.Count > 0 && map.Menus.Bottom.items[1] != null)
				{
					BottomAction2Label = map.Menus.Bottom.items[1].iconurl.Replace("{size}", map.Menus.Bottom.items[1].size);
					BottomAction2Command = new Command(() => { Source = BaseUri + map.Menus.Bottom.items[1].href + AppModeParam; });
				}

				if (map.Menus.Bottom.items.Count > 0 && map.Menus.Bottom.items[2] != null)
				{
					BottomAction3Label = map.Menus.Bottom.items[2].iconurl.Replace("{size}", map.Menus.Bottom.items[2].size);
					BottomAction3Command = new Command(() => { Source = BaseUri + map.Menus.Bottom.items[2].href + AppModeParam; });
				}

				if (map.Menus.Bottom.items.Count > 0 && map.Menus.Bottom.items[3] != null)
				{
					BottomAction4Label = map.Menus.Bottom.items[3].iconurl.Replace("{size}", map.Menus.Bottom.items[3].size);
					BottomAction4Command = new Command(() => { Source = BaseUri + map.Menus.Bottom.items[3].href + AppModeParam; });
				}

				if (map.Menus.Bottom.items.Count > 0 && map.Menus.Bottom.items[4] != null)
				{
					BottomAction5Label = map.Menus.Bottom.items[4].iconurl.Replace("{size}", map.Menus.Bottom.items[4].size);
					BottomAction5Command = new Command(() => { Source = BaseUri + map.Menus.Bottom.items[4].href + AppModeParam; });
				}
			}
			else
			{
				IsBottomBarVisible = false;
			}

			if (map.Menus.TopSub.display)
			{
				IsTopBarVisible = true;

				// Top Menu
				if (map.Menus.TopSub.items.Count > 0 && map.Menus.TopSub.items[0] != null)
				{
					TopAction1Label = map.Menus.TopSub.items[0].value;
					TopAction1Command = new Command(() => { Source = BaseUri + map.Menus.TopSub.items[0].href + AppModeParam; });
				}

				if (map.Menus.TopSub.items.Count > 0 && map.Menus.TopSub.items[1] != null)
				{
					TopAction2Label = map.Menus.TopSub.items[1].value;
					TopAction2Command = new Command(() => { Source = BaseUri + map.Menus.TopSub.items[1].href + AppModeParam; });
				}

				if (map.Menus.TopSub.items.Count > 0 && map.Menus.TopSub.items[2] != null)
				{
					TopAction3Label = map.Menus.TopSub.items[2].value;
					TopAction3Command = new Command(() => { Source = BaseUri + map.Menus.TopSub.items[2].href + AppModeParam; });
				}

				if (map.Menus.TopSub.items.Count > 0 && map.Menus.TopSub.items[3] != null)
				{
					TopAction4Label = map.Menus.TopSub.items[3].value;
					TopAction4Command = new Command(() => { Source = BaseUri + map.Menus.TopSub.items[3].href + AppModeParam; });
				}
			}
			else
			{
				IsTopBarVisible = false;
			}
		}

		public static BrowserPageViewModel Create(string baseUri, TownFishMenuMap map)
		{
			var viewModel = new BrowserPageViewModel
			{
				//	PageTitle = map.TopPrimaryButtonText, TODO,
				BaseUri = baseUri
			};

			viewModel.LoadMenuMap(baseUri, map);

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

		bool mIsTopBarVisible;
		bool mIsBottomBarVisible;

		#endregion
	}
}
