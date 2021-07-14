using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using GameHelper.Interfaces.LowLevel;

namespace GameHelper.UserControls
{
    public partial class DatagramControl
    {
        private byte[] _data;
        private IReadOnlyCollection<BinarySearchMatch> _searchMatches;

        public byte[] Data
        {
            get => _data;
            set
            {
                if (_data == value)
                    return;

                _data = value;

                if (_data == null)
                {
                    _ic.ItemsSource = null;
                    return;
                }

                var list = new List<ByteModel>();
                for (var i = 0; i < _data.Length; i++)
                {
                    var isHighlighted = SearchMatches != null && SearchMatches.Any(sm => sm.Data == _data && i >= sm.Position && i < sm.Position + sm.Length);
                    list.Add(new ByteModel(_data[i].ToString(), i, isHighlighted));
                }

                _ic.ItemsSource = list;
            }
        }

        public IReadOnlyCollection<BinarySearchMatch> SearchMatches
        {
            get => _searchMatches;
            set
            {
                if (_searchMatches == value)
                    return;

                _searchMatches = value;
            }
        }

        public DatagramControl()
        {
            InitializeComponent();
        }

        public class ByteModel
        {
            public string Value { get; }
            
            public int Position { get; }

            public bool IsHighlighted { get; }

            public ByteModel(string value, int position, bool isHighlighted)
            {
                Value = value;
                Position = position;
                IsHighlighted = isHighlighted;
            }
        }
    }

    public class DatagramConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DatagramControl.ByteModel datagram)
                if (targetType == typeof(Brush))
                    return datagram.IsHighlighted ? Brushes.Yellow: Brushes.Transparent;

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
