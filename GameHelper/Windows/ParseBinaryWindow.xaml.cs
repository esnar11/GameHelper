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
        private readonly IReadOnlyCollection<Datagram> _items;
        private readonly IBinarySearcher _binarySearcher = new BinarySearcher();

        public Datagram SelectedData => (Datagram)_lb.SelectedItem;

        public DataDirection SelectedDirection => (DataDirection)_cbDirection.SelectedItem;

        public ParseBinaryWindow()
        {
            InitializeComponent();

            _cbDirection.ItemsSource = new[] { DataDirection.ClientToServer, DataDirection.ServerToClient };
            _cbDirection.SelectedItem = DataDirection.ServerToClient;

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
                : _items.Where(d => d.Direction == SelectedDirection);
            _dg.SearchMatches = searchResults != null && searchResults.Any() ? searchResults : null;
        }

        public ParseBinaryWindow(IReadOnlyCollection<Datagram> items): this()
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
            _lb.ItemsSource = items.Where(d => d.Direction == SelectedDirection);
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dg.Data = SelectedData;
            _te.Datagram = SelectedData;
        }

        private void OnDirectionSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_items == null)
                return;

            if (_searchControl.Conditions == null)
                _lb.ItemsSource = _items.Where(d => d.Direction == SelectedDirection);
            else
                SearchConditionsChanged(_searchControl.Conditions.Item1, _searchControl.Conditions.Item2);
        }
    }

    public class BinaryDataConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Datagram datagram)
                if (targetType == typeof(string))
                    return $"{datagram.Direction} {datagram.Data.Length}";

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
