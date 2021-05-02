namespace GameHelper.Interfaces
{
    public interface IAppContext
    {
        ICollectionBase<HealthChange> HealthChanges { get; }

        IGameSource GameSource { get; }

        IRepository<SkillInfo> SkillRepository { get; }
    }
}
