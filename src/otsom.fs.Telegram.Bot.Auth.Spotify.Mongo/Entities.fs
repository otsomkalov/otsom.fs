namespace otsom.fs.Telegram.Bot.Auth.Spotify.Mongo

open System
open MongoDB.Bson.Serialization.Attributes

module internal Entities =

  [<AllowNullLiteral>]
  type Auth() =
    [<BsonId>]
    member val State : string = "" with get, set

    [<BsonElement>]
    member val AccountId: string = null with get, set
    [<BsonElement>]
    member val Code: string = null with get, set

    [<BsonElement>]
    member val CreatedAt: DateTime = DateTime.UtcNow with get

  [<AllowNullLiteral>]
  type UserToken() =
    [<BsonId>]
    member val AccountId: string = "" with get, set
    [<BsonElement>]
    member val RefreshToken: string = null with get, set

    [<BsonElement>]
    member val CreatedAt: DateTime = DateTime.UtcNow with get