namespace PayCheck.Business.Interfaces
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;

    public interface IPessoaFisicaBusiness
    {
        PessoaFisicaResponse Get(Guid guid);
    }
}