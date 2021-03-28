﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GameHelper.Interfaces;
using GameHelper.UserControls;
using GameHelper.Windows;
using AppContext = GameHelper.Engine.AppContext;

namespace GameHelper
{
    public partial class MainWindow
    {
        private readonly ICollection<GameWindow> _windows = new List<GameWindow>();
        private readonly AppContext _appContext = new AppContext();

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

                _appContext.GameSource  = (IGameSource) ((MenuItem) sender).DataContext;
                _appContext.GameSource.Connect();
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
    }
}
