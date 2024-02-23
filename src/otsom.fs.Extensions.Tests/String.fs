module String

open Xunit
open otsom.fs.Extensions.String
open FsUnit.Xunit

[<Fact>]
let ``StartsWith return correct result for strings in different case``() =
  let first = "QwEr"
  let second = "qWe"

  let result =
    match first with
    | StartsWith second -> true
    | _ -> false

  result |> should equal true

[<Fact>]
let ``Equals return correct result for strings in different case``() =
  let first = "QwE"
  let second = "qWe"

  let result =
    match first with
    | Equals second -> true
    | _ -> false

  result |> should equal true