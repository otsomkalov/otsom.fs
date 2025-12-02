[<RequireQualifiedAccess>]
module otsom.fs.Extensions.TaskOption

open System.Diagnostics
open System.Threading.Tasks
open FsToolkit.ErrorHandling

[<StackTraceHidden>]
let inline taskMap ([<InlineIfLambda>] mapping: 'a -> Task<'b>) = Task.bind (Option.taskMap mapping)

[<StackTraceHidden>]
let inline tap ([<InlineIfLambda>] effect: 'a -> unit) (taskOption: Task<'a option>) : Task<'a option> =
  taskOption |> Task.map (Option.tap effect)