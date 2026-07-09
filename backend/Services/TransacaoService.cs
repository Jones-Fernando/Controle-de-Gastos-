using backend.Data;
using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class TransacaoService : ITransacaoService
    {
        private readonly AppDbContext _context;

        public TransacaoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<TransacaoResponseDto>> ListarAsync()
        {
            var transacoes = await _context.Transacoes
                .Include(t => t.Pessoa)
                .OrderBy(t => t.Id)
                .ToListAsync();

            return transacoes.Select(MapearTransacao).ToList();
        }

        /// <summary>
        /// Cria uma nova transação vinculada a uma pessoa existente.
        /// Valida regra de negócio: menores de idade só podem cadastrar despesas.
        /// </summary>
        public async Task<ServiceResult<TransacaoResponseDto>> CriarAsync(CriarTransacaoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Descricao))
            {
                return ServiceResult<TransacaoResponseDto>.Fail("A descrição é obrigatória.");
            }

            if (dto.Valor <= 0)
            {
                return ServiceResult<TransacaoResponseDto>.Fail("O valor deve ser maior que zero.");
            }

            var tipo = dto.Tipo?.Trim() ?? string.Empty;
            if (!TipoTransacao.IsValido(tipo))
            {
                return ServiceResult<TransacaoResponseDto>.Fail("O tipo deve ser 'Despesa' ou 'Receita'.");
            }

            if (dto.PessoaId <= 0)
            {
                return ServiceResult<TransacaoResponseDto>.Fail("Pessoa inválida.");
            }

            var pessoa = await _context.Pessoas.FindAsync(dto.PessoaId);
            if (pessoa == null)
            {
                return ServiceResult<TransacaoResponseDto>.Fail("Pessoa não encontrada.");
            }

            if (pessoa.Idade < 18 && string.Equals(tipo, TipoTransacao.Receita, StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult<TransacaoResponseDto>.Fail("Menores de 18 anos não podem cadastrar receitas.");
            }

            var transacao = new Transacao
            {
                Descricao = dto.Descricao.Trim(),
                Valor = dto.Valor,
                Tipo = tipo,
                PessoaId = dto.PessoaId
            };

            _context.Transacoes.Add(transacao);
            await _context.SaveChangesAsync();

            return ServiceResult<TransacaoResponseDto>.Ok(MapearTransacao(transacao));
        }

        public async Task<ServiceResult<bool>> DeletarAsync(int id)
        {
            var transacao = await _context.Transacoes.FindAsync(id);
            if (transacao == null)
            {
                return ServiceResult<bool>.Fail("Transação não encontrada.");
            }

            _context.Transacoes.Remove(transacao);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        private static TransacaoResponseDto MapearTransacao(Transacao transacao) => new(
            transacao.Id,
            transacao.Descricao,
            transacao.Valor,
            transacao.Tipo,
            transacao.PessoaId,
            transacao.Pessoa?.Nome);
    }
}
