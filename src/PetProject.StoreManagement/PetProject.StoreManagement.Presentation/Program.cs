using PetProject.StoreManagement.CrossCuttingConcerns.Extensions;
using PetProject.StoreManagement.Infrastructure.Extensions;
using PetProject.StoreManagement.Persistence.Extensions;
using PetProject.StoreManagement.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCrossCuttingConcerns();
builder.Services.AddPersistence(configuration.GetConnectionString("StoreManagement"), "");
builder.Services.AddInfrastructure(configuration["Caching:RedisURL"]);
builder.Services.AddApplication();

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
