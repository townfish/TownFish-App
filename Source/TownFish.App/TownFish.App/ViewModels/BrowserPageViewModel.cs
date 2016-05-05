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

		public bool IsTopBarVisible
		{
			get { return mIsTopBarVisible; }
			set
			{
				mIsTopBarVisible = value;
				OnPropertyChanged();
			}
		}

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

		#region Bottom Bar

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

		public ICommand BottomAction1Command
		{
			get { return mBottomAction1Command; }
			set
			{
				mBottomAction1Command = value;
				OnPropertyChanged();
			}
		}

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

		public ICommand BottomAction2Command
		{
			get { return mBottomAction2Command; }
			set
			{
				mBottomAction2Command = value;
				OnPropertyChanged();
			}
		}

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

		public ICommand BottomAction3Command
		{
			get { return mBottomAction3Command; }
			set
			{
				mBottomAction3Command = value;
				OnPropertyChanged();
			}
		}

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

		public ICommand BottomAction4Command
		{
			get { return mBottomAction4Command; }
			set
			{
				mBottomAction4Command = value;
				OnPropertyChanged();
			}
		}

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

		public ICommand BottomAction5Command
		{
			get { return mBottomAction5Command; }
			set
			{
				mBottomAction5Command = value;
				OnPropertyChanged();
			}
		}

		#endregion

		public bool IsBottomBarVisible
		{
			get { return mIsBottomBarVisible; }
			set
			{
				mIsBottomBarVisible = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Top Bar

		public string TopBarLeftLabel
		{
			get { return mTopBarLeftLabel; }
			set
			{
				if (value.Equals(mTopBarLeftLabel, StringComparison.Ordinal))
					return;

				mTopBarLeftLabel = value;
				OnPropertyChanged();
			}
		}

		public ICommand TopBarLeftCommand
		{
			get { return mTopBarLeftCommand; }
			set
			{
				mTopBarLeftCommand = value;
				OnPropertyChanged();
			}
		}

		public string TopBarRightLabel
		{
			get { return mTopBarRightLabel; }
			set
			{
				if (value.Equals(mTopBarRightLabel, StringComparison.Ordinal))
					return;

				mTopBarRightLabel = value;
				OnPropertyChanged();
			}
		}

		public ICommand TopBarRightCommand
		{
			get { return mTopBarRightCommand; }
			set
			{
				mTopBarRightCommand = value;
				OnPropertyChanged();
			}
		}

		public string TopBarRight1Label
		{
			get { return mTopBarRight1Label; }
			set
			{
				if (value.Equals(mTopBarRight1Label, StringComparison.Ordinal))
					return;

				mTopBarRight1Label = value;
				OnPropertyChanged();
			}
		}

		public ICommand TopBarRight1Command
		{
			get { return mTopBarRight1Command; }
			set
			{
				mTopBarRight1Command = value;
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

		#endregion

		#region Top Sub

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

		public ICommand TopAction1Command
		{
			get { return mTopAction1Command; }
			set
			{
				mTopAction1Command = value;
				OnPropertyChanged();
			}
		}

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

		public ICommand TopAction2Command
		{
			get { return mTopAction2Command; }
			set
			{
				mTopAction2Command = value;
				OnPropertyChanged();
			}
		}

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

		public ICommand TopAction3Command
		{
			get { return mTopAction3Command; }
			set
			{
				mTopAction3Command = value;
				OnPropertyChanged();
			}
		}

		public ICommand TopActionMoreCommand
		{
			get { return mTopBarMoreCommand; }
			set
			{
				mTopBarMoreCommand = value;
				OnPropertyChanged();
			}
		}

		public string TopActionMoreLabel
		{
			get { return mTopBarMoreLabel; }
			set
			{
				if (value.Equals(mTopBarMoreLabel, StringComparison.Ordinal))
					return;

				mTopBarMoreLabel = value;
				OnPropertyChanged();
			}
		}

		public bool IsTopBarSubVisible
		{
			get { return mIsTopBarSubVisible; }
			set
			{
				mIsTopBarSubVisible = value;
				OnPropertyChanged();
			}
		}

		public List<TownFishMenuItem> OverflowImages { get; set; }

		#endregion

		#endregion

		#region Events

		public event EventHandler<string> CallbackRequested;

		#endregion

		#region Methods

		public void OnCallbackRequested(string callbackName)
		{
			if (CallbackRequested != null)
				CallbackRequested(this, callbackName);
		}

		public void LoadMenuMap(string baseUri, TownFishMenuMap map)
		{
			if (map.Menus.Bottom.display)
			{
				IsBottomBarVisible = true;

				// Bottom Menu
				if (map.Menus.Bottom.items.Count > 0 && map.Menus.Bottom.items[0] != null)
				{
					BottomAction1Label = GenerateLabel(map.Menus.Bottom.items[0]);
					BottomAction1Command = GenerateAction(map.Menus.Bottom.items[0]);
				}

				if (map.Menus.Bottom.items.Count > 0 && map.Menus.Bottom.items[1] != null)
				{
					BottomAction2Label = GenerateLabel(map.Menus.Bottom.items[1]);
					BottomAction2Command = GenerateAction(map.Menus.Bottom.items[1]);
				}

				if (map.Menus.Bottom.items.Count > 0 && map.Menus.Bottom.items[2] != null)
				{
					BottomAction3Label = GenerateLabel(map.Menus.Bottom.items[2]);
					BottomAction3Command = GenerateAction(map.Menus.Bottom.items[2]); ;
				}

				if (map.Menus.Bottom.items.Count > 0 && map.Menus.Bottom.items[3] != null)
				{
					BottomAction4Label = GenerateLabel(map.Menus.Bottom.items[3]);
					BottomAction4Command = GenerateAction(map.Menus.Bottom.items[3]);
				}

				if (map.Menus.Bottom.items.Count > 0 && map.Menus.Bottom.items[4] != null)
				{
					BottomAction5Label = GenerateLabel(map.Menus.Bottom.items[4]);
					BottomAction5Command = GenerateAction(map.Menus.Bottom.items[4]);
				}
			}
			else
			{
				IsBottomBarVisible = false;
			}

			if(map.Menus.Top.display)
			{
				IsTopBarVisible = true;

				TopBarLeftLabel = GenerateLabel(map.Menus.Top.items[0]);
				TopBarLeftCommand = GenerateAction(map.Menus.Top.items[0]);

				PageTitle = GenerateLabel(map.Menus.Top.items[1]);

				TopBarRightLabel = GenerateLabel(map.Menus.Top.items[2]);
				TopBarRightCommand = GenerateAction(map.Menus.Top.items[2]);

				if (map.Menus.Top.items.Count == 4)
				{
					TopBarRight1Label = GenerateLabel(map.Menus.Top.items[3]);
					TopBarRight1Command = GenerateAction(map.Menus.Top.items[3]);
				}
				else
				{
					TopBarRight1Label = "";
					//TopBarRight1Command = new Command(() => { });
				}
			}
			else
			{
				IsTopBarVisible = false;
			}

			if (map.Menus.TopSub.display)
			{
				IsTopBarSubVisible = true;

				// Top Menu
				if (map.Menus.TopSub.items.Count > 0 && map.Menus.TopSub.items[0] != null)
				{
					TopAction1Label = GenerateLabel(map.Menus.TopSub.items[0]);
					TopAction1Command = GenerateAction(map.Menus.TopSub.items[0]);
				}

				if (map.Menus.TopSub.items.Count > 0 && map.Menus.TopSub.items[1] != null)
				{
					TopAction2Label = GenerateLabel(map.Menus.TopSub.items[1]);
					TopAction2Command = GenerateAction(map.Menus.TopSub.items[1]);
				}

				if (map.Menus.TopSub.items.Count > 0 && map.Menus.TopSub.items[2] != null)
				{
					TopAction3Label = GenerateLabel(map.Menus.TopSub.items[2]);
					TopAction3Command = GenerateAction(map.Menus.TopSub.items[2]);
				}

				var moreIcon = map.Menus.TopSub.items.FirstOrDefault(x => x.type == "limitby");

				if (moreIcon != null)
				{
					TopActionMoreLabel = GenerateLabel(map.Menus.TopSub.items[3]);
					OverflowImages = map.Menus.TopSub.items.Skip(3).Where(i => i.type != "limitby").ToList();
				}
			}
			else
			{
				IsTopBarSubVisible = false;
			}
		}

		string GenerateLabel(TownFishMenuItem item)
		{
			if (item.kind == "icon")
			{
				var url = item.iconurl;

				url = url.Replace("{size}", item.size);
				url = url.Replace("{color}", item.color);

				return BaseUri + url;
			}
			else if(item.type == "heading")
			{
				return item.main;
			}
			else
			{
				return item.value;
			}
		}

		ICommand GenerateAction(TownFishMenuItem item)
		{
			if(item.type == "link")
			{
				return new Command(() =>
				{
					Source = BaseUri + item.href + AppModeParam;
				});
			}
			else if(item.type == "callback")
			{
				return new Command(() =>
				{
					OnCallbackRequested(item.value);
				});
			}
			else if(item.type == "heading")
			{
				//do nothing
				return null;
			}
			else
			{
				return null;
			}
		}

		public static BrowserPageViewModel Create(string baseUri, TownFishMenuMap map)
		{
			var viewModel = new BrowserPageViewModel
			{
				BaseUri = baseUri,
				OverflowImages = new List<TownFishMenuItem>()
			};

			if(map != null)
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
		ICommand mBottomAction1Command;
		string mBottomAction2Label;
		ICommand mBottomAction2Command;
		string mBottomAction3Label;
		ICommand mBottomAction3Command;
		string mBottomAction4Label;
		ICommand mBottomAction4Command;
		string mBottomAction5Label;
		ICommand mBottomAction5Command;
		bool mIsLoading;
		bool mIsTopBarVisible;
		bool mIsBottomBarVisible;
		string mTopBarLeftLabel;
		ICommand mTopBarLeftCommand;
		string mTopBarRightLabel;
		ICommand mTopBarRightCommand;
		bool mIsTopBarSubVisible;

		string mTopBarMoreLabel;
		ICommand mTopBarMoreCommand;
		ICommand mTopAction1Command;
		ICommand mTopAction2Command;
		ICommand mTopAction3Command;
		ICommand mTopBarRight1Command;
		string mTopBarRight1Label;

		#endregion
	}
}
