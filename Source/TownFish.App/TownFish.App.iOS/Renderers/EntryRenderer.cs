using System;
using System.Collections.Generic;
using System.Text;
using TownFish.App.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Entry), typeof(TownFishEntryRenderer))]
namespace TownFish.App.iOS.Renderers
{
	public class TownFishEntryRenderer : EntryRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (Control == null) return;

			Control.BorderStyle = UIKit.UITextBorderStyle.None;
		}
	}
}
