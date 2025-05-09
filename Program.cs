using cars_web.Extensions;
using cars_web.Services;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register HttpClient and ApiService
builder.Services.AddHttpClient();
builder.Services.AddScoped<cars_web.Services.ApiService>();
builder.Services.AddScoped<cars_web.Services.AuthService>();
builder.Services.AddScoped<PaintingServiceClient>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<AuthService>();
builder.Services.AddHttpClient<AdminService>();
builder.Services.AddScoped<FileService>();

// Add session and authentication services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => 
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add API settings to configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Добавляем поддержку MIME типов для 3D-моделей
var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
provider.Mappings[".gltf"] = "model/gltf+json";
provider.Mappings[".glb"] = "model/gltf-binary";
provider.Mappings[".bin"] = "application/octet-stream";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
