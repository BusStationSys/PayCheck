namespace PayCheck.Business.Interfaces
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;

    public interface IPessoaFisicaBusiness
    {
        void Delete(Guid guid);

        PessoaFisicaResponseDto Get(Guid guid);

        public IEnumerable<PessoaFisicaResponseDto> GetAll();

        PessoaFisicaResponseDto SaveData(PessoaFisicaRequestCreateDto? createDto = null, PessoaFisicaRequestUpdateDto? updateDto = null);
    }
}