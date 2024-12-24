namespace otsom.fs.Telegram.Bot.Auth.Spotify

open System.Collections.Generic
open System.Threading.Tasks
open Microsoft.Extensions.Options
open SpotifyAPI.Web
open otsom.fs.Telegram.Bot.Auth.Spotify.Settings
open otsom.fs.Extensions
open shortid

module Workflows =
  type State =
    private
    | State of string

    member this.Value = let (State state) = this in state

  [<RequireQualifiedAccess>]
  module State =
    let create () = ShortId.Generate() |> State

    let parse str = State str

  type Inited = { AccountId: AccountId; State: State }

  [<RequireQualifiedAccess>]
  module Inited =
    type Save = Inited -> Task<unit>
    type Load = State -> Task<Inited option>

  let init (saveInitedAuth: Inited.Save) (options: IOptions<SpotifySettings>) : Auth.Init =
    let settings = options.Value

    fun userId scopes ->
      let initedAuth =
        { AccountId = userId
          State = State.create () }

      task {
        do! saveInitedAuth initedAuth

        let loginRequest =
          LoginRequest(
            settings.CallbackUrl,
            settings.ClientId,
            LoginRequest.ResponseType.Code,
            Scope = (scopes |> List<string>),
            State = initedAuth.State.Value
          )

        return loginRequest.ToUri().ToString()
      }

  type Code = Code of string

  [<RequireQualifiedAccess>]
  module Code =
    let value (Code code) = code

  type Fulfilled =
    { AccountId: AccountId
      State: State
      Code: Code }

  [<RequireQualifiedAccess>]
  module Fulfilled =
    type Save = Fulfilled -> Task<unit>
    type Load = State -> Task<Fulfilled option>

  let fulfill (loadInitedAuth: Inited.Load) (saveFulfilledAuth: Fulfilled.Save) : Auth.Fulfill =
    let addCodeToAuth code =
      fun (initedAuth: Inited option) ->
        match initedAuth with
        | Some auth -> task {
            let fulfilledAuth: Fulfilled =
              { State = auth.State
                AccountId = auth.AccountId
                Code = Code code }

            do! saveFulfilledAuth fulfilledAuth

            return (auth.State.Value |> Ok)
          }
        | None -> Auth.FulfillmentError.StateNotFound |> Error |> Task.FromResult

    fun state code -> state |> State.parse |> loadInitedAuth |> Task.bind (addCodeToAuth code)

  type Completed = { AccountId: AccountId; Token: string }

  [<RequireQualifiedAccess>]
  module Completed =
    type Save = Completed -> Task<unit>
    type Load = AccountId -> Task<Completed option>

  let complete
    (loadFulfilledAuth: Fulfilled.Load)
    (saveCompletedAuth: Completed.Save)
    (options: IOptions<SpotifySettings>)
    : Auth.Complete =
    let settings = options.Value

    let createCompletedAuth (auth: Fulfilled) = task {
      let! token =
        (settings.ClientId, settings.ClientSecret, (auth.Code |> Code.value), settings.CallbackUrl)
        |> AuthorizationCodeTokenRequest
        |> OAuthClient().RequestToken
        |> Task.map _.RefreshToken

      let completed =
        { AccountId = auth.AccountId
          Token = token }

      do! saveCompletedAuth completed
    }

    let validateAuth userId =
      fun (auth: Fulfilled) ->
        if userId = auth.AccountId then
          Ok(auth)
        else
          Error(Auth.StateDoesntBelongToUser)

    fun userId state ->
      state
      |> State.parse
      |> loadFulfilledAuth
      |> Task.map (Result.ofOption Auth.CompleteError.StateNotFound)
      |> Task.map (Result.bind (validateAuth userId))
      |> Task.bind (Result.taskMap createCompletedAuth)