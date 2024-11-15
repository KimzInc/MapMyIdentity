using MapMyIdentity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints();


builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme);


builder.Services.AddAuthorizationBuilder()
    .AddPolicy("api", p =>
    {
        p.RequireAuthenticatedUser();
        p.AddAuthenticationSchemes(IdentityConstants.BearerScheme);
    });

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


app.MapGet("api/foo", () =>
{
    return new[] { "one", "two", "three", "four" };
})
    .RequireAuthorization("api");


app.MapGroup("api/auth")
    .MapIdentityApi<IdentityUser>();

app.Run();
