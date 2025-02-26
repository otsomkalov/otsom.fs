namespace otsom.fs.Resources

open otsom.fs.Resources
open System

type DefaultResourceProvider(resources: Resource seq) =
  let resources = Helpers.createResourcesMap resources

  interface IResourceProvider with
    member this.Item
      with get (key: string): string = resources |> Map.tryFind key |> Option.defaultValue key

    member this.Item
      with get (key: string, [<OptionalArgument>] args: obj array): string =
        resources
        |> Map.tryFind key
        |> Option.map (fun r -> String.Format(r, args))
        |> Option.defaultValue key

type ResourceProvider(defaultLocalizer: IResourceProvider, resources: Resource seq) =
  let resources = Helpers.createResourcesMap resources

  interface IResourceProvider with
    member this.Item
      with get (key: string): string = resources |> Map.tryFind key |> Option.defaultValue (defaultLocalizer[key])

    member this.Item
      with get (key: String, [<OptionalArgument>] args: obj array): string =
        resources
        |> Map.tryFind key
        |> Option.map (fun r -> String.Format(r, args))
        |> Option.defaultValue (defaultLocalizer[key, args])