module internal otsom.fs.Resources.Workflows

open otsom.fs.Resources.Repos
open otsom.fs.Extensions.Operators
open System
open otsom.fs.Resources.Settings

let private createResourcesMap (resources: Resource seq) =
  resources
  |> Seq.groupBy (_.Key)
  |> Seq.map (fun (key, translations) -> (key, translations |> Seq.map (_.Value) |> Seq.head))
  |> Map.ofSeq

let loadDefaultResources (settings: ResourcesSettings) (loadResources: ResourceRepo.LoadLangResources) : LoadDefaultResources =
  fun () -> task {
    let! resourcesMap = loadResources settings.DefaultLang &|> createResourcesMap

    let getResource: GetResource =
      fun key -> resourcesMap |> Map.tryFind key |> Option.defaultValue key

    let formatResource: FormatResource =
      fun key args ->
        resourcesMap
        |> Map.tryFind key
        |> Option.map (fun r -> String.Format(r, args))
        |> Option.defaultValue key

    return getResource, formatResource
  }

let loadResources (settings: ResourcesSettings) (loadResources: ResourceRepo.LoadLangResources) : LoadResources =
  fun lang -> task {
    let! defaultResourcesMap = loadResources settings.DefaultLang &|> createResourcesMap
    let! resourcesMap = loadResources lang &|> createResourcesMap

    let getResource: GetResource =
      fun key ->
        resourcesMap
        |> Map.tryFind key
        |> Option.defaultValue (defaultResourcesMap |> Map.tryFind key |> Option.defaultValue key)

    let formatResource: FormatResource =
      fun key args ->
        resourcesMap
        |> Map.tryFind key
        |> Option.map (fun r -> String.Format(r, args))
        |> Option.defaultValue (
          defaultResourcesMap
          |> Map.tryFind key
          |> Option.map (fun r -> String.Format(r, args))
          |> Option.defaultValue key
        )

    return getResource, formatResource
  }
