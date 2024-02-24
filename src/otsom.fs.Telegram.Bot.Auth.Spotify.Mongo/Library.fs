namespace otsom.fs.Telegram.Bot.Auth.Spotify.Mongo

open Microsoft.FSharp.Core
open MongoDB.Driver
open otsom.fs.Extensions
open otsom.fs.Telegram.Bot.Auth.Spotify.Workflows
open otsom.fs.Telegram.Bot.Auth.Spotify.Mongo
open otsom.fs.Telegram.Bot.Auth.Spotify.Mongo.Mappings
open otsom.fs.Telegram.Bot.Core

module internal Workflows =
  [<RequireQualifiedAccess>]
  module Inited =
    let save (db: IMongoDatabase) : Inited.Save =
      fun auth ->
        let collection = db.GetCollection "auth"
        let dbAuth = auth |> Inited.toDb

        task { do! collection.InsertOneAsync(dbAuth) }

    let load (db: IMongoDatabase) : Inited.Load =
      fun state ->
        let collection = db.GetCollection "auth"
        let state = state |> State.value
        let authFilter = Builders<Entities.Auth>.Filter.Eq((fun a -> a.State), state)

        collection.Find(authFilter).SingleOrDefaultAsync()
        |> Task.map (Option.ofObj >> Option.map Inited.fromDb)

  [<RequireQualifiedAccess>]
  module Fulfilled =
    let save (db: IMongoDatabase) : Fulfilled.Save =
      fun auth ->
        let collection = db.GetCollection "auth"
        let state = auth.State |> State.value
        let userId = auth.UserId |> UserId.value
        let dbAuth = auth |> Fulfilled.toDb
        let stateFilter = Builders<Entities.Auth>.Filter.Eq((fun a -> a.State), state)
        let userIdFilter = Builders<Entities.Auth>.Filter.Eq((fun a -> a.UserId), userId)
        let filter = Builders<Entities.Auth>.Filter.And(stateFilter, userIdFilter)

        collection.ReplaceOneAsync(filter, dbAuth) |> Task.map ignore

    let load (db: IMongoDatabase) : Fulfilled.Load =
      fun state ->
        let collection = db.GetCollection "auth"
        let state = state |> State.value
        let stateFilter = Builders<Entities.Auth>.Filter.Eq((fun a -> a.State), state)

        collection.Find(stateFilter).SingleOrDefaultAsync()
        |> Task.map (Option.ofObj >> Option.map Fulfilled.fromDb)

  [<RequireQualifiedAccess>]
  module Completed =
    let save (db: IMongoDatabase) : Completed.Save =
      fun auth ->
        let collection = db.GetCollection "tokens"
        let userId = auth.UserId |> UserId.value
        let filter = Builders<Entities.UserToken>.Filter.Eq((fun t -> t.UserId), userId)
        let dbAuth = auth |> Completed.toDb

        collection.ReplaceOneAsync(filter, dbAuth, ReplaceOptions(IsUpsert = true))
        |> Task.map ignore

    let load (db: IMongoDatabase) : Completed.Load =
      fun userId ->
        let collection = db.GetCollection "tokens"
        let userId = userId |> UserId.value
        let filter = Builders<Entities.UserToken>.Filter.Eq((fun a -> a.UserId), userId)

        collection.Find(filter).SingleOrDefaultAsync()
        |> Task.map (Option.ofObj >> Option.map Completed.fromDb)
