using API1;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.IncludeFields = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/hello", ([FromQuery] string name) =>
{
    return $"Hello {name}";
});

app.MapPost("/message", (Message msg) =>
{
    var result = msg.Data + " - return"; 

    // Write to file on disk: Check if the directory exists, if not, create it and append
    var curDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
    if (!Directory.Exists(curDir))
    {
        Directory.CreateDirectory(curDir);
    }
    using (StreamWriter writer = File.AppendText(Path.Combine(curDir, "data.txt")))
    {
        writer.WriteLine("API1 - " + result);
    }

    return result;
});

app.Run();
