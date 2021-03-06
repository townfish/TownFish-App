﻿using System;

using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;

using Android.Graphics;

using TownFish.App.Droid.Renderers;
using TownFish.App.Controls;
using Android.Content;

[assembly: ExportRenderer (typeof (Badge), typeof (BadgeRenderer))]


namespace TownFish.App.Droid.Renderers
{
	public class BadgeRenderer: VisualElementRenderer<Badge>
	{
		#region Construction

		public BadgeRenderer (Context context)
            : base (context)
		{
            mContext = context;

			SetWillNotDraw (false);
		}

		#endregion Construction

		#region Methods

		protected override void OnElementChanged (ElementChangedEventArgs<Badge> ecea)
		{
			base.OnElementChanged (ecea);

			if (ecea.NewElement != null && ecea.OldElement == null)
			{
				// whenever a relevant property changes, force a redraw
				ecea.NewElement.PropertyChanged += (s, pcea) =>
					{
						var name = pcea.PropertyName;

						if (name == nameof (Badge.Colour) ||
								name == nameof (Badge.CornerRadius) ||
								name == nameof (Badge.Width) ||
								name == nameof (Badge.Height))
							Invalidate();
					};

				// and start off with one for good measure
				Invalidate();
			}
		}

		protected override void OnDraw (Canvas canvas)
		{
			var el = Element;

			var bounds = new Rect();
			GetDrawingRect (bounds);

			var w = bounds.Width();
			var h = bounds.Height();

			if (w <= 0 || h <= 0)
				return;

			var cornerRadius = el.CornerRadius;
			if (cornerRadius == -1f)
				cornerRadius = cDefaultCornerRadius;

			var r = mContext.ToPixels (cornerRadius);
			var c = el.Colour.ToAndroid();

			using (var paint = new Paint { AntiAlias = true })
				using (var path = new Path())
					using (Path.Direction direction = Path.Direction.Cw)
						using (Paint.Style style = Paint.Style.Fill)
							using (var rect = new RectF (0, 0, w, h))
							{
								path.AddRoundRect (rect, r, r, direction);

								paint.SetStyle (style);
								paint.Color = c;

								canvas.DrawPath (path, paint);
							}
		}

		#endregion Methods

		#region Fields

		const float cDefaultCornerRadius = 5;

        Context mContext;

		#endregion Fields
	}
}
