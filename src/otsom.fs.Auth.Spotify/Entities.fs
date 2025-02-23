namespace otsom.fs.Auth.Spotify.Entities

open System
open Microsoft.FSharp.Core
open MongoDB.Bson.Serialization.Attributes
open otsom.fs.Auth

[<AllowNullLiteral>]
type Auth() =
  [<BsonId>]
  member val State: string = "" with get, set

  [<BsonElement>]
  member val AccountId: string = null with get, set

  [<BsonElement>]
  member val Code: string = null with get, set

  [<BsonElement>]
  member val CreatedAt: DateTime = DateTime.UtcNow with get

  member this.ToInited() : Inited =
    { State = State.Parse this.State
      AccountId = AccountId this.AccountId }

  member this.ToFulfilled() : Fulfilled =
    { State = State.Parse this.State
      AccountId = AccountId this.AccountId
      Code = Code this.Code }

  static member FromInited(auth: Inited) =
    Auth(AccountId = auth.AccountId.Value, State = auth.State.Value)

  static member FromFulfilled(auth: Fulfilled) =
    Auth(AccountId = auth.AccountId.Value, State = auth.State.Value, Code = auth.Code.Value)

[<AllowNullLiteral>]
type UserToken() =
  [<BsonId>]
  member val AccountId: string = "" with get, set

  [<BsonElement>]
  member val RefreshToken: string = null with get, set

  [<BsonElement>]
  member val CreatedAt: DateTime = DateTime.UtcNow with get

  member this.ToCompletedAuth() : Completed =
    { AccountId = AccountId this.AccountId
      Token = RefreshToken this.RefreshToken }

  static member FromCompleted(auth: Completed) =
    UserToken(AccountId = auth.AccountId.Value, RefreshToken = auth.Token.Value)