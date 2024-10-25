[<RequireQualifiedAccess>]
module otsom.fs.Extensions.Option

open System.Threading.Tasks

let taskMap (mapping: 'a -> Task<'b>) option =
  match option with
  | Some v -> mapping v |> Task.map Some
  | None -> None |> Task.FromResult

let defaultWithTask (defThunkTask: unit -> Task<'a>) option : Task<'a> =
  match option with
  | Some v -> v |> Task.FromResult
  | None -> defThunkTask ()

let inline tap (effect: 'a -> unit) (option: 'a option) : 'a option =
  match option with
  | Some v ->
    do effect v
    option
  | None -> option

let inline someIf (predicate: 'a -> bool) (value: 'a) =
  if predicate value then Some value else None

let inline noneIf (predicate: 'a -> bool) (value: 'a) =
  if predicate value then None else Some value
