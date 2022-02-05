using Microsoft.Extensions.Caching.Distributed;
using RedisDemo.BackgroundServices;
using RedisDemo.Repositories;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConnectionMultiplexer>(x =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetValue<string>("RedisConnection")));

builder.Services.AddStackExchangeRedisCache(option =>
{
    option.ConnectionMultiplexerFactory = () =>
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        var connection = serviceProvider.GetService<IConnectionMultiplexer>();
        return Task.FromResult(connection);
    };
});

builder.Services.AddSingleton<IPersonRepository, PersonRepository>();
builder.Services.Decorate<IPersonRepository, CachedPersonRepository>();

builder.Services.AddHostedService<RedisSubscriber>();

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