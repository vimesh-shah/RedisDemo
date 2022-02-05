using RedisDemo.Models;

namespace RedisDemo.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly List<Person> _personList = new List<Person>
    {
        new Person
        {
            Id = 1,
            Name = "Vimesh Shah",
            Age = 28
        },
        new Person
        {
            Id = 2,
            Name = "Disha Shah",
            Age = 28
        },
    };
    
    public async Task<Person?> GetPerson(int personId)
    {
        return await Task.FromResult(_personList.SingleOrDefault(x => x.Id == personId));
    }
}