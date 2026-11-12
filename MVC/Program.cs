using MVC.Data;
using MVC.Services;
using MVC.Services.Interfaces;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Session configuration.
builder.Services.AddSession(options =>
{
options.IdleTimeout = TimeSpan.FromMinutes(30);
options.Cookie.HttpOnly = true;
options.Cookie.IsEssential = true;
});

// Neon PostgreSQL configuration.
var connectionString =
builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
throw new InvalidOperationException(
"The Neon database connection string is not configured.");
}

builder.Services.AddSingleton(
NpgsqlDataSource.Create(connectionString));

builder.Services.AddSingleton<DatabaseConnection>();

// Cloudinary configuration.
builder.Services.AddSingleton<
ICloudinaryService,
CloudinaryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
app.UseExceptionHandler("/Home/Error");
app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
