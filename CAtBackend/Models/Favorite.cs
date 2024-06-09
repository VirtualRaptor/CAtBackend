using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CatApp.Models
{
    public class Favorite
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("catImageUrl")]
        public string CatImageUrl { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;
    }
}
