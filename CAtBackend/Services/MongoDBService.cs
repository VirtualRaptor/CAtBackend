using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using CatApp.Models;

namespace CatApp.Services
{
    public class MongoDBService
    {
        private readonly IMongoDatabase _database;

        public MongoDBService(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("MongoDB");
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase("CatAppDatabase"); // Upewnij się, że nazwa bazy danych jest poprawna
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Favorite> Favorites => _database.GetCollection<Favorite>("Favorites");
    }
}
