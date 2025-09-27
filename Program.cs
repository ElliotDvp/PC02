using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PC02.Data;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.AspNetCore.DataProtection.StackExchangeRedis;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var env    = builder.Environment;

// 1) Conectar Redis y configurar DataProtection en Redis
var redisConnString = config.GetConnectionString("Redis")
    ?? throw new InvalidOperationException("Connection string 'Redis' not found.");
var redisOptions = ConfigurationOptions.Parse(redisConnString);
redisOptions.AbortOnConnectFail = false;
redisOptions.ConnectTimeout    = 5000;

// Conexión multiplexer reusable
var muxer = ConnectionMultiplexer.Connect(redisOptions);

// Persistir claves de DataProtection en Redis para evitar errores de cookie
builder.Services.AddDataProtection()
    .SetApplicationName("PC02App")
    .PersistKeysToStackExchangeRedis(muxer, "DataProtection-Keys");

// 2) Cache distribuido y singleton multiplexer
builder.Services.AddStackExchangeRedisCache(opts =>
{
    opts.ConfigurationOptions = redisOptions;
    opts.InstanceName         = "PC02Cache:";
});
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => muxer);

// 3) Sesión
builder.Services.AddSession(opts =>
{
    opts.IdleTimeout    = TimeSpan.FromMinutes(30);
    opts.Cookie.HttpOnly  = true;
    opts.Cookie.IsEssential = true;
});

// 4) EF Core + SQLite
var defaultConn = config.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseSqlite(defaultConn));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 5) Identity + MVC
builder.Services
    .AddDefaultIdentity<IdentityUser>(o => o.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 6) Seed de datos
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Initialize(db);
}

// 7) Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
