using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using RedisDemo.Models;

namespace RedisDemo.Repositories;

public class CachedPersonRepository: IPersonRepository
{
    private readonly IPersonRepository _personRepository;

    private readonly IDistributedCache _distributedCache;

    public CachedPersonRepository(IPersonRepository personRepository, IDistributedCache distributedCache)
    {
        _personRepository = personRepository;
        _distributedCache = distributedCache;
    }

    public async Task<Person?> GetPerson(int personId)
    {
        var personCache = await _distributedCache.GetAsync(personId.ToString());

        Person? person = null;
        
        if (personCache != null)
        {
            var serializePerson = Encoding.UTF8.GetString(personCache);
            person = JsonSerializer.Deserialize<Person>(serializePerson);
            return await Task.FromResult(person);
        }

        person = await _personRepository.GetPerson(personId);

        if (person != null)
        {
            var deserializedPerson = JsonSerializer.Serialize(person);
            personCache = Encoding.UTF8.GetBytes(deserializedPerson);
            await _distributedCache.SetAsync(personId.ToString(), personCache);
        }
        
        return await Task.FromResult(person);
    }
}