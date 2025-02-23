namespace otsom.fs.Core

type UserId =
  | UserId of string

  member this.Value = let (UserId id) = this in id