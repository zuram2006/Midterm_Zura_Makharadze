using Microsoft.EntityFrameworkCore;
using Reddit;
using Reddit.Mapper;
using System.Text.Json.Serialization;
using Reddit.Middlewares;
using Reddit.Filters;

var builder = WebApplication.CreateBuilder(args);
//Model Validation by Zura

builder.Services.AddControllers(options => options.Filters.Add<ModelValidationActionFilter>());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplcationDBContext>(options => {
    options.UseSqlite(builder.Configuration.GetConnectionString("SqliteDb"));
    options.UseLazyLoadingProxies();
    options.LogTo(Console.WriteLine, LogLevel.Information);
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
               builder => builder.AllowAnyOrigin()
                                 .AllowAnyMethod()
                                 .AllowAnyHeader());
});
builder.Services.AddSingleton<IMapper, Mapper>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Type", "application/json");
    await next();
});


app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapGet("/throws", (context) => throw new Exception("Exception, it is zura fault"));

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
