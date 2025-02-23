module otsom.fs.Auth.Settings

open System

[<CLIMutable>]
type AuthSettings =
  { ClientId: string
    ClientSecret: string
    CallbackUrl: Uri
    Scopes: string array }

  static member SectionName = "Auth"