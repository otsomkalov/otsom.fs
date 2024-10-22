module otsom.fs.Extensions.Tests.Operators

open System.Threading.Tasks
open Xunit
open otsom.fs.Extensions.Operators
open FsUnit.Xunit

let private task' = 5 |> Task.FromResult

[<Fact>]
let ``(!|>) works`` () =
  task {
    let! result = task' &|> (fun x -> x + 5)

    result |> should equal 10
  }

[<Fact>]
let ``(&|>&) works`` () =
  task {
    let! result = task' &|&> (fun x -> Task.FromResult(x + 5))

    result |> should equal 10
  }

let private result' : Result<int, unit> = Ok 5

[<Fact>]
let ``(=|>) works`` () =
  let result = result' =|> (fun x -> x + 5)

  result |> should equal (Result<int, unit>.Ok 10)


[<Fact>]
let ``(=|&>) works`` () =
  task {
    let! result = result' =|&> (fun x -> (x + 5) |> Task.FromResult)

    result |> should equal (Result<int, unit>.Ok 10)
  }

let private taskResult = result' |> Task.FromResult

[<Fact>]
let ``(&=|>) works`` () =
  task {
    let! result = taskResult &=|> (fun x -> (x + 5))

    result |> should equal (Result<int, unit>.Ok 10)
  }

[<Fact>]
let ``(&=|&>)``() =
  task {
    let! result = taskResult &=|&> (fun x -> (x + 5) |> Task.FromResult)

    result |> should equal (Result<int, unit>.Ok 10)
  }