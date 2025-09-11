using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Registers MVC controllers with views
builder.Services.AddControllersWithViews();

// Register Entity Framework Core DbContext using SQLite provider
// Uses the "DefaultConnection" string from appsettings.json
builder.Services.AddDbContext<EasyGamesProject.Data.ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

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
