using AspClass.Db.SqlServer.Models;
using Microsoft.EntityFrameworkCore;
using NorthwindAppMvc;

var builder = WebApplication.CreateBuilder(args);

//Add Support for MVC to the DI Container (will be used by the App automatically)
builder.Services.AddControllersWithViews();

// Add DataBase Connection (from settings file) to the DI Container (builder.Services) for injecting it later
builder.Services.AddDbContext<NorthwindContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add SignalR
builder.Services.AddSignalR();
builder.Services.AddCors(options => {
    options.AddPolicy("CorsPolicy", builder => builder
    .WithOrigins("http://localhost:4200")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());
});

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.UseStaticFiles();

// Einrichten des Routings
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapHub<ChatHub>("/chatHub");
app.UseCors("CorsPolicy");

app.Run();