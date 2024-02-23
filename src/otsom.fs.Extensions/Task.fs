[<RequireQualifiedAccess>]
module otsom.fs.Extensions.Task

open System.Threading.Tasks

let map (mapper: 'a -> 'b) (task': Task<'a>) : Task<'b> =
  task {
    let! value = task'

    return mapper value
  }

let bind (binder: 'a -> Task<'b>) (task': Task<'a>) : Task<'b> =
  task {
    let! value = task'

    return! binder value
  }
