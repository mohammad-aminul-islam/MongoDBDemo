using APIDemoMongoDB.Entities;
using MongoDB.Driver;

namespace APIDemoMongoDB.Data;

public class MongoDbService
{
    private readonly IMongoDatabase _mongoDatabase;
    public MongoDbService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DbConnection");
        var mongoUrl = MongoUrl.Create(connectionString);
        var mongoClient = new MongoClient(mongoUrl);
        _mongoDatabase = mongoClient.GetDatabase(mongoUrl.DatabaseName);
        _ = CreateIndex();
    }

    public IMongoDatabase Database
    {
        get
        {
            return _mongoDatabase;
        }
    }

    async Task CreateIndex()
    {
        var indexKeys = Builders<Customer>.IndexKeys.Text(x => x.CustomerName)
            .Text(x => x.Email);
        var collection = Database.GetCollection<Customer>("customer");
        await collection.Indexes.CreateOneAsync(new CreateIndexModel<Customer>(indexKeys));
    }
}
