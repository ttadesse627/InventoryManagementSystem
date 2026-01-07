using Microsoft.OpenApi.Models;
using TemporalWarehouse.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "The Temporal Warehouse API" });

    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
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

builder.Services.AddServicesRegistrations(builder.Configuration);

var app = builder.Build();

app.MapGet("/health", () => Results.Ok("The server is running!"));

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("WarehouseCorsPolicy");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
