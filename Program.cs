using Microsoft.EntityFrameworkCore;
using AsumbiCampusSystem.Data;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// -------------------------
// Controllers with Views
// -------------------------
builder.Services.AddControllersWithViews();

// -------------------------
// Session setup
// -------------------------
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// -------------------------
// Database connection
// -------------------------
builder.Services.AddDbContext<AppDbContextNew>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    )
);

// -------------------------
// Authentication (Cookies)
// -------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

// -------------------------
// Authorization Policies
// -------------------------
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StaffOnly", policy =>
        policy.RequireRole("Kitchen Staff", "Gate Staff"));

    options.AddPolicy("AdminOrDeputyOnly", policy =>
        policy.RequireRole("Admin", "Deputy", "Master Admin"));
});

// -------------------------
// Build app
// -------------------------
var app = builder.Build();

// -------------------------
// Rotativa Setup
// -------------------------
var env = app.Services.GetService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
if (env != null)
{
    RotativaConfiguration.Setup(env.WebRootPath, "Rotativa");
}

// -------------------------
// Middleware pipeline
// -------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ✅ Session first
app.UseSession();

// ✅ Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// -------------------------
// Routes
// -------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "meals",
    pattern: "Meals",
    defaults: new { controller = "Meals", action = "Index" }
);

app.MapControllerRoute(
    name: "gate",
    pattern: "Gate",
    defaults: new { controller = "Gate", action = "Index" }
);

app.Run();