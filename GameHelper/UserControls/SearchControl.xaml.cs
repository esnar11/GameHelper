using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace GameHelper.UserControls
{
    public partial class SearchControl
    {
        public Type SelectedType => (Type) _cb.SelectedItem;

        public event Action<Type, string> ConditionsChanged;

        public SearchControl()
        {
            InitializeComponent();

            _cb.ItemsSource = new[]
            {
                typeof(string), 
                typeof(byte), 
                typeof(ushort), 
                typeof(short), 
                typeof(uint), 
                typeof(int), 
                typeof(ulong), 
                typeof(long)
            }.OrderBy(t => t.Name);
            _cb.SelectedItem = typeof(string);
        }

        private void _tb_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                if (TryParse(SelectedType, _tb.Text))
                    ConditionsChanged?.Invoke(SelectedType, _tb.Text);
        }

        private bool TryParse(Type type, string text)
        {
            if (type == typeof(string))
                return !string.IsNullOrWhiteSpace(text);

            if (type == typeof(byte))
                return byte.TryParse(text, out _);

            if (type == typeof(ushort))
                return ushort.TryParse(text, out _);

            if (type == typeof(short))
                return short.TryParse(text, out _);

            if (type == typeof(ulong))
                return uint.TryParse(text, out _);

            if (type == typeof(long))
                return int.TryParse(text, out _);

            throw new NotImplementedException();
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            ConditionsChanged?.Invoke(SelectedType, null);
        }
    }
}
