using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using TownFish.App.Droid.Renderers;
using TownFish.App.Controls;
using Android.Graphics.Drawables;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(Badge), typeof(BadgeRenderer))]

namespace TownFish.App.Droid.Renderers
{
    public class BadgeRenderer : LabelRenderer
    {
        // Override the OnElementChanged method so we can tweak this renderer post-initial setup
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.SetPadding(12,2,12,2);
                Control.SetIncludeFontPadding(false);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "BadgeColor")
            {
                GradientDrawable shape = new GradientDrawable();
                shape.SetCornerRadius(16);
                var q = (Element as Badge);
                shape.SetColor(q.BadgeColor.ToAndroid());

                Control.Background = shape;
            }

        }
    }
}
