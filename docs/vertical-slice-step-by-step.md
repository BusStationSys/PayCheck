# Arquitetura Vertical Slice (Slicer) - Guia passo a passo

Objetivo: mostrar como organizar uma solution seguindo a arquitetura vertical slice, orquestrando projetos e demonstrando um fluxo mínimo (API -> Feature -> Handler -> Data).

Pré-requisitos
- .NET 7 SDK
- dotnet CLI
- Editor (VS / VS Code)

Resumo da abordagem
- Cada *feature* é um projeto ou pasta contendo tudo que ela precisa: Requests/Handlers, DTOs, Validators, Controllers (ou Endpoints) e persistência específica.
- Dependências compartilhadas (tipos comuns, contratos) ficam em projetos "Shared" ou "Common".
- A API (ou Web) referencia os projetos de feature e registra handlers/serviços.

Estrutura sugerida

- PayCheck.VerticalSlices.sln
- src/
  - PayCheck.Api/           -> Projeto ASP.NET Core Web API (entrypoint)
  - Shared/                 -> Tipos compartilhados (ex.: DTOs, abstrações)
  - Features/
    - Payroll/              -> Feature vertical (ex.: composição salarial)
      - Payroll.Api.csproj? -> ou manter como namespace dentro do projeto de feature
      - Requests/
      - Handlers/
      - Controllers/Endpoints/
      - Data/ (repositório específico ou adaptador)

Passo a passo (comandos dotnet)

1) Criar solution

```powershell
dotnet new sln -n PayCheck.VerticalSlices
```

2) Criar projeto API (se já não existir, caso contrário pule)

```powershell
dotnet new webapi -o src/PayCheck.Api
dotnet sln add src/PayCheck.Api/PayCheck.Api.csproj
```

3) Criar projeto Shared

```powershell
dotnet new classlib -o src/Shared/PayCheck.Shared
dotnet sln add src/Shared/PayCheck.Shared.csproj
```

4) Criar projeto de feature (ex.: Payroll)

```powershell
dotnet new classlib -o src/Features/Payroll/PayCheck.Features.Payroll
dotnet sln add src/Features/Payroll/PayCheck.Features.Payroll.csproj
```

5) Adicionar referências

- API referencia Shared e a(s) Feature(s):

```powershell
dotnet add src/PayCheck.Api/PayCheck.Api.csproj reference src/Shared/PayCheck.Shared.csproj
dotnet add src/PayCheck.Api/PayCheck.Api.csproj reference src/Features/Payroll/PayCheck.Features.Payroll.csproj
```

6) Adicionar pacotes úteis

```powershell
# MediatR para handlers (vertical slice pattern)
dotnet add src/PayCheck.Api/PayCheck.Api.csproj package MediatR.Extensions.Microsoft.DependencyInjection
# FluentValidation (opcional)
dotnet add src/PayCheck.Api/PayCheck.Api.csproj package FluentValidation.AspNetCore
```

7) Scaffold de arquivos da feature (exemplo mínimo)

- Criar `Requests/GetSalaryCompositionQuery.cs` no projeto `PayCheck.Features.Payroll`:

```csharp
using MediatR;
using System.Collections.Generic;

public record GetSalaryCompositionQuery(long EmployeeId) : IRequest<IEnumerable<SalaryCompositionDto>>;

public record SalaryCompositionDto(string DescricaoEvento, decimal Valor, string Cor);
```

- Criar `Handlers/GetSalaryCompositionHandler.cs` no mesmo projeto:

```csharp
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

public class GetSalaryCompositionHandler : IRequestHandler<GetSalaryCompositionQuery, IEnumerable<SalaryCompositionDto>>
{
    // Injetar repositório/adaptador (aqui exemplo inline)
    public Task<IEnumerable<SalaryCompositionDto>> Handle(GetSalaryCompositionQuery request, CancellationToken cancellationToken)
    {
        // Exemplo fixo — substituir por chamada a banco ou serviço
        var result = new List<SalaryCompositionDto>
        {
            new SalaryCompositionDto("Salário Base", 5000m, "#4e73df"),
            new SalaryCompositionDto("Bonificação", 800m, "#1cc88a"),
            new SalaryCompositionDto("Desconto INSS", -300m, "#e74a3b")
        };
        return Task.FromResult<IEnumerable<SalaryCompositionDto>>(result);
    }
}
```

- Expor endpoint na API (ex.: Controller ou Minimal API)

No `src/PayCheck.Api/Program.cs` registre MediatR e adicione endpoint:

```csharp
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetSalaryCompositionHandler).Assembly));

app.MapGet("/api/payroll/{employeeId:int}", async (int employeeId, IMediator mediator) =>
{
    var result = await mediator.Send(new GetSalaryCompositionQuery(employeeId));
    return Results.Ok(result);
});
```

8) Executar e testar

```powershell
dotnet build
dotnet run --project src/PayCheck.Api/PayCheck.Api.csproj
# Teste:
# GET http://localhost:5000/api/payroll/123
```

Orquestrando e demonstrando (fluxo)

- API recebe a requisição -> MediatR resolve handler com base no Request.
- Handler consulta repositórios/adapters (injetados via DI) e retorna DTOs.
- API retorna DTOs para o cliente.

Dicas práticas
- Mantenha cada feature coesa e com responsabilidades claras.
- Use `Shared` para contratos e tipos que realmente são compartilhados.
- Para UI Razor Pages, chame a API (ou injete mediatR se for o mesmo processo/assembly).
- Versione endpoints por pasta/namespace quando necessário (ex.: /api/v1/payroll).

Exemplo de evolução
- Substituir o handler de exemplo por um repositório que usa Dapper/EF Core.
- Adicionar testes unitários por feature (projeto de teste por feature).
- Adicionar validações com `FluentValidation` e comportamento pipeline do MediatR.

Referências rápidas
- MediatR: https://github.com/jbogard/MediatR
- Vertical slice pattern: exemplos no repositório .NET e articles


Se desejar, posso:
- Gerar os projetos reais no workspace com os arquivos mostrados (Requests/Handlers/Program.cs) usando dotnet CLI (PowerShell) e criar a solution.
- Ou aplicar essas mudanças a projetos já existentes na solução atual (integração com `src/PayCheck.Api`).

Informe se quer que eu crie os projetos e arquivos automaticamente no repositório (especifique se prefere controller ou minimal API e se deseja MediatR instalado).