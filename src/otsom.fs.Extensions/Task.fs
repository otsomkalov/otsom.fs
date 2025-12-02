[<RequireQualifiedAccess>]
module otsom.fs.Extensions.Task

open System.Diagnostics
open System.Threading.Tasks

[<StackTraceHidden>]
let inline tap ([<InlineIfLambda>] action: 'a -> unit) (task': Task<'a>) = task {
  let! v = task'

  action v

  return v
}

[<StackTraceHidden>]
let inline taskTap ([<InlineIfLambda>] action: 'a -> Task<unit>) (task': Task<'a>) = task {
  let! v = task'

  do! action v

  return v
}