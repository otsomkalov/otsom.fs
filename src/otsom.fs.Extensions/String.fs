module otsom.fs.Extensions.String

open System

let (|StartsWith|_|) (substring: string) (str: string) =
  if str.StartsWith(substring, StringComparison.InvariantCultureIgnoreCase) then
    Some()
  else
    None

let (|Equals|_|) (toCompare: string) (source: string) =
  if String.Equals(source, toCompare, StringComparison.InvariantCultureIgnoreCase) then
    Some()
  else
    None
