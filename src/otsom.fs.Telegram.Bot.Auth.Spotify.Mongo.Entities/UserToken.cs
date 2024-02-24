using MongoDB.Bson.Serialization.Attributes;

namespace otsom.fs.Telegram.Bot.Auth.Spotify.Mongo.Entities;

public class UserToken
{
    [BsonId]
    public long UserId { get; set; }

    public string RefreshToken { get; set; }

    [BsonElement]
    public DateTime CreatedAt { get; } = DateTime.Now;
}