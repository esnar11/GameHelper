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

namespace GameHelper
{
    public partial class MainWindow
    {
        private IGameSource _gameSource;
        private readonly ICollection<GameWindow> _windows = new List<GameWindow>();

        public MainWindow()
        {
            InitializeComponent();

            var sources = new IGameSource[]
            {
                new Allods.GameSource(),
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

        private void OnConnect(object sender, RoutedEventArgs e)
        {
            foreach (var menuItem in _miGame.Items.OfType<MenuItem>())
                if (menuItem != sender)
                    menuItem.IsChecked = false;

            try
            {
                Cursor = Cursors.Wait;

                _gameSource = (IGameSource) ((MenuItem) sender).DataContext;
                _gameSource.Connect();
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
            _gameSource?.Disconnect();

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

                _gameSource.Disconnect();
                _gameSource = null;
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
            _miDiconnect.IsEnabled = _gameSource != null;
            _miAnalyze.IsEnabled = _gameSource != null;
        }

        private void OnAnalyzeIncomeClick(object sender, RoutedEventArgs e)
        {
            new AnalyzeLowWindow(_gameSource.IncomeLowEventsSource).Show();
        }

        private void OnAnalyzeOutcomeClick(object sender, RoutedEventArgs e)
        {
            new AnalyzeLowWindow(_gameSource.OutcomeLowEventsSource).Show();
        }

        private void OnChatClick(object sender, RoutedEventArgs e)
        {
            if (_gameSource?.EventsSource == null)
                return;

            var settings = new ChatsSettings
            {
                Game = _gameSource.Name,
                StopWords = new []
                {
                    "Набор", "Тяночк", "в ги", "принимаем"
                },
                HighlightWords = new [] { "танк", "групп", "данж" }
            };

            var chatsWindow = new GameWindow(new ChatsControl(_gameSource.EventsSource, settings))
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
    }
}
