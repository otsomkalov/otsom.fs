module Option

open System
open Xunit
open otsom.fs.Extensions
open System.Threading.Tasks

[<Fact>]
let ``defaultWithTask return value if Option is Some`` () = task {
  // Assert

  let value = Some 42

  // Act

  let! result =
    value |> Option.defaultWithTask (fun _ -> raise (NotImplementedException()))

  // Assert

  Assert.Equal(42, result)
}

[<Fact>]
let ``defaultWithTask executes defThunkTask if option is None`` () = task {
  // Assert

  let value = None

  // Act

  let! result = value |> Option.defaultWithTask (fun _ -> 42 |> Task.FromResult)

  // Assert

  Assert.Equal(42, result)
}

[<Fact>]
let ``someIf returns Some if predicate is true`` () =
  let result = 42 |> Option.someIf ((=) 42)

  Assert.Equal(Some 42, result)

[<Fact>]
let ``someIf returns None if predicate is false`` () =
  let result = 42 |> Option.someIf ((=) 42 >> not)

  Assert.Equal(None, result)

[<Fact>]
let ``noneIf returns None if predicate is true`` () =
  let result = 42 |> Option.noneIf ((=) 42)

  Assert.Equal(None, result)

[<Fact>]
let ``noneIf returns None if predicate is false`` () =
  let result = 42 |> Option.noneIf ((=) 42 >> not)

  Assert.Equal(Some 42, result)