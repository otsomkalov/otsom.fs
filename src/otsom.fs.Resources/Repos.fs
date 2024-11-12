module otsom.fs.Resources.Repos

open System.Threading.Tasks

type Resource =
  { Id: string
    Key: string
    Value: string
    Lang: string }

module ResourceRepo =
  type LoadLangResources = string -> Task<Resource seq>