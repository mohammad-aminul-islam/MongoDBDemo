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
    }

    public IMongoDatabase Database
    {
        get
        {
            return _mongoDatabase;
        }
    }
}
