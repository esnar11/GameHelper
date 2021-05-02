using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows;
using GameHelper.Interfaces;

namespace GameHelper.UserControls
{
    public partial class MeterControl
    {
        private readonly Timer _timer = new Timer(TimeSpan.FromSeconds(1).TotalMilliseconds) { AutoReset = true };
        private readonly IAppContext _appContext;
        private readonly ObservableCollection<HealthChangesControl> _healthChangesControls = new ObservableCollection<HealthChangesControl>();

        public MeterControl()
        {
            InitializeComponent();
        }

        public MeterControl(IAppContext appContext): this()
        {
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));

            _ic.ItemsSource = _healthChangesControls;

            Loaded += DpsControl_Loaded;
            Unloaded += DpsControl_Unloaded;

            _timer.Elapsed += OnTimer;
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            if (_appContext == null)
                return;

            this.Do(() =>
            {
                var yourselfId = GetYourselfId();

                var dict = _appContext.HealthChanges.Objects
                    .Where(hc => hc.SourceId == yourselfId)
                    .GroupBy(hc => hc.SkillId)
                    .ToDictionary(g => g.Key, g => g.Sum(ch => -ch.Value));
                _tb.Text = string.Join(Environment.NewLine, dict
                    .OrderByDescending(p => p.Value)
                    .Select(pair => $"{_appContext.SkillRepository.GetById(pair.Key).Name}   ...   {MathF.Round(pair.Value, 1)}"));

                var groups = new Dictionary<int, Tuple<float, IReadOnlyCollection<HealthChange>>>();
                foreach (var gr in _appContext.HealthChanges.Objects
                    .Where(hc => hc.SourceId == yourselfId)
                    .GroupBy(hc => hc.SkillId))
                {
                    var sum = gr.Sum(ch => -ch.Value);
                    groups.Add(gr.Key, new Tuple<float, IReadOnlyCollection<HealthChange>>(sum, gr.ToArray()));
                }

                _healthChangesControls.Clear();
                foreach (var gr in groups.OrderByDescending(g => g.Value.Item1))
                    _healthChangesControls.Add(new HealthChangesControl
                    {
                        AppContext = _appContext,
                        HealthChanges = gr.Value.Item2
                    });
            });
        }

        private int GetYourselfId()
        {
            var groups = _appContext.HealthChanges.Objects
                .GroupBy(hc => hc.SourceId)
                .OrderByDescending(g => g.Count())
                .ToArray();
            var first = groups.FirstOrDefault();
            return first?.Key ?? default;
        }

        private void DpsControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _timer.Start();
        }

        private void DpsControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _timer.Stop();
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            _appContext.HealthChanges.Clear();
        }

        private void OnCopyClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(_tb.Text);
        }
    }
}
