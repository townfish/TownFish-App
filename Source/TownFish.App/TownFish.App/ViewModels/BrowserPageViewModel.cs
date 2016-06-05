using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows.Input;

using Newtonsoft.Json;

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

		public bool BottomAction2HasNumber
		{
			get { return mBottomAction2HasNumber;  }
			set
			{
				mBottomAction2HasNumber = value;
				OnPropertyChanged();
			}
		}

		public string BottomAction2Number
		{
			get { return mBottomAction2Number; }
			set
			{
				mBottomAction2Number = value;
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

		public bool BottomAction3HasNumber
		{
			get { return mBottomAction3HasNumber; }
			set
			{
				mBottomAction3HasNumber = value;
				OnPropertyChanged();
			}
		}

		public string BottomAction3Number
		{
			get { return mBottomAction3Number; }
			set
			{
				mBottomAction3Number = value;
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

		#region TopForm

		public string TopFormLeftActionLabel
		{
			get { return mTopFormLeftActionLabel; }
			set
			{
				if (value.Equals(mTopFormLeftActionLabel, StringComparison.Ordinal))
					return;

				mTopFormLeftActionLabel = value;
				OnPropertyChanged();
			}
		}

		public ICommand TopFormLeftAction
		{
			get { return mTopFormLeftAction; }
			set
			{
				mTopFormLeftAction = value;
				OnPropertyChanged();
			}
		}

		public string TopFormRightActionLabel
		{
			get { return mTopFormRightActionLabel; }
			set
			{
				mTopFormRightActionLabel = value;
				OnPropertyChanged();
			}
		}

		public ICommand TopFormRightAction
		{
			get { return mTopFormRightAction; }
			set
			{
				mTopFormRightAction = value;
				OnPropertyChanged();
			}
		}

		public bool IsTopFormBarVisible
		{
			get { return mIsTopFormBarVisible; }
			set
			{
				mIsTopFormBarVisible = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Search
		public List<TownfishLocationItem> LocationSearchItems
		{
			get { return mLocationSearchItems; }
			set
			{
				mLocationSearchItems = value;
				OnPropertyChanged();
			}
		}

		public bool SearchPanelVisible
		{
			get { return mTopSearchPanelVisible; }
			set
			{
				mTopSearchPanelVisible = value;
				OnPropertyChanged();
			}
		}

		public bool SearchLocationHasResults
		{
			get { return mSearchLocationHasResults; }
			set
			{
				mSearchLocationHasResults = value;
				OnPropertyChanged();
			}
		}

		public ICommand CancelSearchCommand
		{
			get
			{
				return new Command(() =>
				{
					SearchLocationHasResults = false;
					SearchTerm = "";
				});
			}
		}

		public bool SearchHasContent
		{
			get { return mSearchHasContent; }
			set
			{
				mSearchHasContent = value;
				OnPropertyChanged();
			}
		}

		public string SearchTerm
		{
			get { return mSearchTerm; }
			set
			{
				mSearchTerm = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#endregion

		#region Events

		public event EventHandler<string> CallbackRequested;

		public event EventHandler<string> MenuRendered;

		#endregion

		#region Methods

		void OnMenuRendered()
		{
			MenuRendered?.Invoke(this, "");
		}

		public void OnCallbackRequested(string callbackName)
		{
			CallbackRequested?.Invoke(this, callbackName);
		}

		public void LoadMenuMap(string baseUri, TownFishMenuMap map)
		{
			mLocationApiFormat = map.LocationsAPI;
			mLocationSetFormat = map.LocationSetUrl;

			if (map.Menus.Bottom != null && map.Menus.Bottom.display)
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

					if(!string.IsNullOrEmpty(map.Menus.Bottom.items[1].super))
					{
						BottomAction2HasNumber = true;
						BottomAction2Number = map.Menus.Bottom.items[1].super;
					}
				}

				if (map.Menus.Bottom.items.Count > 0 && map.Menus.Bottom.items[2] != null)
				{
					BottomAction3Label = GenerateLabel(map.Menus.Bottom.items[2]);
					BottomAction3Command = GenerateAction(map.Menus.Bottom.items[2]); ;

					if (!string.IsNullOrEmpty(map.Menus.Bottom.items[2].super))
					{
						BottomAction3HasNumber = true;
						BottomAction3Number = map.Menus.Bottom.items[2].super;
					}
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

			if (map.Menus.Top != null && map.Menus.Top.display)
			{
				IsTopBarVisible = true;

				TopBarLeftLabel = GenerateLabel(map.Menus.Top.items[0]);

				// TODO: detect that this is location, Paul is to add a new type in schema for this
				//TopBarLeftCommand = GenerateAction(map.Menus.Top.items[0]);
				TopBarLeftCommand = new Command(() =>
				{
					//SearchPanelVisible = !SearchPanelVisible;
					SearchLocationHasResults = false;
				});

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

			if (map.Menus.TopSub != null && map.Menus.TopSub.display)
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

			if (map.Menus.TopForm != null && map.Menus.TopForm.display)
			{
				if (map.Menus.TopForm.items.Count >= 1)
				{
					TopFormLeftActionLabel = GenerateLabel(map.Menus.TopForm.items[0]);
					TopFormLeftAction = GenerateAction(map.Menus.TopForm.items[0]);
					IsTopFormBarVisible = true;
				}
				else
				{
					IsTopFormBarVisible = false;
				}

				if (map.Menus.TopForm.items.Count >= 2)
				{
					PageTitle = GenerateLabel(map.Menus.TopForm.items[1]);
				}

				if(map.Menus.TopForm.items.Count >= 3)
				{ 
					TopFormRightActionLabel = GenerateLabel(map.Menus.TopForm.items[2]);
					TopFormRightAction = GenerateAction(map.Menus.TopForm.items[2]);
				}
			}
			else
			{
				IsTopFormBarVisible = false;
			}

			OnMenuRendered();
		}

		string GenerateLabel(TownFishMenuItem item)
		{
			if (item.kind == "icon")
			{
				var url = item.iconurl;

				url = url.Replace("{size}", item.size);
				url = url.Replace("{color}", item.color);

				return cBaseUri + url;
			}
			else if (item.type == "heading")
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
			if (item.type == "link")
			{
				return new Command(() =>
				{
					Source = cBaseUri + item.href + cBaseUriParam;
				});
			}
			else if (item.type == "callback")
			{
				return new Command(() =>
				{
					OnCallbackRequested(item.value);
				});
			}
			else if(item.type == "back")
			{
				return new Command(() =>
				{
					Source = cBaseUri + item.href + cBaseUriParam;
				});
			}
			else
			{
				return null;
			}
		}

		public static BrowserPageViewModel Create(TownFishMenuMap map)
		{
			var viewModel = new BrowserPageViewModel
			{
				OverflowImages = new List<TownFishMenuItem>(),
				Source = cBaseUri + cBaseUriParam
			};

			if (map != null)
				viewModel.LoadMenuMap(cBaseUri, map);

			return viewModel;
		}

		public async void UpdateLocationList(string searchTerm)
		{
			sSearchTerm = searchTerm;

			try
			{
				using (var client = new HttpClient())
				{
					var resultJson = await client.GetStringAsync(cBaseUri + mLocationApiFormat.Replace("{term}", searchTerm));

					if (!string.IsNullOrEmpty(resultJson))
					{
						var model = JsonConvert.DeserializeObject<TownFishLocationList>(resultJson);

						if (model != null)
						{
							LocationSearchItems = model.items.ToList();
							SearchLocationHasResults = true;
						}
					}
				}
			}
			catch (Exception)
			{
				// TODO Alert the user of this crap up the wall
				SearchLocationHasResults = false;
			}
		}

		public void SetLocation(string cityId)
		{
			Source = cBaseUri + mLocationSetFormat.Replace("{id}", cityId) + cBaseUriParam;
		}

		#endregion

		#region Fields

		string mPageTitle;
		string mSource;
		bool mIsLoading;

		public const string cBaseUri = "http://dev.townfish.tk";
		string mLocationApiFormat = "";
		string mLocationSetFormat = "";
		public const string cBaseUriParam = "?mode=app";

		string mTopAction1Label;
		string mTopAction3Label;
		string mTopAction2Label;
		string mBottomAction1Label;
		string mBottomAction2Label;
		string mBottomAction3Label;
		string mBottomAction4Label;
		string mBottomAction5Label;
		string mTopBarLeftLabel;
		string mTopBarRightLabel;
		string mTopFormPageTitle;
		string mTopBarRight1Label;
		string mTopFormLeftActionLabel;
		string mTopBarMoreLabel;
		string mTopFormRightActionLabel;

		ICommand mTopBarMoreCommand;
		ICommand mTopAction1Command;
		ICommand mTopAction2Command;
		ICommand mTopAction3Command;
		ICommand mTopBarRight1Command;
		ICommand mTopFormLeftAction;
		ICommand mTopFormRightAction;
		ICommand mTopBarRightCommand;
		ICommand mTopBarLeftCommand;
		ICommand mBottomAction5Command;
		ICommand mBottomAction4Command;
		ICommand mBottomAction2Command;
		ICommand mBottomAction3Command;
		ICommand mBottomAction1Command;

		bool mIsTopFormBarVisible;
		bool mIsTopBarSubVisible;
		bool mIsTopBarVisible;
		bool mIsBottomBarVisible;

		bool mTopSearchPanelVisible;
		List<TownfishLocationItem> mLocationSearchItems;
		bool mSearchLocationHasResults;

		public static string sSearchTerm = "";
		bool mBottomAction2HasNumber;
		string mBottomAction2Number;
		bool mBottomAction3HasNumber;
		string mBottomAction3Number;
		bool mSearchHasContent;
		string mSearchTerm;

		#endregion
	}
}
