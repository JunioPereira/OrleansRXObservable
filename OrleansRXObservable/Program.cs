using SiloHost;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddHostedService<SiloHostService>();

builder.Services.AddSingleton<IClusterClient>(a =>
{
    var host = StartOrleansClient();
    return host.GetAwaiter().GetResult().Services.GetRequiredService<IClusterClient>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


static async Task<IHost> StartOrleansClient()
{
    var client = new HostBuilder()
        .UseOrleansClient(c =>
        {
            c.UseLocalhostClustering();
        })
        .ConfigureLogging(l =>
        {
            l.SetMinimumLevel(LogLevel.Warning).AddConsole();
        });

    var host = client.Build();
    await host.StartAsync();
    return host;
}