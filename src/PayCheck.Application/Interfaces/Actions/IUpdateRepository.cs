namespace PayCheck.Application.Interfaces.Actions
{
    public interface IUpdateRepository<T> where T : class
    {
        T Update(T entity);
    }
}