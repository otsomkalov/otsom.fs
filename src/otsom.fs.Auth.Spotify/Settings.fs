module otsom.fs.Auth.Spotify.Settings

[<CLIMutable>]
type StorageSettings =
  { ConnectionString: string
    Container: string }

  static member SectionName = "Storage"