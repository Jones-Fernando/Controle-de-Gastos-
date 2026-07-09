namespace backend.Dtos
{
    public record CriarTransacaoDto(string Descricao, decimal Valor, string Tipo, int PessoaId);

    public record TransacaoResponseDto(int Id, string Descricao, decimal Valor, string Tipo, int PessoaId, string? PessoaNome);
}
