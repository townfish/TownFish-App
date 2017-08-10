using System;
using System.ComponentModel;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using TownFish.App.Controls;
using TownFish.App.iOS.Renderers;


[assembly: ExportRenderer (typeof (Badge), typeof (BadgeRenderer))]


namespace TownFish.App.iOS.Renderers
{
    public class BadgeRenderer : VisualElementRenderer<Badge>
	{
        protected override void OnElementChanged (ElementChangedEventArgs<Badge> e)
        {
            base.OnElementChanged(e);

            if (Element != null)
            {
                Layer.MasksToBounds = true;
				Layer.CornerRadius = (nfloat) Element.CornerRadius;
				Layer.ShadowOpacity = 0;
            }
        }

        protected override void OnElementPropertyChanged (object sender,
				PropertyChangedEventArgs pcea)
        {
            base.OnElementPropertyChanged (sender, pcea);

			if (Element is Badge badge)
			{
				var name = pcea.PropertyName;

				if (name == nameof (Badge.Colour))
					Layer.BackgroundColor = badge.Colour.ToUIColor().CGColor;
				else if (name == nameof (Badge.CornerRadius))
					Layer.CornerRadius = (nfloat) badge.CornerRadius;
			}
        }
	}
}
