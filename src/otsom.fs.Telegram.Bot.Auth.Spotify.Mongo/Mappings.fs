namespace otsom.fs.Telegram.Bot.Auth.Spotify.Mongo

open otsom.fs.Core
open otsom.fs.Telegram.Bot.Auth.Spotify.Mongo
open otsom.fs.Telegram.Bot.Auth.Spotify.Workflows

module internal Mappings =
  [<RequireQualifiedAccess>]
  module Inited =
    let fromDb (auth: Entities.Auth) : Inited =
      { State = State.parse auth.State
        UserId = auth.UserId |> UserId }

    let toDb (auth: Inited) : Entities.Auth =
      Entities.Auth(UserId = (auth.UserId |> UserId.value), State = (auth.State |> State.value))

  [<RequireQualifiedAccess>]
  module Fulfilled =
    let fromDb (auth: Entities.Auth) : Fulfilled =
      { State = State.parse auth.State
        UserId = auth.UserId |> UserId
        Code = Code auth.Code }

    let toDb (auth: Fulfilled) : Entities.Auth =
      Entities.Auth(UserId = (auth.UserId |> UserId.value), State = (auth.State |> State.value), Code = (auth.Code |> Code.value))

  [<RequireQualifiedAccess>]
  module Completed =
    let toDb (auth: Completed) : Entities.UserToken =
      Entities.UserToken(UserId = (auth.UserId |> UserId.value), RefreshToken = auth.Token)

    let fromDb (token: Entities.UserToken) : Completed =
      { UserId = (token.UserId |> UserId)
        Token = token.RefreshToken }
