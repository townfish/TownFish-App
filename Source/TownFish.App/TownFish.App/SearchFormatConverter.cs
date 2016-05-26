using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using TownFish.App.Models;
using TownFish.App.ViewModels;

namespace TownFish.App
{
	public class SearchFormatConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				var item = value.ToString();
				var searchTerm = parameter.ToString();

				var formattedOutput = new FormattedString();

				HighlightTerm(item, BrowserPageViewModel.sSearchTerm, ref formattedOutput);

				return formattedOutput;
			}
			catch(Exception)
			{
				return new FormattedString();
			}
		}

		void HighlightTerm(string word, string searchTerm, ref FormattedString output)
		{
			if (word.ToLower().Contains(searchTerm.ToLower()))
			{
				// Before search term
				var index = word.ToLower().IndexOf(searchTerm.ToLower());
				if (index > 0)
				{
					output.Spans.Add(new Span
					{
						Text = word.Substring(0, index)
					});
				}

				// search term
				output.Spans.Add(new Span
				{
					Text = word.Substring(index, searchTerm.Length),
					ForegroundColor = Color.Black
				});

				// after search term
				if ((index + searchTerm.Length) < word.Length)
				{
					output.Spans.Add(new Span
					{
						Text = word.Substring(index + searchTerm.Length)
					});
				}
			}
			else
			{
				output.Spans.Add(new Span
				{
					Text = word,
					FontAttributes = FontAttributes.Bold,
					ForegroundColor = Color.Black
				});
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
