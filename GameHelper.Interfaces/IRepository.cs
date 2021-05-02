namespace GameHelper.Interfaces
{
    public interface IRepository<T>
    {
        T GetById(int Id);
    }
}
