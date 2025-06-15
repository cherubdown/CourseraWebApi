using Serilog;


var builder = WebApplication.CreateBuilder(args);



Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information()
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddSingleton<IMyService, MyService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    var service = context.RequestServices.GetRequiredService<IMyService>();
    service.LogCreation("1st request to the service");
    await next();
});

app.Use(async (context, next) =>
{
    var service = context.RequestServices.GetRequiredService<IMyService>();
    service.LogCreation("2nd request to the service");
    await next();
});

app.MapGet("/", (IMyService service) =>
{
    service.LogCreation("Root");
    return Results.Ok("Check the logs for service creation messages.");
});

try
{
    app.MapControllers();
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred while mapping controllers: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    throw;
}


app.Run();