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
