using backend.Data;
using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class PessoaService : IPessoaService
    {
        private readonly AppDbContext _context;

        public PessoaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<PessoaResponseDto>> ListarAsync()
        {
            var pessoas = await _context.Pessoas
                .OrderBy(p => p.Id)
                .ToListAsync();

            return pessoas.Select(MapearPessoa).ToList();
        }

        /// <summary>
        /// Calcula o total de receitas, despesas e saldo para cada pessoa,
        /// bem como o total geral de todas as pessoas cadastradas.
        /// </summary>
        public async Task<TotaisResponseDto> ListarTotaisAsync()
        {
            var pessoas = await _context.Pessoas
                .Include(p => p.Transacoes)
                .OrderBy(p => p.Id)
                .ToListAsync();

            var resumoPorPessoa = pessoas.Select(MapearResumo).ToList();
            var totalGeral = new TotalGeralDto(
                resumoPorPessoa.Sum(item => item.TotalReceitas),
                resumoPorPessoa.Sum(item => item.TotalDespesas),
                resumoPorPessoa.Sum(item => item.Saldo));

            return new TotaisResponseDto(resumoPorPessoa, totalGeral);
        }

        public async Task<ServiceResult<PessoaResponseDto>> CriarAsync(CriarPessoaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                return ServiceResult<PessoaResponseDto>.Fail("O nome é obrigatório.");
            }

            if (dto.Idade < 0)
            {
                return ServiceResult<PessoaResponseDto>.Fail("A idade não pode ser negativa.");
            }

            var pessoa = new Pessoa
            {
                Nome = dto.Nome.Trim(),
                Idade = dto.Idade
            };

            _context.Pessoas.Add(pessoa);
            await _context.SaveChangesAsync();

            return ServiceResult<PessoaResponseDto>.Ok(MapearPessoa(pessoa));
        }

        public async Task<ServiceResult<bool>> DeletarAsync(int id)
        {
            var pessoa = await _context.Pessoas
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pessoa == null)
            {
                return ServiceResult<bool>.Fail("Pessoa não encontrada.");
            }

            _context.Pessoas.Remove(pessoa);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        private static PessoaResponseDto MapearPessoa(Pessoa pessoa) => new(pessoa.Id, pessoa.Nome, pessoa.Idade);

        private static PessoaResumoDto MapearResumo(Pessoa pessoa)
        {
            var totalReceitas = pessoa.Transacoes
                .Where(t => string.Equals(t.Tipo, TipoTransacao.Receita, StringComparison.OrdinalIgnoreCase))
                .Sum(t => t.Valor);

            var totalDespesas = pessoa.Transacoes
                .Where(t => string.Equals(t.Tipo, TipoTransacao.Despesa, StringComparison.OrdinalIgnoreCase))
                .Sum(t => t.Valor);

            return new PessoaResumoDto(
                pessoa.Id,
                pessoa.Nome,
                pessoa.Idade,
                totalReceitas,
                totalDespesas,
                totalReceitas - totalDespesas);
        }
    }
}
