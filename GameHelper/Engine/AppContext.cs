using GameHelper.Albion;
using GameHelper.Interfaces;

namespace GameHelper.Engine
{
    public class AppContext: IAppContext
    {
        private IGameSource _gameSource;

        public ICollectionBase<HealthChange> HealthChanges { get; } = new CollectionBase<HealthChange>();

        public IGameSource GameSource
        {
            get => _gameSource;
            set
            {
                if (_gameSource == value)
                    return;

                if (_gameSource != null)
                {
                    _gameSource.Connected -= _gameSource_Connected;
                    _gameSource.Disconnected -= _gameSource_Disconnected;
                }

                _gameSource = value;

                if (_gameSource != null)
                {
                    _gameSource.Connected += _gameSource_Connected;
                    _gameSource.Disconnected += _gameSource_Disconnected;
                }
            }
        }

        private void _gameSource_Disconnected()
        {
            _gameSource.EventsSource.HealthChange -= EventsSource_HealthChange;
        }

        private void _gameSource_Connected()
        {
            _gameSource.EventsSource.HealthChange += EventsSource_HealthChange;
        }

        private void EventsSource_HealthChange(HealthChange healthChange)
        {
            HealthChanges.Add(healthChange);
        }

        public IRepository<SkillInfo> SkillRepository { get; } = new SkillRepository();
    }
}
