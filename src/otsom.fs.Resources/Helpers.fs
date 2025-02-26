namespace otsom.fs.Resources

[<RequireQualifiedAccess>]
module internal Helpers =
  let createResourcesMap (resources: Resource seq) =
    resources
    |> Seq.groupBy _.Key
    |> Seq.map (fun (key, translations) -> (key, translations |> Seq.map _.Value |> Seq.head))
    |> Map.ofSeq