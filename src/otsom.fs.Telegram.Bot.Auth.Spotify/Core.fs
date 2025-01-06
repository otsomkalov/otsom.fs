namespace otsom.fs.Telegram.Bot.Auth.Spotify

open System.Threading.Tasks

type AccountId =
  | AccountId of string

  member this.Value = let (AccountId value) = this in value

[<RequireQualifiedAccess>]
module Auth =
  type Init = AccountId -> Task<string>

  type FulfillmentError = | StateNotFound

  type Fulfill = string -> string -> Task<Result<string, FulfillmentError>>

  type CompleteError =
    | StateNotFound
    | StateDoesntBelongToUser

  type Complete = AccountId -> string -> Task<Result<unit, CompleteError>>