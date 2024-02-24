namespace otsom.fs.Telegram.Bot.Auth.Spotify

open System.Threading.Tasks
open otsom.fs.Telegram.Bot.Core

[<RequireQualifiedAccess>]
module Auth =
  type Init = UserId -> string list -> Task<string>

  type FulfillmentError = | StateNotFound

  type Fulfill = string -> string -> Task<Result<string, FulfillmentError>>

  type CompleteError =
    | StateNotFound
    | StateDoesntBelongToUser

  type Complete = UserId -> string -> Task<Result<unit, CompleteError>>