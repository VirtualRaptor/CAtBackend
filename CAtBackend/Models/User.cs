using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using BCrypt.Net;

namespace CatApp.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;

        public void HashPassword()
        {
            Password = BCrypt.Net.BCrypt.HashPassword(Password);
        }

        public bool VerifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, Password);
        }
    }
}
