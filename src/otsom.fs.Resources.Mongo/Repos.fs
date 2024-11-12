module otsom.fs.Resources.Mongo.Repos

open MongoDB.Driver
open otsom.fs.Resources.Repos
open MongoDB.Driver.Linq
open otsom.fs.Extensions.Operators

module ResourceRepo =
  let loadLangResources (collection: IMongoCollection<Resource>) : ResourceRepo.LoadLangResources =
    fun lang -> collection.AsQueryable().Where(fun r -> r.Lang = lang).ToListAsync() &|> seq
