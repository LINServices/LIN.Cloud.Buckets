global using Http.ResponsesList;
global using LIN.Cloud.Services;
global using LIN.Types.Cloud.Models;
global using LIN.Types.Responses;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.StaticFiles;
global using LIN.Cloud.Repository.Abstractions;
using Http.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor.
builder.Services.AddLINHttp();
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();

// Servicios.
builder.Services.AddScoped<IFileRepository, LIN.Cloud.Repository.FileRepository>();
builder.Services.AddScoped<BucketService, BucketService>();

// Build.
var app = builder.Build();

// Usar servicios.
app.UseLINHttp();
app.UseAuthorization();
app.MapControllers();

BucketService.Default = "C:/Data/Cloud";

app.Run();