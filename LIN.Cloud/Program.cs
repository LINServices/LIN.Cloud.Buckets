global using Http.ResponsesList;
global using LIN.Cloud.Services;
global using LIN.Types.Cloud.Models;
global using LIN.Types.Responses;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.StaticFiles;
global using LIN.Cloud.Repository.Abstractions;
using Http.Extensions;
using LIN.Cloud.Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor.
builder.Services.AddLINHttp(true, (options) =>
{
    options.OperationFilter<CustomOperationFilter<ServiceFilterAttribute>>("key");
});

builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();
builder.Services.AddPersistence(builder.Configuration);

// Servicios.
builder.Services.AddScoped<IFileRepository, LIN.Cloud.Repository.FileRepository>();
builder.Services.AddScoped<BucketService, BucketService>();
builder.Services.AddScoped<IdentityTokenAttribute, IdentityTokenAttribute>();

// Build.
var app = builder.Build();

// Usar servicios.
app.UseLINHttp();
app.UseDataBase();
app.UseAuthorization();
app.MapControllers();

BucketService.Default = "C:/Data/Cloud";

app.Run();