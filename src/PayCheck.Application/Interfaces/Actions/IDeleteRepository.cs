namespace PayCheck.Application.Interfaces.Actions
{
    public interface IDeleteRepository<Y>
    {
        void Delete(Y id);
    }
}