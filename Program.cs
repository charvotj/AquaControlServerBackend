using Microsoft.EntityFrameworkCore;
using AquaControlServerBackend.Authorization;
using AquaControlServerBackend.Helpers;
using AquaControlServerBackend.Services;

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.Configuration.GetSection("AppSettings");

// add services to DI container
{
    var services = builder.Services;
    var env = builder.Environment;
 
    // use sql server db in production and sqlite db in development
    if (env.IsProduction())
        services.AddDbContext<DataContext>();
    else
        services.AddDbContext<DataContext, SqliteDataContext>();
 
    services.AddCors();
    services.AddControllers();

    // configure automapper with all automapper profiles from this assembly
    services.AddAutoMapper(typeof(Program));

    // configure strongly typed settings object
    services.Configure<AppSettings>(appSettings);

    // configure DI for application services
    services.AddScoped<IJwtUtils, JwtUtils>();
    services.AddScoped<IUserService, UserService>();
}

var app = builder.Build();

// migrate any database changes on startup (includes initial db creation)
using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();    
    dataContext.Database.Migrate();
}

// configure HTTP request pipeline
{
    // global cors policy
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();

    // custom jwt auth middleware
    app.UseMiddleware<JwtMiddleware>();

    app.MapControllers();
}

var scheme = appSettings.GetValue<string>("Scheme");
var hostname = appSettings.GetValue<string>("Hostname");
var port = appSettings.GetValue<int>("Port");
//builder.Configuration.
app.Run($"{scheme}://{hostname}:{port}");