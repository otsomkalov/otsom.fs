namespace otsom.fs.Resources

open System.Threading.Tasks
open Microsoft.FSharp.Core

type GetResource = string -> string
type FormatResource = string -> obj seq -> string

type LoadDefaultResources = unit -> Task<GetResource * FormatResource>
type LoadResources = string -> Task<GetResource * FormatResource>