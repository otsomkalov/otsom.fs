namespace otsom.fs.Telegram.Bot.Auth.Spotify

open System.Collections.Generic
open System.Threading.Tasks
open Microsoft.Extensions.Options
open SpotifyAPI.Web
open otsom.fs.Telegram.Bot.Auth.Spotify.Settings
open otsom.fs.Telegram.Bot.Core
open otsom.fs.Extensions
open shortid

module Workflows =
  type State = private State of string

  [<RequireQualifiedAccess>]
  module State =
    let create () = ShortId.Generate() |> State

    let parse str = State str

    let value (State key) = key

  type Inited = { UserId: UserId; State: State }

  [<RequireQualifiedAccess>]
  module Inited =
    type Save = Inited -> Task<unit>
    type Load = State -> Task<Inited option>

  let init (saveInitedAuth: Inited.Save) (options: IOptions<SpotifySettings>) : Auth.Init =
    let settings = options.Value

    fun userId scopes ->
      let initedAuth =
        { UserId = userId
          State = State.create () }

      task {
        do! saveInitedAuth initedAuth

        let loginRequest =
          LoginRequest(
            settings.CallbackUrl,
            settings.ClientId,
            LoginRequest.ResponseType.Code,
            Scope = (scopes |> List<string>),
            State = (initedAuth.State |> State.value)
          )

        return loginRequest.ToUri().ToString()
      }

  type Code = Code of string

  [<RequireQualifiedAccess>]
  module Code =
    let value (Code code) = code

  type Fulfilled =
    { UserId: UserId
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
        | Some auth ->
          task {
            let fulfilledAuth: Fulfilled =
              { State = auth.State
                UserId = auth.UserId
                Code = Code code }

            do! saveFulfilledAuth fulfilledAuth

            return (auth.State |> State.value |> Ok)
          }
        | None -> Auth.FulfillmentError.StateNotFound |> Error |> Task.FromResult

    fun state code -> state |> State.parse |> loadInitedAuth |> Task.bind (addCodeToAuth code)

  type Completed = { UserId: UserId; Token: string }

  [<RequireQualifiedAccess>]
  module Completed =
    type Save = Completed -> Task<unit>
    type Load = UserId -> Task<Completed option>

  let complete
    (loadFulfilledAuth: Fulfilled.Load)
    (saveCompletedAuth: Completed.Save)
    (options: IOptions<SpotifySettings>)
    : Auth.Complete =
    let settings = options.Value

    let createCompletedAuth (auth: Fulfilled) =
      task {
        let! token =
          (settings.ClientId, settings.ClientSecret, (auth.Code |> Code.value), settings.CallbackUrl)
          |> AuthorizationCodeTokenRequest
          |> OAuthClient().RequestToken
          |> Task.map _.RefreshToken

        let completed = { UserId = auth.UserId; Token = token }

        do! saveCompletedAuth completed
      }

    let validateAuth userId =
      fun (auth: Fulfilled) ->
        if userId = auth.UserId then
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
