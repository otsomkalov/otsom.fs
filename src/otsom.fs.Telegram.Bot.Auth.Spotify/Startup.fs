namespace otsom.fs.Telegram.Bot.Auth.Spotify

open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Options
open otsom.fs.Extensions.DependencyInjection
open otsom.fs.Telegram.Bot.Auth.Spotify.Settings
open otsom.fs.Telegram.Bot.Auth.Spotify.Workflows

[<RequireQualifiedAccess>]
module Startup =
  let addSpotifyAuth (configuration: IConfiguration) (services: IServiceCollection) =
    services
      .Configure<SpotifySettings>(configuration.GetSection(SpotifySettings.SectionName))

      .BuildScoped<Auth.Init, Inited.Save, IOptions<SpotifySettings>>(init)
      .BuildScoped<Auth.Fulfill, Inited.Load, Fulfilled.Save>(fulfill)
      .BuildScoped<Auth.Complete, Fulfilled.Load, Completed.Save, IOptions<SpotifySettings>>(complete)
