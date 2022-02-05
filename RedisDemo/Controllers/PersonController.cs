using Microsoft.AspNetCore.Mvc;
using RedisDemo.Models;
using RedisDemo.Repositories;

namespace RedisDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonController : ControllerBase
{
    private readonly IPersonRepository _personRepository;

    public PersonController(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetPerson(int personId)
    {
        var person = await _personRepository.GetPerson(personId);

        return person == null ? NotFound() : Ok(person);
    }
}