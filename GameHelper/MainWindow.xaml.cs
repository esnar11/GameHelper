using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GameHelper.Interfaces;
using GameHelper.UserControls;
using GameHelper.Windows;
using SharpPcap;
using AppContext = GameHelper.Engine.AppContext;

namespace GameHelper
{
    public partial class MainWindow
    {
        private readonly ICollection<GameWindow> _windows = new List<GameWindow>();
        private readonly AppContext _appContext = new AppContext();

        private ICaptureDevice SelectedDevice
        {
            get
            {
                if (string.IsNullOrEmpty(Settings.Default.SelectedDevice))
                    return null;

                return CaptureDeviceList.Instance.FirstOrDefault(d => d.Name == Settings.Default.SelectedDevice);
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            foreach (var device in CaptureDeviceList.Instance)
            {
                var deviceMenuItem = new MenuItem { Header = device.Description, Tag = device };
                if (SelectedDevice != null)
                    deviceMenuItem.IsChecked = device.Name == SelectedDevice.Name;
                deviceMenuItem.Click += DeviceMenuItem_Click;
                _miDevices.Items.Add(deviceMenuItem);
            }

            var sources = new IGameSource[]
            {
                new Albion.GameSource()
            };
            foreach (var gameSource in sources.OrderByDescending(gs => gs.Name))
            {
                var menuItem = new MenuItem { Header = gameSource.Name, IsCheckable = true, DataContext = gameSource };
                menuItem.Checked += OnConnect;
                _miGame.Items.Insert(0, menuItem);
            }

            TuneControls();
        }

        private void DeviceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var mi = (MenuItem) sender;
            var device = (ICaptureDevice) mi.Tag;

            foreach (MenuItem devicesItem in _miDevices.Items)
                devicesItem.IsChecked = false;

            mi.IsChecked = true;
            Settings.Default.SelectedDevice = device.Name;
            Settings.Default.Save();
        }

        private void OnConnect(object sender, RoutedEventArgs e)
        {
            foreach (var menuItem in _miGame.Items.OfType<MenuItem>())
                if (menuItem != sender)
                    menuItem.IsChecked = false;

            try
            {
                Cursor = Cursors.Wait;

                _appContext.GameSource  = (IGameSource) ((MenuItem) sender).DataContext;
                _appContext.GameSource.Connect(SelectedDevice.Name);
                TuneControls();
            }
            catch (Exception exception)
            {
                App.ShowError(exception);
            }
            finally
            {
                Cursor = null;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _appContext.GameSource?.Disconnect();

            foreach (var window in _windows.ToArray())
                window.Close();

            base.OnClosing(e);
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnDisconnectClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;

                _appContext.GameSource.Disconnect();
                _appContext.GameSource = null;
            }
            finally
            {
                Cursor = null;
            }

            foreach (var menuItem in _miGame.Items.OfType<MenuItem>())
                menuItem.IsChecked = false;
            TuneControls();
        }

        private void TuneControls()
        {
            _miDiconnect.IsEnabled = _appContext.GameSource != null;
            _miAnalyze.IsEnabled = _appContext.GameSource != null;
        }

        private void OnAnalyzeIncomeClick(object sender, RoutedEventArgs e)
        {
            new AnalyzeLowWindow(_appContext.GameSource.IncomeLowEventsSource).Show();
        }

        private void OnAnalyzeOutcomeClick(object sender, RoutedEventArgs e)
        {
            new AnalyzeLowWindow(_appContext.GameSource.OutcomeLowEventsSource).Show();
        }

        private void OnChatClick(object sender, RoutedEventArgs e)
        {
            if (_appContext.GameSource?.EventsSource == null)
                return;

            var settings = new ChatsSettings
            {
                Game = _appContext.GameSource.Name,
                StopWords = new []
                {
                    "Набор", "Тяночк", "в ги", "принимаем"
                },
                HighlightWords = new [] { "танк", "групп", "данж" }
            };

            var chatsWindow = new GameWindow(new ChatsControl(_appContext.GameSource.EventsSource, settings))
            {
                Tag = nameof(ChatsControl),
                Width = 400,
                Height = 300
            };
            chatsWindow.Closed += GameWindow_Closed;
            chatsWindow.Show();
            _windows.Add(chatsWindow);
        }

        private void GameWindow_Closed(object sender, EventArgs e)
        {
            _windows.Remove((GameWindow)sender);
        }

        private void OnCreateNameClick(object sender, RoutedEventArgs e)
        {
            new CreateNameWindow().Show();
        }

        private void OnDPSClick(object sender, RoutedEventArgs e)
        {
            if (_appContext.GameSource?.EventsSource == null)
                return;

            var dpsWindow = new GameWindow(new MeterControl(_appContext))
            {
                Tag = nameof(MeterControl),
                Width = 400,
                Height = 300
            };
            dpsWindow.Closed += GameWindow_Closed;
            dpsWindow.Show();
            _windows.Add(dpsWindow);
        }

        private void OnBufsClick(object sender, RoutedEventArgs e)
        {
            if (_appContext.GameSource?.EventsSource == null)
                return;

            var buffsWindow = new GameWindow(new BuffsControl(_appContext))
            {
                Tag = nameof(BuffsControl),
                Width = 300,
                Height = 200
            };
            buffsWindow.Closed += GameWindow_Closed;
            buffsWindow.Show();
            _windows.Add(buffsWindow);
        }

        private void OnYourselfHPClick(object sender, RoutedEventArgs e)
        {
            if (_appContext.GameSource.Avatar == null)
                return;

            var window = new GameWindow(new RangeControl { Range = _appContext.GameSource.Avatar.HP })
            {
                Tag = nameof(RangeControl) + "HP",
                Width = 300,
                Height = 50
            };
            window.Closed += GameWindow_Closed;
            window.Show();
            _windows.Add(window);
        }

        private void OnChatHistoryClick(object sender, RoutedEventArgs e)
        {
            new ChatHistoryWindow(new IChatHistoryReadonly[0]).Show();
        }

        private void OnUdpTrafficAnalyze(object sender, RoutedEventArgs e)
        {
            new UdpTrafficAnalyzeWindow(_appContext.GameSource.UDPs).Show();
        }
    }
}
