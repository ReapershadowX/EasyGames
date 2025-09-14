using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Registers MVC controllers with views
builder.Services.AddControllersWithViews();
// Register Entity Framework Core DbContext using SQLite provider
// Uses the "DefaultConnection" string from appsettings.json
builder.Services.AddDbContext<EasyGamesProject.Data.ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add authentication services using cookie authentication as the default scheme
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Users/Login";          // Path to redirect for login when unauthorized
        options.AccessDeniedPath = "/Home/AccessDenied"; // Path for access denied page
    });

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // In non-development scenarios, use a custom error handling page
    app.UseExceptionHandler("/Home/Error");
    // HTTP Strict Transport Security (HSTS) enforces HTTPS for 30 days by default
    app.UseHsts();
}
// Redirect HTTP requests to HTTPS
app.UseHttpsRedirection();
// Serve static files from wwwroot (CSS, JS, images)
app.UseStaticFiles();
// Enable routing middleware
app.UseRouting();

// Add authentication middleware before authorization
app.UseAuthentication();

// Apply authorization middleware (checks [Authorize] attributes)
app.UseAuthorization();
// Map static asset routes (custom extension for static content)
app.MapStaticAssets();
// Define the default route: HomeController → Index action
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
)
.WithStaticAssets(); // Ensure static asset mapping is applied here as well
// Run the application
app.Run();
