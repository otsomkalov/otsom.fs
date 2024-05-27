module List

open Xunit
open otsom.fs.Extensions
open FsUnit.Xunit

[<Fact>]
let ``takeSafe returns original list if count greater than length`` () =
  let list = [ 1; 2 ]

  let result = list |> List.takeSafe 3

  result |> should equal list

[<Fact>]
let ``takeSafe returns original list if count equals length`` () =
  let list = [ 1; 2 ]

  let result = list |> List.takeSafe 2

  result |> should equal list

[<Fact>]
let ``takeSafe returns correct list if count lower than length`` () =
  let list = [ 1; 2 ]

  let result = list |> List.takeSafe 1

  result |> should equal [ 1 ]

[<Fact>]
let ``prepend prepends list2 to list1`` () =
  let list1 = [ 1; 2 ]
  let list2 = [ 3; 4 ]

  let result = List.prepend list1 list2

  result
  |> should equal [ 3; 4; 1; 2 ]