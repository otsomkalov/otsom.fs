namespace otsom.fs.OAuth.Storage.Mongo

open System
open System.Runtime.CompilerServices
open FsToolkit.ErrorHandling
open Microsoft.Extensions.DependencyInjection
open MongoDB.Bson.Serialization.Attributes
open MongoDB.Driver
open otsom.fs.OAuth

[<AllowNullLiteral>]
type internal Auth() =
  [<BsonId>]
  member val StateHash = Unchecked.defaultof<string> with get, set

  [<BsonElement>]
  member val AccountId = Unchecked.defaultof<string> with get, set

  [<BsonElement>]
  member val Verifier = Unchecked.defaultof<string> with get, set

  [<BsonElement>]
  member val Provider = Unchecked.defaultof<string> with get, set

  [<BsonElement>]
  member val CreatedAt: DateTime = DateTime.UtcNow with get

  member this.ToInited() : Inited =
    {
      AccountId = AccountId this.AccountId
      StateHash = StateHash this.StateHash
      Verifier = PKCEVerifier this.Verifier
      Provider = OAuthProvider this.Provider
    }

  static member FromInited(auth: Inited) =
    Auth(AccountId = auth.AccountId.Value, StateHash = auth.StateHash.Value, Verifier = auth.Verifier.Value, Provider = auth.Provider.Value)

[<AllowNullLiteral; BsonIgnoreExtraElements>]
type internal UserToken() =
  [<BsonElement>]
  member val AccountId = Unchecked.defaultof<string> with get, set

  [<BsonElement>]
  member val AccessToken = Unchecked.defaultof<string> with get, set

  [<BsonElement>]
  member val RefreshToken = Unchecked.defaultof<string> with get, set

  [<BsonElement>]
  member val Provider = Unchecked.defaultof<string> with get, set

  [<BsonElement>]
  member val CreatedAt: DateTime = DateTime.UtcNow with get

  member this.ToCompletedAuth() : Completed =
    {
      AccountId = AccountId this.AccountId
      AccessToken = AccessToken this.AccessToken
      RefreshToken = RefreshToken this.RefreshToken
      Provider = OAuthProvider this.Provider
    }

  static member FromCompleted(auth: Completed) =
    UserToken(
      AccountId = auth.AccountId.Value,
      AccessToken = auth.AccessToken.Value,
      RefreshToken = auth.RefreshToken.Value,
      Provider = auth.Provider.Value
    )

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
    member this.PopInited(StateHash stateHash) =
      let authFilter = Builders<Auth>.Filter.Eq(_.StateHash, stateHash)

      authCollection.FindOneAndDeleteAsync<Auth>(authFilter)
      |> Task.map (Option.ofObj >> Option.map _.ToInited())

    member this.SaveInited(auth) = task { do! authCollection.InsertOneAsync(Auth.FromInited auth) }

    member this.SaveCompleted(auth) =
      let filter =
        Builders<UserToken>
          .Filter.And(
            Builders<UserToken>.Filter.Eq(_.AccountId, auth.AccountId.Value),
            Builders<UserToken>.Filter.Eq(_.Provider, auth.Provider.Value)
          )

      tokenCollection.ReplaceOneAsync(filter, UserToken.FromCompleted auth, ReplaceOptions(IsUpsert = true))
      |> Task.map ignore

    member this.LoadCompleted(accountId, provider) =
      let filter =
        Builders<UserToken>
          .Filter.And(
            Builders<UserToken>.Filter.Eq(_.AccountId, accountId.Value),
            Builders<UserToken>.Filter.Eq(_.Provider, provider.Value)
          )

      tokenCollection.Find(filter).SingleOrDefaultAsync()
      |> Task.map (Option.ofObj >> Option.map _.ToCompletedAuth())

type OAuthBuilderExtensions =
  [<Extension>]
  static member AddMongoStore(builder: IOAuthBuilder) =
    builder.Services.AddSingleton<IOAuthRepo, MongoOAuthRepo>() |> ignore

    builder