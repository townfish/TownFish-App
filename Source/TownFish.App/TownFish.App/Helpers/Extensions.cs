using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;

namespace TownFish.App.Helpers
{
    public static class Extensions
	{
        public static string GetMD5Hash(string input)
        {
            var data = Encoding.UTF8.GetBytes(input);
            MD5Digest hash = new MD5Digest();
            hash.BlockUpdate(data, 0, data.Length);
            byte[] result = new byte[hash.GetDigestSize()];
            hash.DoFinal(result, 0);
            return Hex.ToHexString(result);
        }
    }

    public class ObservableRangeCollection<T> : ObservableCollection<T>
    {
        public ObservableRangeCollection() : base() { }

        public ObservableRangeCollection(IEnumerable<T> collection) : base(collection) { }

        public ObservableRangeCollection(List<T> list) : base(list) { }

        public void AddRange(IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                Items.Add(item);
            }

            this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Reset(IEnumerable<T> range)
        {
            this.Items.Clear();

            AddRange(range);
        }
    }
}

