using Microsoft.EntityFrameworkCore;
using Database_Access_Layer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Business_Logic_Layer;
using Microsoft.AspNetCore.Session;
using System.Configuration;
using System;
using BookStore.Pages.Shared.Components;
using BookStore.Views.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<Dbcontext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the UserSignupLogin service.
builder.Services.AddScoped<UserSignupLogin>();
builder.Services.AddScoped<InventoryManager>();
builder.Services.AddScoped<OrderProcessor>();
builder.Services.AddScoped<CustomExceptionFilterAttribute>();

builder.Services.AddTransient<LogoutViewComponent>();

builder.Services.AddHttpContextAccessor();


builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(1800);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();


app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "logout",
        pattern: "/Shared/Components/LogoutViewComponent/OnPost",
        defaults: new { controller = "LogoutViewComponent", action = "OnPost" }
    );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Order}/{action=Index}/{id?}"
    );
});

app.Run();






