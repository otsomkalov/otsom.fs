namespace otsom.fs.Resources

open System.Threading.Tasks
open Microsoft.FSharp.Core

type IResourceProvider =
  abstract Item: key: string -> string with get
  abstract Item: key: string * [<OptionalArgument>] args: obj array -> string with get

type CreateDefaultResourceProvider = unit -> Task<IResourceProvider>
type CreateResourceProvider = string -> Task<IResourceProvider>