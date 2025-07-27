using APIDemoMongoDB.Data;
using APIDemoMongoDB.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace APIDemoMongoDB.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerApiController : ControllerBase
{
    private readonly IMongoCollection<Customer> _customerCollection;
    public CustomerApiController(MongoDbService mongoDbService)
    {
        _customerCollection = mongoDbService.Database.GetCollection<Customer>("customer");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _customerCollection.Find(FilterDefinition<Customer>.Empty).ToListAsync();
        return Ok(customers);
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetById(string id)
    {
        var filter = Builders<Customer>.Filter.Eq(x => x.Id, id);
        var customer = await _customerCollection.Find(filter).FirstOrDefaultAsync();
        return customer == null ? NotFound() : Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Add(Customer customer)
    {
        await _customerCollection.InsertOneAsync(customer);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    [HttpPut]
    public async Task<IActionResult> Update(Customer customer)
    {
        var filter = Builders<Customer>.Filter.Eq(x => x.Id, customer.Id);
        await _customerCollection.ReplaceOneAsync(filter, customer);
        return Ok();
    }

    [HttpDelete("id")]
    public async Task<IActionResult> DeleteById(string id)
    {
        var filter = Builders<Customer>.Filter.Eq(x => x.Id, id);
        await _customerCollection.DeleteOneAsync(filter);
        return Ok();
    }
}
