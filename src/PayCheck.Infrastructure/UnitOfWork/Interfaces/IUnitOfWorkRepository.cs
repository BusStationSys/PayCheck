namespace PayCheck.Infrastructure.UnitOfWork.Interfaces
{
    using PayCheck.Application.Interfaces.Repository;

    public interface IUnitOfWorkRepository
    {
        IMatriculaDemonstrativoPagamentoRepository MatriculaDemonstrativoPagamentoRepository { get; }

        IPessoaFisicaRepository PessoaFisicaRepository { get; }

        IUsuarioRepository UsuarioRepository { get; }
    }
}