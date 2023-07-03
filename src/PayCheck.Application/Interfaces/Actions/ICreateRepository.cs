namespace PayCheck.Application.Interfaces.Actions
{
    public interface ICreateRepository<T> where T : class
    {
        T Create(T entity);
    }
}