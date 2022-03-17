using System.Drawing;
using System.Linq;
using System.Windows.Controls;

namespace OpticalReader.Controls
{
    public partial class TuneCaptureAreaControl
    {
        private ICaptureEngineExt _captureEngine;
        private AppSettings _settings;

        public ICaptureEngineExt CaptureEngine
        {
            get => _captureEngine;
            set
            {
                if (_captureEngine == value)
                    return;

                _captureEngine = value;
            }
        }

        private CaptureArea SelectedArea => _cbArea.SelectedItem as CaptureArea;

        public AppSettings Settings
        {
            get => _settings;
            set
            {
                if (_settings == value)
                    return;

                _settings = value;

                _cbArea.ItemsSource = _settings.CaptureAreas.OrderBy(ca => ca.Name);
                _cbArea.SelectedItem = _settings.CaptureAreas.OrderBy(ca => ca.Name).FirstOrDefault();
            }
        }

        public TuneCaptureAreaControl()
        {
            InitializeComponent();

            Loaded += (sender, e) =>
            {
                if (CaptureEngine != null)
                    CaptureEngine.ImageCaptured += CaptureEngine_ImageCaptured;
            };
            Unloaded += (sender, e) =>
            {
                if (CaptureEngine != null)
                    CaptureEngine.ImageCaptured -= CaptureEngine_ImageCaptured;
            };
        }

        private void CaptureEngine_ImageCaptured(CaptureArea area, System.Windows.Media.Imaging.BitmapImage image)
        {
            if (area == SelectedArea)
                _img.Source = image;
        }

        private void OnAreaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _tbX.Text = SelectedArea.Point.X.ToString();
            _tbY.Text = SelectedArea.Point.Y.ToString();
            _tbWidth.Text = SelectedArea.Size.Width.ToString();
            _tbHeight.Text = SelectedArea.Size.Height.ToString();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoaded)
            {
                SelectedArea.Point = new Point(int.Parse(_tbX.Text), int.Parse(_tbY.Text));
                SelectedArea.Size = new Size(int.Parse(_tbWidth.Text), int.Parse(_tbHeight.Text));
            }
        }
    }
}
