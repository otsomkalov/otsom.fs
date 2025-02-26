namespace otsom.fs.Resources

#nowarn "20"

open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Options
open otsom.fs.Extensions.DependencyInjection

[<RequireQualifiedAccess>]
module Startup =
  let createDefaultResourceProvider (options: IOptions<ResourcesSettings>) (repo: IResourceRepo) : CreateDefaultResourceProvider =
    let settings = options.Value

    fun () -> task {
      let! resourcesMap = repo.LoadResources settings.DefaultLang

      return DefaultResourceProvider(resourcesMap) :> IResourceProvider
    }

  let createResourceProvider (createDefaultResourceProvider: CreateDefaultResourceProvider) (repo: IResourceRepo) : CreateResourceProvider =
    fun lang -> task {
      let! defaultResourceProvider = createDefaultResourceProvider ()
      let! resourcesMap = repo.LoadResources lang

      return ResourceProvider(defaultResourceProvider, resourcesMap) :> IResourceProvider
    }

  let addResources (cfg: IConfiguration) (services: IServiceCollection) =
    services.Configure<ResourcesSettings>(cfg.GetSection(ResourcesSettings.SectionName))

    services
      .BuildSingleton<ResourcesSettings, IOptions<ResourcesSettings>>(_.Value)

      .BuildSingleton<CreateDefaultResourceProvider, _, _>(createDefaultResourceProvider)
      .BuildSingleton<CreateResourceProvider, _, _>(createResourceProvider)