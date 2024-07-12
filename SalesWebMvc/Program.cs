using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesWebMvc.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using SalesWebMvc.Services;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<SalesWebMvcContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("SalesWebMvcContext"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("SalesWebMvcContext")),
        mysqlOptions => mysqlOptions.MigrationsAssembly("SalesWebMvc")));


builder.Services.AddTransient<SeedingService>();
builder.Services.AddScoped<SeedingService>();
builder.Services.AddScoped<SellerService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<SalesWebMvcContext>();
        var seedingService = services.GetRequiredService<SeedingService>();
        seedingService.Seed(); 
    }
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
