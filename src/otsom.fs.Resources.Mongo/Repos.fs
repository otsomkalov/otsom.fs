namespace otsom.fs.Resources.Mongo

open MongoDB.Driver
open otsom.fs.Resources
open otsom.fs.Extensions.Operators

type ResourceRepo(collection: IMongoCollection<Resource>) =
  interface IResourceRepo with
    member this.LoadResources(lang) =
      let filter = Builders<Resource>.Filter.Eq(_.Lang, lang)

      collection.Find(filter).ToListAsync() &|> seq