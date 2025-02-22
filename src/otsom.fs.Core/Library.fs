namespace otsom.fs.Core

type UserId = UserId of string

[<RequireQualifiedAccess>]
module UserId =
  let value (UserId id) = id