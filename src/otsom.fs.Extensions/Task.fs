[<RequireQualifiedAccess>]
module otsom.fs.Extensions.Task

open System.Threading.Tasks

let inline map ([<InlineIfLambda>] mapper: 'a -> 'b) (task': Task<'a>) : Task<'b> =
  task {
    let! value = task'

    return mapper value
  }

let inline bind ([<InlineIfLambda>] binder: 'a -> Task<'b>) (task': Task<'a>) : Task<'b> =
  task {
    let! value = task'

    return! binder value
  }

let inline ignore (task': Task<'a>) : Task<unit> =
  task'
  |> map ignore

let inline tap (action: 'a -> unit) (task': Task<'a>) =
  task {
    let! v = task'

    action v

    return v
  }