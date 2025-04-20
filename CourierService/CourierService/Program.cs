using DataBase.CRUD;
using DataBase.DataBase;
using Microsoft.EntityFrameworkCore;
using Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<CourierServiceContext>(options=> options.UseNpgsql("Host=localhost;Database=CourierService;Username=postgres;Password=123456"));
builder.Services.AddScoped<CientCRUD>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<RegistrationService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=MainMenu}/{id?}");

app.Run();