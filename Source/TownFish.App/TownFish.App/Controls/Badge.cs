using System;

using Xamarin.Forms;

using TownFish.App.Helpers;


namespace TownFish.App.Controls
{
	public class Badge: ContentView
	{
		#region Construction

		public Badge()
		{
			BackgroundColor = Color.Transparent; // allows my renderer to draw over it
			HeightRequest = mSize;
			MinimumWidthRequest = mSize;
			CornerRadius = mSize / 2;
			Padding = new Thickness (Util.OnPlatform (4, 5), 0);

			Content = mLabel = new Label
			{
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center,
				FontSize = 10,
				FontAttributes = FontAttributes.Bold
			};
		}

		#endregion Construction

		#region Properties

		#region Colour BindableProperty

		public static readonly BindableProperty ColourProperty =
				BindableProperty.Create (nameof (Colour), typeof (Color),
						typeof (Badge), Color.Red,
						propertyChanged: (bo, ov, nv) =>
								(bo as Badge).mLabel.TextColor = (Color) nv);

		public Color Colour
		{
			get { return (Color) GetValue (ColourProperty); }
			set { SetValue (ColourProperty, value); }
		}

		#endregion Colour BindableProperty

		#region CornerRadius BindableProperty

		public static readonly BindableProperty CornerRadiusProperty =
				BindableProperty.Create (nameof (CornerRadius), typeof (double),
						typeof (Badge), 5.0);

		public double CornerRadius
		{
			get { return (double) GetValue (CornerRadiusProperty); }
			set { SetValue (CornerRadiusProperty, value); }
		}

		#endregion CornerRadius BindableProperty

		#region Text BindableProperty

		public static readonly BindableProperty TextProperty =
				BindableProperty.Create (nameof (Text), typeof (string),
						typeof (Badge), null,
						propertyChanged: (bo, ov, nv) =>
								(bo as Badge).mLabel.Text = nv as string);

		public string Text
		{
			get { return (string) GetValue (TextProperty); }
			set { SetValue (TextProperty, value); }
		}

		#endregion Text BindableProperty

		#region TextColour BindableProperty

		public static readonly BindableProperty TextColourProperty =
				BindableProperty.Create (nameof (TextColour), typeof (Color),
						typeof (Badge), Color.Red,
						propertyChanged: (bo, ov, nv) =>
								(bo as Badge).mLabel.TextColor = (Color) nv);

		public Color TextColour
		{
			get { return (Color) GetValue (TextColourProperty); }
			set { SetValue (TextColourProperty, value); }
		}

		#endregion TextColour BindableProperty

		#endregion Properties

		#region Fields

		float mSize = Util.OnPlatform<float> (14, 15, 15);
		Label mLabel;

		#endregion Fields
	}
}
