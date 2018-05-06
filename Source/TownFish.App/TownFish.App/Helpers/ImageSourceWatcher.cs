using System;

using Xamarin.Forms;


namespace TownFish.App.Helpers
{
	public class ImageSourceWatcher
	{
		#region ImageLoaded Attached Property

		public static readonly BindableProperty ImageLoadedHandlerProperty =
			BindableProperty.CreateAttached ("ImageLoaded", typeof (EventHandler<bool>),
				typeof (ImageSource), default (EventHandler<bool>));
			//BindableProperty.CreateAttached<ImageSource, EventHandler<bool>> (b =>
			//	GetImageLoadedHandler (b), default (EventHandler<bool>), BindingMode.OneWay);

		public static EventHandler<bool> GetImageLoadedHandler (
				BindableObject bo) =>
					(EventHandler<bool>) bo.GetValue (ImageLoadedHandlerProperty);

		public static void SetImageLoadedHandler (
				BindableObject bo, EventHandler<bool> value) =>
					bo.SetValue (ImageLoadedHandlerProperty, value);

		#endregion ImageLoaded Attached Property
	}
}
