# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a **credit simulation API** developed for Hackathon CAIXA 2024. The API provides loan simulation with SAC and PRICE amortization systems, built with .NET 8 using Clean Architecture principles.

**Key Features:**
- Credit simulation with SAC and PRICE amortization systems
- Product filtering based on business rules
- Real-time telemetry and metrics collection
- Azure Event Hub integration for event-driven architecture
- SQLite local persistence with SQL Server external connectivity

## Development Commands

### Local Development
```bash
# Setup environment (run once)
./scripts/setup.sh                 # Linux/macOS
./scripts/setup.ps1                # Windows PowerShell
./scripts/setup.bat                # Windows CMD

# Run API locally
cd Hackathon.API
dotnet run                          # http://localhost:5000

# Access documentation
open http://localhost:5000/swagger
```

### Docker Development
```bash
# Development mode (port 5000)
./scripts/docker.sh dev
docker-compose --profile dev up -d

# Production mode (port 8080)  
./scripts/docker.sh prod
docker-compose --profile prod up -d
```

### Testing
```bash
# Run all tests
dotnet test

# Run tests with coverage
./scripts/test-coverage.sh coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test projects
dotnet test Hackathon.API.Tests
dotnet test Hackathon.Application.Tests
dotnet test Hackathon.Domain.Tests
dotnet test Hackathon.Infrastructure.Tests

# Advanced coverage with dotCover (if available)
./scripts/test-coverage.sh dotcover
```

### Database Operations
```bash
# Apply migrations (automatic on startup)
dotnet ef database update --project Hackathon.Infrastructure --startup-project Hackathon.API

# Create new migration
dotnet ef migrations add <MigrationName> --project Hackathon.Infrastructure --startup-project Hackathon.API
```

## Architecture

### Clean Architecture Structure
```
Hackathon/
├── Hackathon.API/           # Presentation Layer (Controllers, Middleware)
├── Hackathon.Application/   # Application Layer (Use Cases, Services, DTOs)
├── Hackathon.Domain/        # Domain Layer (Entities, Value Objects, Business Rules)
├── Hackathon.Infrastructure/# Infrastructure Layer (Data Access, External Services)
└── Hackathon.*.Tests/       # Unit and Integration Tests
```

**Dependency Flow:** API → Application → Domain ← Infrastructure

### Key Architectural Patterns
- **Clean Architecture**: Clear separation of concerns across layers
- **Domain-Driven Design (DDD)**: Rich domain models with value objects
- **CQRS**: Command-Query segregation for operations
- **Repository Pattern**: Data access abstraction
- **Dependency Injection**: Isolated configuration per layer

### Core Domain Concepts

**Entities:**
- `Simulacao` - Credit simulation aggregate root
- `Produto` - Financial product with business rules
- `ResultadoSimulacao` - Amortization calculation results
- `MetricaRequisicao` - Telemetry data collection

**Value Objects:**
- `ValorEmprestimo` - Loan amount with validation
- `TaxaJuros` - Interest rate calculations
- `PrazoMeses` - Term constraints
- `ValorMonetario` - Money representation

**Services:**
- `CalculadoraPRICE` / `CalculadoraSAC` - Amortization calculations
- `CachedProdutoService` - Product filtering with caching
- `TelemetriaService` - Performance metrics collection
- `EventHubService` - Azure Event Hub integration

## API Endpoints

### Core Simulation
- `POST /simulacao` - Perform credit simulation
- `GET /simulacao` - List simulations (paginated)  
- `GET /simulacao/volume` - Volume aggregated by product and date

### Telemetry & Monitoring
- `GET /telemetria/por-dia` - Daily performance metrics
- `GET /health` - Health check endpoint

## Configuration Management

### Environment Variables
```bash
# Database connections
ConnectionStrings__LocalDb=Data Source=/app/data/hack.db
ConnectionStrings__ProdutosDb=Server=...;Database=hack;...

# Event Hub
ConnectionStrings__EventHub=Endpoint=sb://...

# Environment
ASPNETCORE_ENVIRONMENT=Development|Production
DOTNET_RUNNING_IN_CONTAINER=true
```

### Key Configuration Files
- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development overrides
- `global.json` - .NET SDK version pinning
- `docker-compose.yml` - Container orchestration

## Testing Strategy

The project maintains **>90% test coverage** across all layers:

### Test Organization
- **Unit Tests**: Domain logic, value objects, business rules
- **Integration Tests**: Repository operations, service interactions  
- **API Tests**: Controller behavior, middleware, validation
- **Behavior Tests**: End-to-end scenarios

### Test Execution Patterns
- Use `FluentAssertions` for readable assertions
- Mock external dependencies with clear interfaces
- Test business rules comprehensively in Domain layer
- Validate integration points in Infrastructure tests

## Performance Considerations

### Optimization Strategies
- **Async/Await**: All I/O operations are asynchronous
- **Caching**: Products cached with intelligent invalidation
- **Background Processing**: Event Hub publishing via ThreadPool
- **Connection Pooling**: EF Core handles database connections efficiently

### Telemetry Integration
The `TelemetriaMiddleware` automatically captures:
- Request/response times
- Success/failure rates  
- Volume metrics per endpoint
- Performance trends over time

## Docker & Deployment

### Multi-stage Dockerfile
- **Build stage**: Compilation and testing
- **Runtime stage**: Minimal production image
- **Security**: Non-root user execution
- **Health checks**: Integrated monitoring

### Database Strategy
- **Development**: SQLite for local development
- **Production**: SQL Server with automatic migrations
- **Hybrid**: Both databases supported simultaneously

## Development Guidelines

### Code Quality Standards
- Follow Clean Architecture dependency rules
- Use Value Objects for domain concepts
- Implement comprehensive validation at boundaries
- Maintain high test coverage (>90%)
- Apply defensive programming practices

### Business Logic Placement
- **Domain**: Core business rules and calculations
- **Application**: Use cases and orchestration  
- **Infrastructure**: External system integration
- **API**: Input validation and response formatting

### Error Handling Strategy  
- Custom domain exceptions with specific error codes
- Global exception middleware for consistent responses
- Structured logging with correlation IDs
- Graceful degradation for non-critical failures

## External Integrations

### Azure Event Hub
- **Purpose**: Real-time event publishing for analytics
- **Pattern**: Fire-and-forget background processing
- **Resilience**: Failures logged but don't block operations

### Database Connectivity
- **SQL Server**: External product catalog (read-only)
- **SQLite**: Local simulation persistence
- **EF Core**: Code-first migrations and change tracking

## Common Development Tasks

### Adding New Features
1. Start with Domain entities and value objects
2. Create Application services and commands/queries  
3. Add Infrastructure implementations
4. Build API controllers and contracts
5. Write comprehensive tests for all layers

### Debugging Performance
1. Check telemetry metrics via `/telemetria/por-dia`
2. Review application logs for timing information
3. Use health checks to validate system status
4. Monitor Event Hub integration success rates