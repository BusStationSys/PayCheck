namespace PayCheck.Infrastructure.UnitOfWork.Interfaces
{
    public interface IUnitOfWork
    {
        IUnitOfWorkAdapter Create();
    }
}