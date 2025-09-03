module Operators

open System.Threading.Tasks
open Xunit
open otsom.fs.Extensions.Operators
open FsUnit.Xunit

let private task' = 5 |> Task.FromResult

[<Fact>]
let ``(&|>) works`` () =
  task {
    let! result = task' &|> (fun x -> x + 5)

    result |> should equal 10
  }

[<Fact>]
let ``(&|!) works`` () =
  task {
    let mutable result1 = 5

    let! result2 = (task' &|! (fun x -> result1 <- result1 + 5))

    result1 |> should equal 10
    result2 |> should equal 5
  }

[<Fact>]
let ``(&|&>) works`` () =
  task {
    let! result = task' &|&> (fun x -> Task.FromResult(x + 5))

    result |> should equal 10
  }

let private result': Result<int, unit> = Ok 5

[<Fact>]
let ``(=|>) works`` () =
  let result = result' =|> (fun x -> x + 5)

  result |> should equal (Result<int, unit>.Ok 10)


[<Fact>]
let ``(=|=>) works`` () =
  let result = result' =|=> (fun x -> Ok(x + 5))

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
let ``(&=|&>) works`` () =
  task {
    let! result = taskResult &=|&> (fun x -> (x + 5) |> Task.FromResult)

    result |> should equal (Result<int, unit>.Ok 10)
  }

[<Fact>]
let ``(&=|&!) works`` () =
  task {
    let mutable result1 = 5

    let! result2 = taskResult &=|&! (fun x -> task { result1 <- result1 + 5 })

    result1 |> should equal 10
    result2 |> should equal (Result<int, unit>.Ok 5)
  }
