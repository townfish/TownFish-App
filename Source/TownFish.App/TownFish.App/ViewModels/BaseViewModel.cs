using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using TownFish.App.Models;

namespace TownFish.App.ViewModels
{
	public class BaseViewModel : INotifyPropertyChanged
	{
		#region Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Methods

		internal void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;

			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
