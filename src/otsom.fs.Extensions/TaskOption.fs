[<RequireQualifiedAccess>]
module otsom.fs.Extensions.TaskOption

open System.Threading.Tasks

let inline map ([<InlineIfLambda>] mapping: 'a -> 'b) = Task.map (Option.map mapping)

let inline taskMap ([<InlineIfLambda>] mapping: 'a -> Task<'b>) = Task.bind (Option.taskMap mapping)

let inline tap ([<InlineIfLambda>] effect: 'a -> unit) (taskOption: Task<'a option>) : Task<'a option> =
  taskOption |> Task.map (Option.tap effect)
