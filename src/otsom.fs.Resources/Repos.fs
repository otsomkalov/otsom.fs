namespace otsom.fs.Resources

open System.Threading.Tasks

type Resource =
  { Key: string
    Value: string
    Lang: string }

type IResourceRepo =
  abstract LoadResources: string -> Task<Resource seq>