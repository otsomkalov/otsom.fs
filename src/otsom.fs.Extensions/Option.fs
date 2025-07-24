[<RequireQualifiedAccess>]
module otsom.fs.Extensions.Option

open System.Diagnostics
open System.Threading.Tasks

[<StackTraceHidden>]
let inline taskMap ([<InlineIfLambda>] mapping: 'a -> Task<'b>) option =
  match option with
  | Some v -> mapping v |> Task.map Some
  | None -> None |> Task.FromResult

[<StackTraceHidden>]
let inline defaultWithTask ([<InlineIfLambda>] defThunkTask: unit -> Task<'a>) option : Task<'a> =
  match option with
  | Some v -> v |> Task.FromResult
  | None -> defThunkTask ()

[<StackTraceHidden>]
let inline tap ([<InlineIfLambda>] effect: 'a -> unit) (option: 'a option) : 'a option =
  match option with
  | Some v ->
    do effect v
    option
  | None -> option

[<StackTraceHidden>]
let inline someIf ([<InlineIfLambda>] predicate: 'a -> bool) (value: 'a) =
  if predicate value then Some value else None

[<StackTraceHidden>]
let inline noneIf ([<InlineIfLambda>] predicate: 'a -> bool) (value: 'a) =
  if predicate value then None else Some value