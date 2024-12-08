global using Http.ResponsesList;
global using LIN.Cloud.Services;
global using LIN.Types.Cloud.Models;
global using LIN.Types.Responses;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.StaticFiles;
global using LIN.Cloud.Repository.Abstractions;
using LIN.Access.Developer;
using Http.Extensions;
using LIN.Cloud.Persistence.Extensions;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor.
builder.Services.AddLINHttp(true, (options) =>
{
    options.OperationFilter<Http.Extensions.OpenApi.HeaderMapAttribute<ServiceFilterAttribute>>("key");
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue;
});

builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddDeveloperService();

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

// Llave de LIN Cloud Developers.
LIN.Cloud.Identity.Utilities.Build.Init(builder.Configuration["identity:key"] ?? string.Empty);

app.Run();