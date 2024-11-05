module otsom.fs.Resources.Startup

open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Options
open otsom.fs.Resources.Repos
open otsom.fs.Resources.Settings
open otsom.fs.Extensions.DependencyInjection

let addResources (cfg: IConfiguration) (services: IServiceCollection) =
  services.Configure<ResourcesSettings>(cfg.GetSection(ResourcesSettings.SectionName))

  services
    .BuildSingleton<ResourcesSettings, IOptions<ResourcesSettings>>(_.Value)

    .BuildSingleton<LoadDefaultResources, ResourcesSettings, ResourceRepo.LoadLangResources>(Workflows.loadDefaultResources)
    .BuildSingleton<LoadResources, ResourcesSettings, ResourceRepo.LoadLangResources>(Workflows.loadResources)
