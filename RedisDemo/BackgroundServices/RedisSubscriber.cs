using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace RedisDemo.BackgroundServices;
public class RedisSubscriber : BackgroundService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisSubscriber(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriber = _connectionMultiplexer.GetSubscriber();
        return subscriber.SubscribeAsync("messages", OnMessageReceived);
    }

    private void OnMessageReceived(RedisChannel channel, RedisValue value)
    {
        Console.WriteLine($"Value received: {value}");
    }
}
