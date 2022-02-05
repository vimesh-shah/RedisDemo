using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using RedisDemo.Models;
using StackExchange.Redis;

namespace RedisDemo.Repositories;

public class CachedPersonRepository : IPersonRepository
{
    private readonly IPersonRepository _personRepository;
    private readonly IDistributedCache _distributedCache;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public CachedPersonRepository(
        IPersonRepository personRepository,
        IDistributedCache distributedCache,
        IConnectionMultiplexer connectionMultiplexer)
    {
        _personRepository = personRepository;
        _distributedCache = distributedCache;
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<Person?> GetPerson(int personId)
    {
        var personCache = await _distributedCache.GetAsync(personId.ToString());

        Person? person = null;

        if (personCache != null)
        {
            var serializePerson = Encoding.UTF8.GetString(personCache);
            person = JsonSerializer.Deserialize<Person>(serializePerson);
        }
        else
        {
            person = await _personRepository.GetPerson(personId);

            if (person != null)
            {
                var deserializedPerson = JsonSerializer.Serialize(person);
                personCache = Encoding.UTF8.GetBytes(deserializedPerson);
                await _distributedCache.SetAsync(personId.ToString(), personCache);
            }
        }

        if (person != null)
        {
            var subscriber = _connectionMultiplexer.GetSubscriber();
            await subscriber.PublishAsync("messages", $"Get user [ID={person.Id}, Name={person.Name}, Age={person.Age}]");
        }

        return await Task.FromResult(person);
    }
}