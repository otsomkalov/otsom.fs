namespace otsom.fs.Resources.Mongo

open FsToolkit.ErrorHandling
open MongoDB.Driver
open otsom.fs.Resources

type ResourceRepo(collection: IMongoCollection<Resource>) =
  interface IResourceRepo with
    member this.LoadResources(lang) =
      let filter = Builders<Resource>.Filter.Eq(_.Lang, lang)

      collection.Find(filter).ToListAsync() |> Task.map seq