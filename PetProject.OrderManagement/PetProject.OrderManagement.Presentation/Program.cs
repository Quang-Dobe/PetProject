using Microsoft.Extensions.Options;
using PetProject.OrderManagement.CrossCuttingConcerns.SharedAppSetting;
using PetProject.OrderManagement.CrossCuttingConcerns.Extensions;
using PetProject.OrderManagement.Infrastructure.Extensions;
using PetProject.OrderManagement.Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AppSettings>(configuration)
    .AddSingleton(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value);
builder.Services.AddCrossCuttingConcerns();
builder.Services.AddPersistence(configuration.GetConnectionString("OrderManagement"), "");
builder.Services.AddInfrastructure();

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
