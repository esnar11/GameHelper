using System.Windows;
using OpticalReader.Chat;
using OpticalReader.Winds;

namespace OpticalReader
{
    public partial class MainWindow
    {
        private readonly ICaptureEngineExt _captureEngine;
        private readonly Settings _settings = new Settings();
        private readonly IChatParser _chatParser;
        private readonly IChat _chat;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            _captureEngine = new CaptureEngine(_settings.CaptureAreas);
            _chatParser = new ChatParser(_captureEngine, "NW_Chat");
            _chat = new Chat.Chat(_chatParser, _settings.ChatSettings);
            _chatControl.Chat = _chat;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _captureEngine.Start();
        }

        private void OnSettingsClick(object sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow(_settings, _captureEngine) { Owner = this };
            window.ShowDialog();
        }
    }
}
