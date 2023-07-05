namespace PayCheck.Application.Interfaces.Actions
{
    using System.Collections.Generic;

    public interface IReadRepository<out T, in U> where T : class
    {
        IEnumerable<T> GetAll();

        T Get(U pk);
    }
}