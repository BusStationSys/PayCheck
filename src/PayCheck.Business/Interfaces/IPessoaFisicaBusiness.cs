namespace PayCheck.Business.Interfaces
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;

    public interface IPessoaFisicaBusiness
    {
        PessoaFisicaResponseDto Get(Guid guid);
    }
}