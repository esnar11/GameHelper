using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using GameHelper.Engine;
using GameHelper.Engine.Impl;
using GameHelper.Interfaces;

namespace GameHelper.Windows
{
    public partial class AnalyzeLowWindow
    {
        private readonly ILowEventsSource _lowEventsSource;
        private readonly IEventsStorage _eventsStorage = new EventsStorage();
        private bool _capturing;
        private readonly Timer _timer = new Timer(TimeSpan.FromSeconds(1).TotalMilliseconds) { AutoReset = true };

        public AnalyzeLowWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _timer.Stop();

            if (_capturing)
                _lowEventsSource.Event -= OnEvent;

            base.OnClosing(e);
        }

        public AnalyzeLowWindow(ILowEventsSource lowEventsSource) : this()
        {
            _lowEventsSource = lowEventsSource ?? throw new ArgumentNullException(nameof(lowEventsSource));
            TuneControls();

            _timer.Elapsed += OnTimer;
            _timer.Start();
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            this.Do(() =>
            {
                _tbIncomeCount.Text = _eventsStorage.Count.ToString();
            });
        }

        private void TuneControls()
        {
            _btnCaptureStart.IsEnabled = !_capturing;
            _btnCaptureStop.IsEnabled = _capturing;
        }

        private void OnIncomeCaptureStartClick(object sender, RoutedEventArgs e)
        {
            _capturing = true;
            _lowEventsSource.Event += OnEvent;
            TuneControls();
        }

        private void OnIncomeCaptureStopClick(object sender, RoutedEventArgs e)
        {
            _lowEventsSource.Event -= OnEvent;
            _capturing = false;
            TuneControls();
        }

        private void OnEvent(ushort code, IReadOnlyDictionary<byte, object> parameters)
        {
            _eventsStorage.Add(code, parameters);
        }

        private void OnIncomeCaptureClearClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Clear?", "Clear?", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            _eventsStorage.Clear();
        }
    }
}
