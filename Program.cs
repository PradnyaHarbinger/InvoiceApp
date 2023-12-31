using InvoiceApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using InvoiceApp.Authorization;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() /*Allows us to add and specify roles - Its just a functionality for creating roles - Our app is ready to use roles*/
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 10;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.AllowedForNewUsers = true;
    options.User.RequireUniqueEmail = true;
});

// creating authorizations
builder.Services.AddAuthorization(options =>
{
    // using this if we are not logged in or authenticated we will not beable to anything in the application
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddScoped<IAuthorizationHandler, InvoiceCreatorAuthorizationHandler>(); // now we can use this using DI
builder.Services.AddSingleton<IAuthorizationHandler, InvoiceManagerAuthorizationHandler>(); // singleton cz we want only 1 instance and we are also not using EF core for this purpose
builder.Services.AddSingleton<IAuthorizationHandler, InvoiceAdminAuthorizationHandler>(); // singleton as we want only 1 instance --||--

var app = builder.Build();

using (var scoped = app.Services.CreateScope())
{
    var services = scoped.ServiceProvider;

    //if you don't have a database when hosting it will be created
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();

    var seedUserPass = builder.Configuration.GetValue<string>("SeedUserPassword");
    
    await SeedData.Initialize(services, seedUserPass);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
