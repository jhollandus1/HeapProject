using HeapProject;
using HeapProject.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
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
builder.Services.AddControllersWithViews();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
