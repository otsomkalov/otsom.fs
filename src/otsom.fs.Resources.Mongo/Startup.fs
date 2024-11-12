module otsom.fs.Resources.Mongo.Startup

open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Options
open MongoDB.Bson
open MongoDB.Bson.Serialization
open MongoDB.Bson.Serialization.Attributes
open MongoDB.Bson.Serialization.IdGenerators
open MongoDB.Bson.Serialization.Serializers
open MongoDB.Driver
open otsom.fs.Resources
open otsom.fs.Extensions.DependencyInjection
open otsom.fs.Resources.Mongo.Repos
open otsom.fs.Resources.Repos

let addMongoResources (cfg: IConfiguration) (services: IServiceCollection) =

  services
    .BuildSingleton<IMongoCollection<Resource>, IMongoDatabase>(_.GetCollection("resources"))

    .BuildSingleton<ResourceRepo.LoadLangResources, _>(ResourceRepo.loadLangResources)
