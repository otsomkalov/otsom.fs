[<RequireQualifiedAccess>]
module otsom.fs.Extensions.TaskResult

open System.Threading.Tasks

let ok v = Ok v |> Task.FromResult

let error e = Error e |> Task.FromResult

let bind binder taskResult =
  taskResult |> Task.map (Result.bind binder)

let map mapping taskResult =
  taskResult |> Task.map (Result.map mapping)

let taskMap mapping taskResult =
  taskResult |> Task.bind (Result.taskMap mapping)

let mapError mapping taskResult =
  taskResult |> Task.map (Result.mapError mapping)

let either onOk onError taskResult =
  taskResult |> Task.map (Result.either onOk onError)

let inline taskEither
  ([<InlineIfLambda>] onOk: 'okInput -> Task<'output>)
  ([<InlineIfLambda>] onError: 'errorInput -> Task<'output>)
  =
  Task.bind (Result.either onOk onError)