using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

// Registro do FluentValidation para validação automática dos DTOs.
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registro dos serviços de domínio. Eles encapsulam regras de negócio e
// validam dados antes de persistir no banco SQLite em arquivo, garantindo
// persistência entre reinicializações da aplicação.
builder.Services.AddScoped<IPessoaService, PessoaService>();
builder.Services.AddScoped<ITransacaoService, TransacaoService>();

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    await next();
});

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var ex = feature?.Error;

        // Se for uma RegraDeNegocioException, retornamos 422 com a mensagem da exceção.
        if (ex is backend.Exceptions.RegraDeNegocioException rex)
        {
            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { erro = rex.Message });
            return;
        }

        // Para demais erros, mantemos o tratamento genérico 500.
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { erro = "Ocorreu um erro inesperado no servidor." });
    });
});

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.MapHealthChecks("/health");
app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));
app.MapControllers();

app.Run();

// Necessário para que os testes de integração consigam referenciar o tipo Program
// ao usar WebApplicationFactory<Program>. Mantemos a declaração parcial vazia.
public partial class Program { }