namespace PayCheck.Application.Interfaces.Actions
{
    public interface ICreateRepository<out T, in Y> where T : class
    {
        T Create(Y entity);
    }
}