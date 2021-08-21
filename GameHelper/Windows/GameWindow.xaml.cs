using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GameHelper.Windows
{
    public partial class GameWindow
    {
        private const char PartSeparator = '|';
        private const char ValueSeparator = ';';
        private const double DSize = 0.025;

        private Point _startDragPoint;
        private Point _windowStartPoint;
        private Size _windowStartSize;
        private bool _resizing;

        public GameWindow()
        {
            InitializeComponent();
        }

        public GameWindow(UserControl userControl) : this()
        {
            if (userControl == null) throw new ArgumentNullException(nameof(userControl));

            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
            MouseMove += OnMouseMove;

            _border.Child = userControl;

            var pos = LoadPosition();
            if (pos != null)
            {
                Left = pos.Value.X;
                Top = pos.Value.Y;
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this);
            _startDragPoint = PointToScreen(position);
            _windowStartPoint = new Point(Left, Top);
            _windowStartSize = new Size(Width, Height);

            _resizing = position.X > ActualWidth * 0.9 && position.Y > ActualHeight * 0.9;

            CaptureMouse();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                var pos = PointToScreen(e.GetPosition(this));
                var dx = pos.X - _startDragPoint.X;
                var dy = pos.Y - _startDragPoint.Y;
                if (_resizing)
                {
                    Width = _windowStartSize.Width + dx;
                    Height = _windowStartSize.Height + dy;
                }
                else
                {
                    Left = _windowStartPoint.X + dx;
                    Top = _windowStartPoint.Y + dy;
                }
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
                SavePosition(new Point(Left, Top));
            }
        }

        private Point? LoadPosition()
        {
            var parts = string.IsNullOrEmpty(Settings.Default.WindowsPositions)
                ? new string[0]
                : Settings.Default.WindowsPositions.Split(PartSeparator);

            var tag = (string)Tag;

            foreach (var part in parts)
            {
                var values = part.Split(ValueSeparator);
                if (values[0] == tag)
                {
                    var x = int.Parse(values[1]);
                    var y = int.Parse(values[2]);
                    return new Point(x, y);
                }
            }

            return default;
        }

        private void SavePosition(Point position)
        {
            var parts = string.IsNullOrEmpty(Settings.Default.WindowsPositions)
                ? new List<string>()
                : Settings.Default.WindowsPositions.Split(PartSeparator).ToList();

            var tag = (string)Tag;

            string value = null;
            foreach (var part in parts)
            {
                var values = part.Split(ValueSeparator);
                if (values[0] == tag)
                {
                    value = part;
                    break;
                }
            }

            if (value != null)
                parts.Remove(value);

            parts.Add(string.Join(ValueSeparator,
                tag, (int)position.X, (int)position.Y));

            Settings.Default.WindowsPositions = string.Join(PartSeparator, parts);
            Settings.Default.Save();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Handled)
                return;

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                switch (e.Key)
                {
                    case Key.Left:
                        Width -= Width * DSize;
                        break;
                    case Key.Right:
                        Width += Width * DSize;
                        break;
                    case Key.Up:
                        Height -= Height * DSize;
                        break;
                    case Key.Down:
                        Height += Height * DSize;
                        break;
                }
        }
    }
}
