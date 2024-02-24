using MongoDB.Bson.Serialization.Attributes;

namespace otsom.fs.Telegram.Bot.Auth.Spotify.Mongo.Entities;

public class Auth
{
    [BsonId]
    public string State { get; set; }

    public long UserId { get; set; }

    public string Code { get; set; }

    [BsonElement]
    public DateTime CreatedAt { get; } = DateTime.Now;
}