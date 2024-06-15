module Option

open System
open Xunit
open otsom.fs.Extensions
open FsUnit.Xunit
open System.Threading.Tasks

[<Fact>]
let ``defaultWithTask return value if Option is Some``() =
  task{
    // Assert

    let value = Some 42

    // Act

    let! result = value |> Option.defaultWithTask (fun _ -> raise (NotImplementedException()))

    // Assert

    result |> should equal 42
  }

[<Fact>]
let ``defaultWithTask executes defThunkTask if option is None``() =
  task{
    // Assert

    let value = None

    // Act

    let! result = value |> Option.defaultWithTask (fun _ -> 42 |> Task.FromResult)

    // Assert

    result |> should equal 42
  }