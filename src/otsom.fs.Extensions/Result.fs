module otsom.fs.Extensions.Result

open System.Diagnostics
open System.Threading.Tasks

[<StackTraceHidden>]
let ofOption error option =
  match option with
  | Some value -> Ok value
  | None -> Error error

[<StackTraceHidden>]
let inline taskMap ([<InlineIfLambda>] mappingTask) result =
  match result with
  | Ok v -> mappingTask v |> Task.map Ok
  | Error e -> Error e |> Task.FromResult

[<StackTraceHidden>]
let inline taskBind ([<InlineIfLambda>] binder) result =
  match result with
  | Error e -> Error e |> Task.FromResult
  | Ok x -> binder x

[<StackTraceHidden>]
let inline either
  ([<InlineIfLambda>] onOk: 'okInput -> 'output)
  ([<InlineIfLambda>] onError: 'errorInput -> 'output)
  (input: Result<'okInput, 'errorInput>)
  : 'output =
  match input with
  | Ok x -> onOk x
  | Error err -> onError err

[<StackTraceHidden>]
let inline tap ([<InlineIfLambda>] action: 'okInput -> unit) (input: Result<'okInput, 'errorInput>) : Result<'okInput, 'errorInput> =
  match input with
  | Ok x ->
    action x
    Ok x
  | Error err -> Error err

[<StackTraceHidden>]
let inline taskTap
  ([<InlineIfLambda>] action: 'okInput -> Task<unit>)
  (input: Result<'okInput, 'errorInput>)
  : Task<Result<'okInput, 'errorInput>> =
  task {
    match input with
    | Ok x ->
      do! action x
      return Ok x
    | Error err -> return Error err
  }
