using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownFish.App.ViewModels;

namespace TownFish.App.Models
{
	public class AvailableLocation : BaseViewModel
	{
		
		#region Properties

		public string id { get; set; }

		public string name { get; set; }

		public string url { get; set; }

		public bool IsSelected { get; set; }

		public bool IsLocked { get; set; }

		public string LeftImage
		{
			get { return mLeftImage; }
			set
			{
				mLeftImage = value;
				OnPropertyChanged();
			}
		}

		public string RightImage
		{
			get { return mRightImage; }
			set
			{
				mRightImage = value;
				OnPropertyChanged();
			}
		}

		#endregion


		#region Fields

		string mLeftImage;
		string mRightImage;

		#endregion
	}

	public class LocationIcons
	{
		#region Properties

		public string Lock { get; set; }

		public string Pin { get; set; }

		public string Tick { get; set; }

		#endregion
	}
}
