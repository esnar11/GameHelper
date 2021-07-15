using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GameHelper.Interfaces.LowLevel;
using GameHelper.Utils;

namespace GameHelper.Windows
{
    public partial class ParseBinaryWindow
    {
        private readonly IReadOnlyCollection<byte[]> _items;
        private readonly IBinarySearcher _binarySearcher = new BinarySearcher();

        public byte[] SelectedData => (byte[])_lb.SelectedItem;

        public ParseBinaryWindow()
        {
            InitializeComponent();

            _searchControl.ConditionsChanged += SearchConditionsChanged;
        }

        private void SearchConditionsChanged(Type type, string text)
        {
            IReadOnlyCollection<BinarySearchMatch> searchResults = null;
            if (!string.IsNullOrEmpty(text))
            {
                if (type == typeof(string))
                    searchResults = _binarySearcher.Search(_items, text);
                else if (type == typeof(byte))
                    searchResults = _binarySearcher.Search(_items, byte.Parse(text));
                else if (type == typeof(ushort))
                    searchResults = _binarySearcher.Search(_items, ushort.Parse(text));
                else
                    throw new NotImplementedException();

                if (!searchResults.Any())
                    MessageBox.Show(this, "Not found", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            _lb.ItemsSource = searchResults != null && searchResults.Any()
                ? searchResults.Select(r => r.Data)
                : _items;
            _dg.SearchMatches = searchResults != null && searchResults.Any() ? searchResults : null;
        }

        public ParseBinaryWindow(IReadOnlyCollection<byte[]> items): this()
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
            _lb.ItemsSource = items;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dg.Data = SelectedData;
            _te.BinaryData = SelectedData;
        }
    }

    public class BinaryDataConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = (byte[])value;
            if (targetType == typeof(string))
                return data.Length.ToString();

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
