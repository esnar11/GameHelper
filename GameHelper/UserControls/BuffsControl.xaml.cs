using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using GameHelper.Interfaces;

namespace GameHelper.UserControls
{
    public partial class BuffsControl
    {
        private readonly IAppContext _appContext;
        private readonly ICollection<Buff> _buffs = new ObservableCollection<Buff>();
        private readonly Timer _timer = new Timer(TimeSpan.FromSeconds(0.05).TotalMilliseconds) { AutoReset = true };

        public BuffsControl()
        {
            InitializeComponent();
        }

        public BuffsControl(IAppContext appContext): this()
        {
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));

            _timer.Elapsed += OnTimer;
            Loaded += BuffsControl_Loaded;
            Unloaded += BuffsControl_Unloaded;

            _listBox.ItemsSource = _buffs;
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            this.Do(() =>
            {
                var toRemove = _buffs.Where(b => DateTime.UtcNow >= b.EndTime).ToArray();
                foreach (var buff in toRemove)
                    _buffs.Remove(buff);
            });
        }

        private void BuffsControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _appContext.GameSource.Connected += GameSource_Connected;
            _appContext.GameSource.Disconnected += GameSource_Disconnected;

            if (_appContext.GameSource.EventsSource != null)
                GameSource_Connected();
        }

        private void BuffsControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _appContext.GameSource.Connected -= GameSource_Connected;
            _appContext.GameSource.Disconnected -= GameSource_Disconnected;
        }

        private void GameSource_Connected()
        {
            _appContext.GameSource.EventsSource.BuffAdded += EventsSource_BuffAdded;
            _timer.Start();
        }

        private void GameSource_Disconnected()
        {
            _timer.Stop();
            _appContext.GameSource.EventsSource.BuffAdded -= EventsSource_BuffAdded;
        }

        private void EventsSource_BuffAdded(Buff buff)
        {
            this.Do(() =>
            {
                _buffs.Add(buff);
            });
        }
    }
}
