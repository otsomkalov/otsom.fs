[<RequireQualifiedAccess>]
module otsom.fs.Extensions.Async

open System.Diagnostics

[<StackTraceHidden>]
let inline map ([<InlineIfLambda>] mapper: 'a -> 'b) (task': Async<'a>) : Async<'b> = async {
  let! value = task'

  return mapper value
}

[<StackTraceHidden>]
let inline bind ([<InlineIfLambda>] binder: 'a -> Async<'b>) (task': Async<'a>) : Async<'b> = async {
  let! value = task'

  return! binder value
}