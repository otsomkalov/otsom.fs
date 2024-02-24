namespace otsom.fs.Telegram.Bot.Auth.Spotify.Mongo

open Microsoft.Extensions.DependencyInjection
open MongoDB.Driver
open otsom.fs.Extensions.DependencyInjection
open otsom.fs.Telegram.Bot.Auth.Spotify.Workflows
open otsom.fs.Telegram.Bot.Auth.Spotify.Mongo.Workflows

[<RequireQualifiedAccess>]
module Startup =
  let addMongoSpotifyAuth (services: IServiceCollection) =
    services
      .BuildSingleton<Inited.Load, IMongoDatabase>(Inited.load)
      .BuildSingleton<Inited.Save, IMongoDatabase>(Inited.save)
      .BuildSingleton<Fulfilled.Load, IMongoDatabase>(Fulfilled.load)
      .BuildSingleton<Fulfilled.Save, IMongoDatabase>(Fulfilled.save)
      .BuildSingleton<Completed.Load, IMongoDatabase>(Completed.load)
      .BuildSingleton<Completed.Save, IMongoDatabase>(Completed.save)
