using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TemporalWarehouse.Api.Application.Interfaces;
using TemporalWarehouse.Api.Application.Services;
using TemporalWarehouse.Api.Infrastructure.Contexts;
using TemporalWarehouse.Api.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

string? connectionString = builder.Configuration.GetConnectionString("NpgsqlRemoteConnection");
builder.Services.AddDbContext<WarehouseDbContext>(options =>
{
    options.UseNpgsql(connectionString, opt =>
    {
        opt.CommandTimeout(60);
        opt.EnableRetryOnFailure();
    });

}, ServiceLifetime.Scoped);

#region Repositories and Services Registration
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<IStockRepository, StockRepository>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IStockService, StockService>();
builder.Services.AddTransient<IHistoryService, HistoryService>();
#endregion

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "The Temporal Warehouse API" });
});
builder.Services.AddCors(options =>
    {
        options.AddPolicy("WarehouseCorsPolicy", builder =>
        {
            builder.WithOrigins("http://localhost:3000", "https://temporal-warehouse.vercel.app")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
    });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("WarehouseCorsPolicy");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();

