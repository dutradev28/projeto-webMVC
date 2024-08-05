using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesWebMvc.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using SalesWebMvc.Services;
using System.Globalization;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<SalesWebMvcContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("SalesWebMvcContext"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("SalesWebMvcContext")),
        mysqlOptions => mysqlOptions.MigrationsAssembly("SalesWebMvc")));


builder.Services.AddTransient<SeedingService>();
builder.Services.AddScoped<SeedingService>();
builder.Services.AddTransient<SellerService>();
builder.Services.AddScoped<SellerService>();
builder.Services.AddTransient<DepartmentService>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<SalesRecordsService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();
var enUS = new CultureInfo("en-US"); 
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(enUS),
    SupportedCultures = new List<CultureInfo> { enUS },
    SupportedUICultures = new List<CultureInfo> { enUS }
}; 
app.UseRequestLocalization(localizationOptions);

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
