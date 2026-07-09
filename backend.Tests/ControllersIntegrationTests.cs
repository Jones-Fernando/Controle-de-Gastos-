using System.Net;
using System.Net.Http.Json;
using backend.Data;
using backend.Dtos;
using backend.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace backend.Tests;

public class ControllersIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly SqliteConnection _connection;

    public ControllersIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    private HttpClient CreateClientWithInMemoryDb()
    {
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove AppDbContext registrado originalmente
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                // Registra AppDbContext usando SQLite in-memory compartilhado
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });

                // Ensure database is created
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            });
        }).CreateClient();

        return client;
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsOk()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostPessoa_ThenPostTransacao_MenorDeIdade_RejeitaReceita()
    {
        using var client = CreateClientWithInMemoryDb();

        // cria pessoa menor de idade
        var pessoaResp = await client.PostAsJsonAsync("/api/Pessoas", new CriarPessoaDto("Joaozinho", 17));
        pessoaResp.EnsureSuccessStatusCode();
        var pessoa = await pessoaResp.Content.ReadFromJsonAsync<backend.Dtos.PessoaResponseDto>();

        // tenta criar receita para menor -> deve falhar
        var transacao = new CriarTransacaoDto("Presente", 100m, TipoTransacao.Receita, pessoa.Id);
        var transResp = await client.PostAsJsonAsync("/api/Transacoes", transacao);

        Assert.Equal(HttpStatusCode.BadRequest, transResp.StatusCode);
    }

    [Fact]
    public async Task DeletarPessoa_PropagaExclusaoDasTransacoes()
    {
        using var client = CreateClientWithInMemoryDb();

        // cria pessoa
        var pessoaResp = await client.PostAsJsonAsync("/api/Pessoas", new CriarPessoaDto("Ana", 25));
        pessoaResp.EnsureSuccessStatusCode();
        var pessoa = await pessoaResp.Content.ReadFromJsonAsync<backend.Dtos.PessoaResponseDto>();

        // cria transacao
        var transacao = new CriarTransacaoDto("Mercado", 50m, TipoTransacao.Despesa, pessoa.Id);
        var transResp = await client.PostAsJsonAsync("/api/Transacoes", transacao);
        transResp.EnsureSuccessStatusCode();

        // deleta pessoa
        var delResp = await client.DeleteAsync($"/api/Pessoas/{pessoa.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delResp.StatusCode);

        // lista transacoes e verifica que não há nenhuma
        var listResp = await client.GetAsync("/api/Transacoes");
        listResp.EnsureSuccessStatusCode();
        var transacoes = await listResp.Content.ReadFromJsonAsync<List<backend.Dtos.TransacaoResponseDto>>();
        Assert.Empty(transacoes);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
