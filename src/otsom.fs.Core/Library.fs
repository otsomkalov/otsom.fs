namespace otsom.fs.Core

type UserId = UserId of int64

[<RequireQualifiedAccess>]
module UserId =
  let value (UserId id) = id