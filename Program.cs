using Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information()
    .CreateLogger();
builder.Host.UseSerilog();

// add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IMyService, MyService>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// Example middleware using IMyService
app.Use(async (context, next) =>
{
    var service = context.RequestServices.GetRequiredService<IMyService>();
    service.LogCreation("Middleware 1");
    await next();
});

app.Use(async (context, next) =>
{
    var service = context.RequestServices.GetRequiredService<IMyService>();
    service.LogCreation("Middleware 2");
    await next();
});

app.MapControllers();

app.MapGet("/", (IMyService service) =>
{
    service.LogCreation("Root Endpoint");
    return Results.Ok("Hello from .NET 10 with Serilog & Swagger!");
});

app.Run();
