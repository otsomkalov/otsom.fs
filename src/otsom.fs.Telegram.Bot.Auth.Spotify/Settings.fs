namespace otsom.fs.Telegram.Bot.Auth.Spotify

open System

module Settings =
  [<CLIMutable>]
  type SpotifySettings =
    { ClientId: string
      ClientSecret: string
      CallbackUrl: Uri
      Scopes: string array }

    static member SectionName = "Spotify"