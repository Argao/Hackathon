using System.Data;
using Hackathon.API.Persistence.Produtos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<Func<IDbConnection>>(_ =>
{
    var cs = builder.Configuration.GetConnectionString("ProdutosDb")!;
    return () =>
    {
        var cn = new SqlConnection(cs);
        cn.Open(); 
        return cn;
    };
});

builder.Services.AddDbContext<ProdutosDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("ProdutosDb"),
            sql => sql.EnableRetryOnFailure())   // resiliente p/ rede
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

// Repositório remoto
//builder.Services.AddScoped<IProdutoReadRepository, EfProdutoReadRepository>();
builder.Services.AddScoped<IProdutoReadRepository, SqlServerProdutoReadRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();