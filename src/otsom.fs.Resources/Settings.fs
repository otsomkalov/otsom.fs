module otsom.fs.Resources.Settings

[<CLIMutable>]
type ResourcesSettings =
  { DefaultLang: string }

  static member SectionName = "Resources"
