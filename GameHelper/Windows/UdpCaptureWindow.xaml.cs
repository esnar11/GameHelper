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

        private IPortListener SelectedPortListener => _cbPorts.SelectedItem as IPortListener;

        public UdpCaptureWindow()
        {
            InitializeComponent();
        }

        public UdpCaptureWindow(IReadOnlyCollection<IPortListener> udps) : this()
        {
            _cbPorts.ItemsSource = udps.OrderBy(u => u.Port);
            _cbPorts.SelectedItem = udps.OrderBy(u => u.Port).FirstOrDefault();

            TuneControls();

            _timer.Elapsed += OnTimer;
            _timer.Start();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _timer.Stop();

            if (_capturing)
                SelectedPortListener.DataCaptured -= OnNewData;

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
            _btnCaptureStart.IsEnabled = SelectedPortListener != null && !_capturing;
            _btnCaptureStop.IsEnabled = _capturing;
            _btnSave.IsEnabled = !_capturing && _dataStorage.Items.Any();
            _btnCaptureClear.IsEnabled = !_capturing && _dataStorage.Items.Any();
            _btnParse.IsEnabled = _dataStorage.Items.Any();
        }

        private void OnIncomeCaptureStartClick(object sender, RoutedEventArgs e)
        {
            _capturing = true;
            SelectedPortListener.DataCaptured += OnNewData;
            TuneControls();
        }

        private void OnIncomeCaptureStopClick(object sender, RoutedEventArgs e)
        {
            SelectedPortListener.DataCaptured -= OnNewData;
            _capturing = false;
            TuneControls();
        }

        private void OnNewData(Datagram datagram)
        {
            _dataStorage.Add(datagram);
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
