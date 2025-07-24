module otsom.fs.Auth.Spotify.Repo

open System.Threading.Tasks
open Microsoft.Extensions.Options
open MongoDB.Driver
open SpotifyAPI.Web
open otsom.fs.Auth
open otsom.fs.Auth.Repo
open otsom.fs.Auth.Settings
open otsom.fs.Extensions
open System.Collections.Generic

type AuthRepo(authOptions: IOptions<AuthSettings>, db: IMongoDatabase) =
  let authCollection = db.GetCollection "auth"
  let tokenCollection = db.GetCollection "token"
  let settings = authOptions.Value

  interface IAuthRepo with
    member this.LoadInitedAuth(state) =
      let authFilter = Builders<Entities.Auth>.Filter.Eq((fun a -> a.State), state.Value)

      authCollection.Find(authFilter).SingleOrDefaultAsync()
      |> Task.map (Option.ofObj >> Option.map _.ToInited())

    member this.SaveInitedAuth(auth) = task { do! authCollection.InsertOneAsync(Entities.Auth.FromInited auth) }

    member this.GetAuthUrl(state) =
      LoginRequest(
        settings.CallbackUrl,
        settings.ClientId,
        LoginRequest.ResponseType.Code,
        Scope = (settings.Scopes |> List<string>),
        State = state.Value
      )
        .ToUri()

    member this.GetRefreshToken(code) =
      AuthorizationCodeTokenRequest(settings.ClientId, settings.ClientSecret, code.Value, settings.CallbackUrl)
      |> OAuthClient().RequestToken
      |> Task.map (_.RefreshToken >> RefreshToken)

    member this.SaveFulfilledAuth(auth: Fulfilled) =
      let stateFilter =
        Builders<Entities.Auth>.Filter.Eq((fun a -> a.State), auth.State.Value)

      let userIdFilter =
        Builders<Entities.Auth>.Filter.Eq((fun a -> a.AccountId), auth.AccountId.Value)

      let filter = Builders<Entities.Auth>.Filter.And(stateFilter, userIdFilter)

      authCollection.ReplaceOneAsync(filter, Entities.Auth.FromFulfilled auth)
      |> Task.map ignore

    member this.LoadFulfilledAuth(state) =
      let stateFilter = Builders<Entities.Auth>.Filter.Eq((fun a -> a.State), state.Value)

      authCollection.Find(stateFilter).SingleOrDefaultAsync()
      |> Task.map (Option.ofObj >> Option.map _.ToFulfilled())

    member this.SaveCompletedAuth(auth) =
      let filter =
        Builders<Entities.UserToken>.Filter.Eq((fun t -> t.AccountId), auth.AccountId.Value)

      tokenCollection.ReplaceOneAsync(filter, Entities.UserToken.FromCompleted auth, ReplaceOptions(IsUpsert = true))
      |> Task.map ignore

    member this.LoadCompletedAuth(accountId) =
      let filter =
        Builders<Entities.UserToken>.Filter.Eq((fun a -> a.AccountId), accountId.Value)

      tokenCollection.Find(filter).SingleOrDefaultAsync()
      |> Task.map (Option.ofObj >> Option.map _.ToCompletedAuth())