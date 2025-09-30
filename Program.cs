using AdminNeco.Components;
using AdminNeco.Services;
using MudBlazor.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

// Razor Components + Server Interactivity
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

builder.Services.AddMudServices();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// خواندن آدرس‌ها از appsettings.json
var apiBase = builder.Configuration["Api:BaseUrl"] ?? "http://localhost:5000/";
var fileBase = builder.Configuration["Api:FileUrl"] ?? "http://localhost:5001/";

// HttpClient برای API
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri(apiBase);
});

// HttpClient برای File API
builder.Services.AddHttpClient("fileApi", client =>
{
    client.BaseAddress = new Uri(fileBase);
});

// سرویس‌ها
builder.Services.AddScoped<AppState>();
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<FileService>();

// Session Storage برای توکن
builder.Services.AddScoped<ProtectedSessionStorage>();

var app = builder.Build();
 
// ❌ این بخش را حذف/کامنت کردیم چون فقط برای HTTPS لازم است
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Error");
//     app.UseHsts();
// }

// ❌ این خط را هم حذف/کامنت کردیم تا همیشه روی HTTP بماند
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
