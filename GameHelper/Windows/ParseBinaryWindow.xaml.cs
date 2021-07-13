using System;
using System.Collections.Generic;
using System.Globalization;
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

            _searchControl.ConditionsChanged += _searchControl_ConditionsChanged;
        }

        private void _searchControl_ConditionsChanged(Type type, string text)
        {
            var searchResults = _binarySearcher.Search(_items, text);

            throw new NotImplementedException();
        }

        public ParseBinaryWindow(IReadOnlyCollection<byte[]> items): this()
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
            _lb.ItemsSource = items;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
