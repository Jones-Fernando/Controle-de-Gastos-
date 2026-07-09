namespace backend.Dtos
{
    public record CriarPessoaDto(string Nome, int Idade);

    public record PessoaResponseDto(int Id, string Nome, int Idade);

    public record PessoaResumoDto(int Id, string Nome, int Idade, decimal TotalReceitas, decimal TotalDespesas, decimal Saldo);

    public record TotalGeralDto(decimal TotalReceitas, decimal TotalDespesas, decimal Saldo);

    public record TotaisResponseDto(IReadOnlyList<PessoaResumoDto> Pessoas, TotalGeralDto TotalGeral);
}
