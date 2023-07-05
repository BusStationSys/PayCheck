namespace PayCheck.Application.Interfaces.Actions
{
    public interface IDeleteRepository<in Y>
    {
        void Delete(Y pk);
    }
}