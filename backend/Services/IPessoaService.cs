using backend.Dtos;

namespace backend.Services
{
    public interface IPessoaService
    {
        Task<IReadOnlyList<PessoaResponseDto>> ListarAsync();
        Task<TotaisResponseDto> ListarTotaisAsync();
        Task<ServiceResult<PessoaResponseDto>> CriarAsync(CriarPessoaDto dto);
        Task<ServiceResult<bool>> DeletarAsync(int id);
    }
}
