namespace otsom.fs.Auth.Spotify

open Microsoft.Extensions.DependencyInjection
open otsom.fs.Auth.Repo
open otsom.fs.Auth.Spotify.Repo

[<RequireQualifiedAccess>]
module Startup =
  let addSpotifyAuth (services: IServiceCollection) =
    services.AddSingleton<IAuthRepo, AuthRepo>()
