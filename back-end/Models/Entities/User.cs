using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace back_end.Models.Entities
{
    public enum Gender
    {
        Male,
        Female,
        Other
    }

    public class User
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String), BsonRequired]
        public string? Name { get; set; }

        [BsonElement("email"), BsonRepresentation(BsonType.String), BsonRequired]
        public string? Email { get; set; }

        [BsonElement("password"), BsonRepresentation(BsonType.String), BsonRequired]
        public string? Password { get; set; }

        [BsonElement("age"), BsonRepresentation(BsonType.Int32), BsonRequired]
        public int Age { get; set; }

        [BsonElement("gender"), BsonRepresentation(BsonType.String), BsonRequired]
        public Gender Gender { get; set; }

        [BsonElement("gender_preferences"), BsonRequired]
        public List<Gender>? GenderPreferences { get; set; }

        [BsonElement("bio"), BsonRepresentation(BsonType.String)]
        public string Bio { get; set; } = "";
        [BsonElement("image"), BsonRepresentation(BsonType.String)]
        public string? Image { get; set; }

        [BsonElement("likes")] // other users
        public List<ObjectId> Likes { get; set; } = new List<ObjectId>();

        [BsonElement("dislikes")] // other users
        public List<ObjectId> Dislikes { get; set; } = new List<ObjectId>();

        [BsonElement("matches")] // other users
        public List<ObjectId> Matches { get; set; } = new List<ObjectId>();
    }

    public class GenderSerializer : IBsonSerializer<Gender>
    {
        public Type ValueType => typeof(Gender);

        public Gender Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var genderString = context.Reader.ReadString();
            return (Gender)Enum.Parse(typeof(Gender), genderString, true);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Gender value)
        {
            context.Writer.WriteString(value.ToString().ToLower());
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            context.Writer.WriteString(value.ToString().ToLower());
        }

        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var genderString = context.Reader.ReadString();
            return (Gender)Enum.Parse(typeof(Gender), genderString, true);
        }
    }
}
