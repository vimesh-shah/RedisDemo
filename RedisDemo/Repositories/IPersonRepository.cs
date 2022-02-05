using RedisDemo.Models;

namespace RedisDemo.Repositories;

public interface IPersonRepository
{
    Task<Person?> GetPerson(int personId);
}