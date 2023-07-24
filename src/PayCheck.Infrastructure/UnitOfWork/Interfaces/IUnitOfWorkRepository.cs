namespace PayCheck.Infrastructure.UnitOfWork.Interfaces
{
    using PayCheck.Application.Interfaces.Repository;

    public interface IUnitOfWorkRepository
    {
        IMatriculaDemonstrativoPagamentoRepository MatriculaDemonstrativoPagamentoRepository { get; }

        IUsuarioRepository UsuarioRepository { get; }
    }
}