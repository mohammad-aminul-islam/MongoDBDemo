using APIDemoMongoDB.Data;
using APIDemoMongoDB.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    public async Task<IActionResult> GetAll(int page_size, int page_number, string? q)
    {
        var filters = Builders<Customer>.Filter.Or(
            Builders<Customer>.Filter.Text(q)
            );
        var total = await _customerCollection.CountDocumentsAsync(filters);
        var customers = await _customerCollection.Find(filters)
            .Skip((page_number - 1) * page_size)
            .Limit(page_size)
            .ToListAsync();
        var paginatedData = new
        {
            data = customers,
            pagination = new
            {
                page_size = page_size,
                page_number = page_number,
                total = total
            }
        };
        return Ok(paginatedData);
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetById(string id)
    {
        string email;
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
