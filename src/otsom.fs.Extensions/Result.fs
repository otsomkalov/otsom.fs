module otsom.fs.Extensions.Result

open System.Threading.Tasks

let ofOption error option =
  match option with
  | Some value -> Ok value
  | None -> Error error

let taskMap mappingTask result =
  match result with
  | Ok v -> mappingTask v |> Task.map Ok
  | Error e -> Error e |> Task.FromResult

let taskBind binder result =
  match result with
  | Error e -> Error e |> Task.FromResult
  | Ok x -> binder x

let inline either
  ([<InlineIfLambda>] onOk: 'okInput -> 'output)
  ([<InlineIfLambda>] onError: 'errorInput -> 'output)
  (input: Result<'okInput, 'errorInput>)
  : 'output =
  match input with
  | Ok x -> onOk x
  | Error err -> onError err
