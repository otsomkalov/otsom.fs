[<RequireQualifiedAccess>]
module otsom.fs.Extensions.TaskResult

open System.Diagnostics
open System.Threading.Tasks

let ok v = Ok v |> Task.FromResult

let error e = Error e |> Task.FromResult

[<StackTraceHidden>]
let inline bind ([<InlineIfLambda>] binder: 'a -> Task<Result<'b, 'e>>) (taskResult: Task<Result<'a, 'e>>) : Task<Result<'b, 'e>> =
  taskResult |> Task.bind (Result.taskBind binder)

[<StackTraceHidden>]
let inline map ([<InlineIfLambda>] mapping) taskResult =
  taskResult |> Task.map (Result.map mapping)

[<StackTraceHidden>]
let inline taskMap ([<InlineIfLambda>] mapping) taskResult =
  taskResult |> Task.bind (Result.taskMap mapping)

[<StackTraceHidden>]
let inline mapError ([<InlineIfLambda>] mapping) taskResult =
  taskResult |> Task.map (Result.mapError mapping)

[<StackTraceHidden>]
let inline either ([<InlineIfLambda>] onOk) ([<InlineIfLambda>] onError) taskResult =
  taskResult |> Task.map (Result.either onOk onError)

[<StackTraceHidden>]
let inline taskEither ([<InlineIfLambda>] onOk: 'okInput -> Task<'output>) ([<InlineIfLambda>] onError: 'errorInput -> Task<'output>) =
  Task.bind (Result.either onOk onError)

[<StackTraceHidden>]
let inline taskTap ([<InlineIfLambda>] effect: 'ok -> Task<unit>) (taskResult: Task<Result<'ok, 'err>>) : Task<Result<'ok, 'err>> =
  taskResult |> Task.bind (Result.taskTap effect)