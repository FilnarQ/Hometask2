using Microsoft.EntityFrameworkCore;
using Hometask2.Models;
using Hometask2.Contexts;
using Hometask2.Services;
using Hometask2;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<Config>(builder.Configuration.GetSection(Config.ConfigName));
builder.Services.AddControllers();
builder.Services.AddScoped<BookContext, BookContext>();
builder.Services.AddScoped<BooksService, BooksService>();
builder.Services.AddDbContext<BookContext>(opt => opt.UseInMemoryDatabase("Books"));
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestMethod|Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestHeaders| Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestQuery|Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestBody;
    logging.ResponseHeaders.Clear();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.UseHttpLogging();

app.Run();
