[<RequireQualifiedAccess>]
module otsom.fs.Extensions.TaskOption

open System.Diagnostics
open System.Threading.Tasks

[<StackTraceHidden>]
let inline map ([<InlineIfLambda>] mapping: 'a -> 'b) = Task.map (Option.map mapping)

[<StackTraceHidden>]
let inline taskMap ([<InlineIfLambda>] mapping: 'a -> Task<'b>) = Task.bind (Option.taskMap mapping)

[<StackTraceHidden>]
let inline tap ([<InlineIfLambda>] effect: 'a -> unit) (taskOption: Task<'a option>) : Task<'a option> =
  taskOption |> Task.map (Option.tap effect)

[<StackTraceHidden>]
let inline taskBind ([<InlineIfLambda>] binder: 'a -> Task<'b option>) (opt: Task<'a option>) : Task<'b option> = task {
  let! value = opt

  match value with
  | None -> return None
  | Some v -> return! binder v
}