using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using TownFish.App.Models;
using TownFish.App.ViewModels;
using Newtonsoft.Json;

namespace TownFish.App
{
	public class SearchFormatConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				var item = value.ToString();

				// sadly converter parameter isn't bindable so we have to hack this with
				// a global (static) in the view model. Yuck.

				//var searchTerm = parameter.ToString();
				var searchTerm = BrowserPageViewModel.CurrentSearchTerm;

				var formattedOutput = new FormattedString();

				HighlightTerm (item, searchTerm, ref formattedOutput);

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

    //public class UberWebViewMessageConverter : JsonConverter
    //{
    //    public override bool CanConvert(Type objectType)
    //    {
    //        return objectType == typeof(UberWebViewMessage);
    //    }

    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        if (reader.TokenType == JsonToken.StartObject)
    //        {
    //            var dict = new Dictionary<string, string>();
    //            serializer.Populate(reader, dict);
    //            return new UberWebViewMessage(dict["action"], dict["result"]);
    //        }

    //        //if (reader.TokenType == JsonToken.String)
    //        //{
    //        //    return new UberWebViewMessage((string)reader.Value);
    //        //}

    //        return null;
    //    }

    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
