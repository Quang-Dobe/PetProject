using Microsoft.Extensions.Options;
using PetProject.IdentityServer.CrossCuttingConcerns.SharedAppSetting;
using PetProject.IdentityServer.CrossCuttingConcerns.Extensions;
using PetProject.IdentityServer.Infrastructure.Extensions;
using PetProject.IdentityServer.Persistence.Extensions;
using PetProject.IdentityServer.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCrossCuttingConcerns();
builder.Services.AddPersistence(configuration.GetConnectionString("Identity"), "");
builder.Services.AddInfrastructure();
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
