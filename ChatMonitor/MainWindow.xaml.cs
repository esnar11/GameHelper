using System.Linq;
using System.Windows.Input;

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
                var windows = new CapturableWindowDetector().FindWindows();
                _cbWindow.ItemsSource = windows.OrderBy(w => w.Name);
                _cbWindow.SelectedItem = windows.FirstOrDefault(w => w.Name.Contains("NewWorld.exe"));
            }
            finally
            {
                Cursor = null;
            }
        }
    }
}
