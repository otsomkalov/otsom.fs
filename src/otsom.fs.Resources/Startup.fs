module otsom.fs.Resources.Startup

#nowarn "20"

open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Options
open otsom.fs.Resources.Settings
open otsom.fs.Extensions.DependencyInjection

let addResources (cfg: IConfiguration) (services: IServiceCollection) =
  services.Configure<ResourcesSettings>(cfg.GetSection(ResourcesSettings.SectionName))

  services
    .BuildSingleton<ResourcesSettings, IOptions<ResourcesSettings>>(_.Value)

    .BuildSingleton<LoadDefaultResources, _, _>(Workflows.loadDefaultResources)
    .BuildSingleton<LoadResources, _, _>(Workflows.loadResources)
