using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TownFish.App.Controls;
using TownFish.App.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Badge), typeof(BadgeRenderer))]

namespace TownFish.App.iOS.Renderers
{
    public class BadgeRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (Element != null)
            {
                Layer.MasksToBounds = true;
                Layer.CornerRadius = 8;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "BadgeColor")
            {
                Layer.BackgroundColor = (Element as Badge).BadgeColor.ToUIColor().CGColor;
            }
        }
    }
}
