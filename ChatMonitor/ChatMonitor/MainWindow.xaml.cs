using System.Linq;
using System.Windows.Input;
using ChatMonitor.Capture;

namespace ChatMonitor
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;

                var windowDetector = new CapturableWindowDetector();
                var windows = windowDetector.FindWindows();
                windowDetector.SelectedWindow = windows.FirstOrDefault(w => w.Name.Contains("NewWorld.exe"));
                _cbWindow.ItemsSource = windows.OrderBy(w => w.Name);
                _cbWindow.SelectedItem = windowDetector.SelectedWindow;

                ICaptureMethod captureMethod = new GraphicsCapture(windowDetector);
                captureMethod.Equals(null);
            }
            finally
            {
                Cursor = null;
            }
        }
    }
}
