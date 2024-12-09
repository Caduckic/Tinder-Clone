using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace back_end.Models.Entities
{
    public class Message
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("sender_id"), BsonRepresentation(BsonType.ObjectId), BsonRequired]
        public string? SenderID { get; set; }

        [BsonElement("receiver_id"), BsonRepresentation(BsonType.ObjectId), BsonRequired]
        public string? ReceiverId { get; set; }

        [BsonElement("content"), BsonRepresentation(BsonType.String), BsonRequired]
        public string? Content { get; set; }

        [BsonElement("_createdAt"), BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
