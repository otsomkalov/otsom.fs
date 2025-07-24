namespace otsom.fs.Resources

[<CLIMutable>]
type ResourcesSettings =
  { DefaultLang: string }

  static member SectionName = "Resources"