using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EventManager.Data;
using EventManager.Models;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container
builder.Services.AddControllersWithViews();

//acquires connection string from file
var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");

//uses connection string to establish connection w/ MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

//establishes repository
builder.Services.AddTransient<IEventRepository, EventRepository>();

//identity prerequisites
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//Add services to the container
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//establish seedData
using (var scope = app.Services.CreateScope())
{
    await ConfigureIdentity.CreateAdminUserAsync(scope.ServiceProvider);
    var dbContext = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();
}

app.Run();
