# Controle de Gastos Residenciais

Aplicação full-stack para registrar pessoas, receitas e despesas, com persistência em SQLite e interface em React + TypeScript.

## Tecnologias

- Backend: ASP.NET Core 8, Entity Framework Core, SQLite
- Frontend: React, TypeScript, Vite, Axios
- Testes: xUnit

## Requisitos

- .NET 8 SDK
- Node.js 20+
- npm

## Backend

1. Entre na pasta do backend:
   ```bash
   cd backend
   ```
2. Restaure os pacotes:
   ```bash
   dotnet restore
   ```
3. Aplique as migrations e rode a API:
   ```bash
   dotnet ef database update
   dotnet run
   ```
4. A API ficará disponível em:
   - http://localhost:5281
   - Swagger em http://localhost:5281/swagger
   - Health check em http://localhost:5281/api/health

## Frontend

1. Entre na pasta do frontend:
   ```bash
   cd frontend
   ```
2. Instale as dependências:
   ```bash
   npm install
   ```
3. Inicie o ambiente de desenvolvimento:
   ```bash
   npm run dev
   ```
4. A interface estará disponível em http://localhost:5173

## Variáveis de ambiente

O frontend lê a URL da API a partir de `VITE_API_URL`. Exemplo:

```bash
VITE_API_URL=http://localhost:5281/api
```

## Funcionalidades

- Cadastro de pessoas
- Cadastro de receitas e despesas
- Validação de menoridade para receitas
- Resumo dos totais por pessoa e geral
- Endpoint de health check para verificação rápida da API

## Testes

Execute os testes do backend com:

```bash
dotnet test backend.Tests/backend.Tests.csproj
```
