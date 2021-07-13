using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using GameHelper.Engine.Impl;
using GameHelper.Interfaces.LowLevel;
using Microsoft.Win32;

namespace GameHelper.Windows
{
    public partial class UdpCaptureWindow
    {
        protected internal const string DefaultUdpFileExt = "udp";
        private readonly IUdpDataStorage _dataStorage = new UdpDataStorage();
        private bool _capturing;
        private readonly Timer _timer = new Timer(TimeSpan.FromSeconds(1).TotalMilliseconds) { AutoReset = true };

        private IUDP SelectedUDP => _cbPorts.SelectedItem as IUDP;

        public UdpCaptureWindow()
        {
            InitializeComponent();
        }

        public UdpCaptureWindow(IReadOnlyCollection<IUDP> udps) : this()
        {
            _cbPorts.ItemsSource = udps.OrderBy(u => u.Port);

            TuneControls();

            _timer.Elapsed += OnTimer;
            _timer.Start();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _timer.Stop();

            if (_capturing)
                SelectedUDP.OnData -= OnNewData;

            base.OnClosing(e);
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            this.Do(() =>
            {
                _tbIncomeCount.Text = _dataStorage.Count.ToString();
            });
        }

        private void TuneControls()
        {
            _cbPorts.IsEnabled = !_capturing;
            _btnCaptureStart.IsEnabled = SelectedUDP != null && !_capturing;
            _btnCaptureStop.IsEnabled = _capturing;
            _btnSave.IsEnabled = !_capturing;
            _btnLoad.IsEnabled = !_capturing;
            _btnParse.IsEnabled = _dataStorage.Items.Any();
        }

        private void OnIncomeCaptureStartClick(object sender, RoutedEventArgs e)
        {
            _capturing = true;
            SelectedUDP.OnData += OnNewData;
            TuneControls();
        }

        private void OnIncomeCaptureStopClick(object sender, RoutedEventArgs e)
        {
            SelectedUDP.OnData -= OnNewData;
            _capturing = false;
            TuneControls();
        }

        private void OnNewData(byte[] data)
        {
            _dataStorage.Add(data);
        }

        private void OnCaptureClearClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Clear?", "Clear?", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            _dataStorage.Clear();
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new SaveFileDialog { DefaultExt = DefaultUdpFileExt };
            if (fileDialog.ShowDialog(this) == true)
            {
                using (var file = new FileStream(fileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                    _dataStorage.Save(file);
                MessageBox.Show(this, "Data saved.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnLoadClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog { DefaultExt = DefaultUdpFileExt };
            if (fileDialog.ShowDialog(this) == true)
            {
                using (var file = new FileStream(fileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    _dataStorage.Load(file);
                TuneControls();
            }
        }

        private void OnPortChanged(object sender, SelectionChangedEventArgs e)
        {
            _dataStorage.Clear();
            TuneControls();
        }

        private void OnParseClick(object sender, RoutedEventArgs e)
        {
            new ParseBinaryWindow(_dataStorage.Items) { Owner = this }.ShowDialog();
        }
    }
}
