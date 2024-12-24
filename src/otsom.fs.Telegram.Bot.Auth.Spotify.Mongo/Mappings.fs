namespace otsom.fs.Telegram.Bot.Auth.Spotify.Mongo

open otsom.fs.Telegram.Bot.Auth.Spotify
open otsom.fs.Telegram.Bot.Auth.Spotify.Mongo
open otsom.fs.Telegram.Bot.Auth.Spotify.Workflows

module internal Mappings =
  [<RequireQualifiedAccess>]
  module Inited =
    let fromDb (auth: Entities.Auth) : Inited =
      { State = State.parse auth.State
        AccountId = auth.AccountId |> AccountId }

    let toDb (auth: Inited) : Entities.Auth =
      Entities.Auth(AccountId = auth.AccountId.Value, State = auth.State.Value)

  [<RequireQualifiedAccess>]
  module Fulfilled =
    let fromDb (auth: Entities.Auth) : Fulfilled =
      { State = State.parse auth.State
        AccountId = auth.AccountId |> AccountId
        Code = Code auth.Code }

    let toDb (auth: Fulfilled) : Entities.Auth =
      Entities.Auth(AccountId = auth.AccountId.Value, State = auth.State.Value, Code = (auth.Code |> Code.value))

  [<RequireQualifiedAccess>]
  module Completed =
    let toDb (auth: Completed) : Entities.UserToken =
      Entities.UserToken(AccountId = auth.AccountId.Value, RefreshToken = auth.Token)

    let fromDb (token: Entities.UserToken) : Completed =
      { AccountId = (token.AccountId |> AccountId)
        Token = token.RefreshToken }