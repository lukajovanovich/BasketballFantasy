using System.Text.Json.Serialization;
using BasketballFantasy.API.Services;
using BasketballFantasy.Domain.Repositories;
using BasketballFantasy.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddOpenApi();
builder.Services.AddDbContext<BasketballFantasyContext>(options =>
    options
        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .EnableSensitiveDataLogging());

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<FantasyPointsService>();
builder.Services.AddScoped<ExcelExportService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthorization();
app.MapControllers();

app.Run();
