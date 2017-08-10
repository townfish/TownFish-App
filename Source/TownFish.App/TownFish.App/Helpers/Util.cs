using System;

using Xamarin.Forms;


namespace TownFish.App.Helpers
{
	static class Util
	{
		public static T OnPlatform<T> (T iOS, T android, T uwp = default (T)) =>
				Device.RuntimePlatform == Device.Android ? android :
				Device.RuntimePlatform == Device.iOS ? iOS :
				Device.RuntimePlatform == Device.UWP ? uwp :
				android; // assume all others are like Android
	}
}
