[<RequireQualifiedAccess>]
module otsom.fs.Extensions.Async

let inline map ([<InlineIfLambda>] mapper: 'a -> 'b) (task': Async<'a>) : Async<'b> =
  async {
    let! value = task'

    return mapper value
  }

let inline bind ([<InlineIfLambda>] binder: 'a -> Async<'b>) (task': Async<'a>) : Async<'b> =
  async {
    let! value = task'

    return! binder value
  }
