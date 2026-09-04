module List

open Xunit
open otsom.fs.Extensions

[<Fact>]
let ``takeSafe returns original list if count greater than length`` () =
  let list = [ 1; 2 ]

  let result = list |> List.takeSafe 3

  Assert.Equal<int list>(list, result)

[<Fact>]
let ``takeSafe returns original list if count equals length`` () =
  let list = [ 1; 2 ]

  let result = list |> List.takeSafe 2

  Assert.Equal<int list>(list, result)

[<Fact>]
let ``takeSafe returns correct list if count lower than length`` () =
  let list = [ 1; 2 ]

  let result = list |> List.takeSafe 1

  Assert.Equal<int list>([ 1 ], result)

[<Fact>]
let ``prepend prepends list2 to list1`` () =
  let list1 = [ 1; 2 ]
  let list2 = [ 3; 4 ]

  let result = List.prepend list1 list2

  Assert.Equal<int list>([ 3; 4; 1; 2 ], result)