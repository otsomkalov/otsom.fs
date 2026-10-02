namespace otsom.fs.OAuth.Storage.Mongo

open System
open FsToolkit.ErrorHandling
open MongoDB.Bson.Serialization.Attributes
open MongoDB.Driver
open otsom.fs.OAuth

[<AllowNullLiteral>]
type internal Auth() =
  [<BsonId>]
  member val AccountId = Unchecked.defaultof<string> with get, set

  [<BsonElement>]
  member val State = Unchecked.defaultof<string> with get, set

  [<BsonElement>]
  member val Verifier = Unchecked.defaultof<string> with get, set

  [<BsonElement>]
  member val CreatedAt: DateTime = DateTime.UtcNow with get

  member this.ToInited() : Inited =
    {
      AccountId = AccountId this.AccountId
      State = State this.State
      Verifier = PKCEVerifier this.Verifier
    }

  static member FromInited(auth: Inited) =
    Auth(AccountId = auth.AccountId.Value, State = auth.State.Value, Verifier = auth.Verifier.Value)

[<AllowNullLiteral>]
type internal UserToken() =
  [<BsonId>]
  member val AccountId = Unchecked.defaultof<string> with get, set

  [<BsonElement>]
  member val AccessToken = Unchecked.defaultof<string> with get, set

  [<BsonElement>]
  member val RefreshToken = Unchecked.defaultof<string> with get, set

  [<BsonElement>]
  member val CreatedAt: DateTime = DateTime.UtcNow with get

  member this.ToCompletedAuth() : Completed =
    {
      AccountId = AccountId this.AccountId
      AccessToken = AccessToken this.AccessToken
      RefreshToken = RefreshToken this.RefreshToken
    }

  static member FromCompleted(auth: Completed) =
    UserToken(AccountId = auth.AccountId.Value, AccessToken = auth.AccessToken.Value, RefreshToken = auth.RefreshToken.Value)

[<CLIMutable>]
type StorageSettings =
  {
    ConnectionString: string
    Container: string
  }

  static member SectionName = "Storage"

type MongoOAuthRepo(db: IMongoDatabase) =
  let authCollection = db.GetCollection "auth"
  let tokenCollection = db.GetCollection "token"

  interface IOAuthRepo with
    member this.LoadInited(AccountId accountId) =
      let authFilter = Builders<Auth>.Filter.Eq(_.AccountId, accountId)

      authCollection.Find(authFilter).SingleOrDefaultAsync()
      |> Task.map (Option.ofObj >> Option.map _.ToInited())

    member this.SaveInited(auth) = task { do! authCollection.InsertOneAsync(Auth.FromInited auth) }

    member this.SaveCompleted(auth) =
      let filter = Builders<UserToken>.Filter.Eq(_.AccountId, auth.AccountId.Value)

      tokenCollection.ReplaceOneAsync(filter, UserToken.FromCompleted auth, ReplaceOptions(IsUpsert = true))
      |> Task.map ignore

    member this.LoadCompleted(accountId) =
      let filter = Builders<UserToken>.Filter.Eq(_.AccountId, accountId.Value)

      tokenCollection.Find(filter).SingleOrDefaultAsync()
      |> Task.map (Option.ofObj >> Option.map _.ToCompletedAuth())