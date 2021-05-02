using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GameHelper.Interfaces;

namespace GameHelper.UserControls
{
    public partial class HealthChangesControl
    {
        private IAppContext _appContext;
        private IReadOnlyCollection<HealthChange> _healthChanges;

        public IAppContext AppContext
        {
            get => _appContext;
            set
            {
                if (_appContext == value)
                    return;

                _appContext = value;
            }
        }

        public IReadOnlyCollection<HealthChange> HealthChanges
        {
            get => _healthChanges;
            set
            {
                if (_healthChanges == value)
                    return;

                _healthChanges = value;

                if (_healthChanges != null && _healthChanges.Count > 0)
                {
                    var skillName = _appContext.SkillRepository.GetById(_healthChanges.First().SkillId).Name;
                    var damageSum = _healthChanges.Sum(ch => -ch.Value);

                    //var minTime = _healthChanges.Min(ch => ch.Time);
                    //var maxTime = _healthChanges.Max(ch => ch.Time);
                    //damageSum /= (float)(maxTime - minTime).TotalSeconds;

                    _tb.Text = $"{skillName}   ...   {MathF.Round(damageSum).ToString("### ### ### ###")}";
                }
                else
                    _tb.Text = string.Empty;
            }
        }

        public HealthChangesControl()
        {
            InitializeComponent();

            MouseRightButtonDown += HealthChangesControl_MouseRightButtonDown;
        }

        private void HealthChangesControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(string.Join(Environment.NewLine, HealthChanges.Select(hc => hc.Value)));
        }
    }
}
