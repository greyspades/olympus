using MongoDB.Driver;
using Slider.Model;

namespace Mongo.client;


public class MongoConnection {
    private readonly IConfiguration _config;
    private readonly IMongoDatabase _database;

    private readonly IMongoClient _client;
    public MongoConnection(IConfiguration config) {

        _config = config;

        var connectionString = _config.GetConnectionString("MongoDBSettings");

        // Console.WriteLine(connectionString);

        var client = new MongoClient("mongodb://localhost:27017");

        _client = client;

        _database = client.GetDatabase("LAPO_CMS");
    }

    public  IMongoCollection<Model> MakeConnection<Model>(string collection) {

        var mongoCollection = _database.GetCollection<Model>(collection);

        return mongoCollection;

    }
}
