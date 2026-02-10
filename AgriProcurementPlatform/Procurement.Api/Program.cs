using Microsoft.EntityFrameworkCore;
using Procurement.Application.Services;
using Procurement.Infrastructure.Persistence;
using Procurement.Application.Abstractions;
using Procurement.Api.Middleware;
using Procurement.Api.Services;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProcurementDbContext>(sp => sp.GetRequiredService<ProcurementDbContext>());
builder.Services.AddScoped<ExceptionMiddleware>();
builder.Services.AddScoped<ProcurementService>();
builder.Services.AddScoped<QueryService>();
builder.Services.AddHostedService<Procurement.Api.Background.OutboxProcessor>();


builder.Services.AddHealthChecks();

builder.Services.AddDbContext<ProcurementDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("db")));





var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks("/health");
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();

app.Run();
