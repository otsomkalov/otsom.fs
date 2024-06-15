[<RequireQualifiedAccess>]
module otsom.fs.Extensions.List

let takeSafe count list =
  if (list |> List.length) < count then
    list
  else
    list |> List.take count

let prepend list1 list2 = list2 @ list1
