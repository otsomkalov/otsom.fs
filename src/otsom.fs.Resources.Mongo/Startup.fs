module otsom.fs.Resources.Mongo.Startup

#nowarn "20"

open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open MongoDB.Bson.Serialization
open MongoDB.Driver
open otsom.fs.Extensions.DependencyInjection
open otsom.fs.Resources

let addMongoResources (cfg: IConfiguration) (services: IServiceCollection) =

  BsonClassMap.RegisterClassMap<Resource>(fun cm ->
    cm.AutoMap()
    cm.SetIgnoreExtraElements true
    ())

  services
    .BuildSingleton<IMongoCollection<Resource>, IMongoDatabase>(_.GetCollection("resources"))

    .AddSingleton<IResourceRepo, ResourceRepo>()