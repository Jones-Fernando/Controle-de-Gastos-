using backend.Dtos;

namespace backend.Services
{
    public interface ITransacaoService
    {
        Task<IReadOnlyList<TransacaoResponseDto>> ListarAsync();
        Task<ServiceResult<TransacaoResponseDto>> CriarAsync(CriarTransacaoDto dto);
        Task<ServiceResult<bool>> DeletarAsync(int id);
    }
}
