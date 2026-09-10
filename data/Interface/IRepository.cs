namespace data.Interface;

public interface IRepository<T> where T : class
{
    Task Add(T entity);
    void Update(T entity);
}