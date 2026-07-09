using backend.Data;
using backend.Dtos;
using backend.Models;
using backend.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace backend.Tests;

public class TransacaoServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly TransacaoService _service;

    public TransacaoServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
        _service = new TransacaoService(_context);
    }

    [Fact]
    public async Task CriarAsync_RejeitaParaMenorDeIdade_DeveRetornarErro()
    {
        _context.Pessoas.Add(new Pessoa { Nome = "João", Idade = 17 });
        await _context.SaveChangesAsync();

        var resultado = await _service.CriarAsync(new CriarTransacaoDto("Salário", 100, "Receita", 1));

        Assert.False(resultado.Success);
        Assert.Contains("Menores", resultado.Error);
    }

    [Fact]
    public async Task CriarAsync_TransacaoValida_DevePersistir()
    {
        _context.Pessoas.Add(new Pessoa { Nome = "Maria", Idade = 30 });
        await _context.SaveChangesAsync();

        var resultado = await _service.CriarAsync(new CriarTransacaoDto("Mercado", 50, "Despesa", 1));

        Assert.True(resultado.Success);
        Assert.NotNull(resultado.Value);
        Assert.Equal(1, await _context.Transacoes.CountAsync());
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
