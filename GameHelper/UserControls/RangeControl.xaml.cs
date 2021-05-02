using System;
using System.Windows.Media;
using GameHelper.Interfaces;

namespace GameHelper.UserControls
{
    public partial class RangeControl
    {
        private RangeF _range;

        public RangeF Range
        {
            get => _range;
            set
            {
                if (value == _range)
                    return;

                if (_range != null)
                    _range.ValueChanged -= ValueChanged;

                _range = value;

                if (_range != null)
                {
                    _range.ValueChanged += ValueChanged;
                    ValueChanged(_range, _range.Value, _range.Value);
                }
            }
        }

        /// <summary>
        /// Начиная с какого порога отображать кистью <see cref="BrushMin"/>
        /// </summary>
        public float Threshold { get; set; } = 0.25f;

        public SolidColorBrush BrushMin { get; set; } = Brushes.Red;

        public SolidColorBrush BrushMax { get; set; } = Brushes.Green;

        private void ValueChanged(RangeF range, float oldValue, float newValue)
        {
            this.Do(() =>
            {
                var v = range.NormalizedValue;

                _valueRect.Width = _border.ActualWidth * v;

                _valueRect.Fill = v < Threshold ? BrushMin : BrushMax;
                _border.BorderBrush = _valueRect.Fill;
                //_bkRect.Fill = _valueRect.Fill;

                _text.Text = $"{MathF.Round(newValue)} / {MathF.Round(range.Max)}";

                //var color = ((SolidColorBrush)_valueRect.Fill).Color;
                //_text.Foreground = color.R + color.G + color.B < 128 * 3
                //    ? Brushes.White
                //    : Brushes.Black;

                _valueRect.ToolTip = _text.Text;
                _border.ToolTip = _valueRect.ToolTip;
                _bkRect.ToolTip = _valueRect.ToolTip;
            });
        }

        public RangeControl()
        {
            InitializeComponent();

            Loaded += RangeControl_Loaded;
            Unloaded += (sender, e) => Range = null;
        }

        private void RangeControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Range != null)
                ValueChanged(Range, Range.Value, Range.Value);
        }
    }
}
