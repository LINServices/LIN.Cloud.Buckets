global using Http.ResponsesList;
global using LIN.Cloud.Buckets.Repository.Abstractions;
global using LIN.Cloud.Buckets.Services;
global using LIN.Types.Cloud.Models;
global using LIN.Types.Responses;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.StaticFiles;
using Http.Extensions;
using LIN.Access.Developer;
using LIN.Cloud.Buckets.Persistence.Extensions;
using LIN.Cloud.Buckets.Repository;
using LIN.Cloud.Identity.Utilities;
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
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<BucketService, BucketService>();
builder.Services.AddScoped<IdentityKeyAttribute, IdentityKeyAttribute>();

// Llave de LIN Cloud Developers.
builder.Services.AddLinCloudOrchestrator(builder.Configuration);

// Build.
var app = builder.Build();

// Usar servicios.
app.UseLINHttp();
app.UseDataBase();
app.UseAuthorization();
app.MapControllers();

BucketService.Default = "C:/Data/Cloud";

app.Run();