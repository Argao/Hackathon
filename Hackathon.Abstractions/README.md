# Hackathon.Abstractions

Este projeto contém as abstrações compartilhadas do sistema Hackathon, seguindo os princípios da Clean Architecture.

## 📁 Estrutura do Projeto

### Interfaces
- **IRepository<TEntity, TId>** - Interface base genérica para repositórios
- **IPagedRepository<TEntity>** - Interface para repositórios com paginação
- **IService<TEntity, TId>** - Interface base genérica para serviços
- **ICacheService** - Interface para serviços de cache
- **IEventPublisher** - Interface para publicação de eventos

### DTOs
#### Requests
- **BaseRequest** - Classe base para requests com validações
- **PagedRequest** - Request base para operações paginadas

#### Responses
- **BaseResponse** - Classe base para responses
- **PagedResponse<T>** - Response base para operações paginadas
- **ErrorResponse** - Response para erros

### Exceptions
- **DomainException** - Exceção base abstrata
- **ValidationException** - Para erros de validação
- **NotFoundException** - Para recursos não encontrados
- **BusinessRuleException** - Para violações de regras de negócio
- **InfrastructureException** - Para erros de infraestrutura
- **ProdutoNotFoundException** - Para produtos não encontrados
- **SimulacaoException** - Para erros de simulação

### Results
- **Result<T>** - Classe para resultados de operações que podem falhar

### Constants
- **ErrorCodes** - Códigos de erro padronizados
- **HttpStatusCodes** - Códigos de status HTTP comuns

## 🚀 Como Usar

### Interfaces Base
```csharp
// Implementando um repositório
public class ProdutoRepository : IRepository<Produto, int>
{
    public async Task<Produto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // Implementação
    }
    
    // Outros métodos...
}

// Implementando um serviço
public class ProdutoService : IService<Produto, int>
{
    public async Task<Produto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // Implementação
    }
    
    // Outros métodos...
}
```

### DTOs Base
```csharp
// Criando um request paginado
public class ListarProdutosRequest : PagedRequest
{
    public string? Nome { get; set; }
    public decimal? PrecoMinimo { get; set; }
}

// Criando um response paginado
var response = new PagedResponse<Produto>(produtos, pageNumber, pageSize, totalRecords);
```

### Exceptions
```csharp
// Lançando exceções específicas
throw new NotFoundException("Produto", produtoId);
throw new BusinessRuleException("Valor deve ser maior que zero", "VALOR_INVALIDO");
throw new ValidationException("Campo obrigatório não informado");
```

### Results
```csharp
// Usando Result para operações que podem falhar
public async Task<Result<Produto>> ObterProdutoAsync(int id)
{
    try
    {
        var produto = await _repository.GetByIdAsync(id);
        return produto != null 
            ? Result<Produto>.Success(produto)
            : Result<Produto>.Failure("Produto não encontrado", ErrorCodes.NOT_FOUND);
    }
    catch (Exception ex)
    {
        return Result<Produto>.Failure("Erro interno", ErrorCodes.INTERNAL_ERROR);
    }
}

// Usando o resultado
var result = await ObterProdutoAsync(1);
result.OnSuccess(produto => Console.WriteLine($"Produto: {produto.Nome}"));
result.OnFailure(error => Console.WriteLine($"Erro: {error}"));
```

## 📋 Dependências

- **FluentValidation** - Para validações
- **System.ComponentModel.DataAnnotations** - Para Data Annotations

## 🔧 Configuração

Este projeto é referenciado pelos outros projetos da solução:
- **Hackathon.API** - Para usar as abstrações na camada de apresentação
- **Hackathon.Application** - Para usar as abstrações na camada de aplicação

## 🎯 Benefícios

1. **Reutilização** - Interfaces e DTOs compartilhados entre camadas
2. **Consistência** - Padrões uniformes para exceções e resultados
3. **Desacoplamento** - Abstrações que permitem trocar implementações
4. **Manutenibilidade** - Código centralizado e bem documentado
5. **Testabilidade** - Interfaces facilitam a criação de mocks
