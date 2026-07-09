using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace backend.Tests;

public class PessoaServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly PessoaService _service;

    public PessoaServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
        _service = new PessoaService(_context);
    }

    [Fact]
    public async Task DeletarAsync_DevePropagarComTransacoes()
    {
        var pessoa = new Pessoa { Nome = "Ana", Idade = 25 };
        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync();
        _context.Transacoes.Add(new Transacao { Descricao = "Mercado", Valor = 50, Tipo = "Despesa", PessoaId = pessoa.Id });
        await _context.SaveChangesAsync();

        await _service.DeletarAsync(pessoa.Id);

        Assert.Equal(0, await _context.Transacoes.CountAsync()); // prova o cascade
    }

    [Fact]
    public async Task CriarAsync_PessoaValida_Persiste()
    {
        var resultado = await _service.CriarAsync(new backend.Dtos.CriarPessoaDto("Carlos", 40));
        Assert.True(resultado.Success);
        Assert.NotNull(resultado.Value);
        Assert.Equal(1, await _context.Pessoas.CountAsync());
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
