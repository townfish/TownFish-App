using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TownFish.App.Controls
{
    public class Badge : Label
    {

        public static readonly BindableProperty BadgeColorProperty = BindableProperty.CreateAttached("BadgeColor", typeof(Color), typeof(Badge), Color.Black);

        public Color BadgeColor
        {
            get { return (Color)GetValue(BadgeColorProperty); }
            set { SetValue(BadgeColorProperty, value); }
        }

        public Badge()
        {
            FontSize = Device.OnPlatform<double>(6, 8, 8);
            HeightRequest = 16;
            WidthRequest = 16;
            VerticalTextAlignment = TextAlignment.Center;
            HorizontalTextAlignment = TextAlignment.Center;
            FontAttributes = FontAttributes.Bold;
        }
    }
}
