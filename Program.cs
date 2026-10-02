using HeapProject;
using HeapProject.Identity;
using HeapProject.CultureProvider;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.CookiePolicy;


var builder = WebApplication.CreateBuilder(args);

// Configure localization options
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("es-ES"),
        new CultureInfo("fr-FR")
    };

    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    // Insert your custom DB provider at index 0 (Highest priority)
    options.RequestCultureProviders.Insert(0, new DatabaseCultureProvider());
});

// Add site-wide cookie security options
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Strict;    // do not share cookies across sites
    options.HttpOnly = HttpOnlyPolicy.Always;          // prevent client-side scripts from accessing cookies
    options.Secure = CookieSecurePolicy.Always;         // ensure cookies are only sent over HTTPS
});


var connectionString = builder.Configuration.GetConnectionString("HeapProjectContext") ?? throw new InvalidOperationException("Connection string 'HeapProjectContext' not found.");

builder.Services.AddDbContext<HeapProjectContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        // the unique username constraint is already being enforced at the application level (somehow - not explicit here),
        // but NormalizedUserName is (also) enforced at the index (Database) level
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDBContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});

var app = builder.Build();

 app.UseRequestLocalization();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
