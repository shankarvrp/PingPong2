using API1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Serilog;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ILogger = Serilog.ILogger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var externalApiBaseUrl = Environment.GetEnvironmentVariable("EXTERNAL_API_URL") ?? "http://localhost:5002";

// Add logging 
builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Get the Pong API URL
builder.Services.AddHttpClient("APIClient", c => { c.BaseAddress = new Uri(externalApiBaseUrl); });


var app = builder.Build();


app.MapGet("/hello", ([FromQuery] string name) =>
{
    return $"Hello {name}";
});


// Send endpoint that posts to Pong and sends some data back 
app.MapPost("/", async (Message todo, IHttpClientFactory httpFactory, ILogger logger) => {
    
    // Check for incoming
    if (todo is not null)
        todo.Data= todo.Data + " - Tested";
    else
        todo = new Message();

    // Write to file on disk: Check if the directory exists, if not, create it and append
    var curDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
    if (!Directory.Exists(curDir))
    {
        Directory.CreateDirectory(curDir);
    }
    using (StreamWriter writer = File.AppendText(Path.Combine(curDir, "data.txt")))
    {
        writer.WriteLine("API0 - " + todo.Data);
    }

    // Call the external API
    var httpClient = httpFactory.CreateClient("APIClient");
    var data = JsonSerializer.Serialize(todo);
    var httpResponse = await httpClient.PostAsync("/todos", new StringContent(data, Encoding.UTF8, "application/json"));
    var pongResponse = await httpResponse.Content.ReadAsStringAsync();

    logger.Information(httpClient.BaseAddress.ToString());

    return pongResponse;
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.Run();
