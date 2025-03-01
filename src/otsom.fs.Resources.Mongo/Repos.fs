namespace otsom.fs.Resources.Mongo

open MongoDB.Driver
open otsom.fs.Resources
open MongoDB.Driver.Linq
open otsom.fs.Extensions.Operators

type ResourceRepo(collection: IMongoCollection<Resource>) =
  interface IResourceRepo with
    member this.LoadResources(lang) =
      collection.AsQueryable().Where(fun r -> r.Lang = lang).ToListAsync() &|> seq