module otsom.fs.Auth.Startup

open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open otsom.fs.Auth.Settings
open otsom.fs.Auth.Workflow


#nowarn "20"

let addAuthCore (cfg: IConfiguration) (services: IServiceCollection) =
  services.Configure<AuthSettings>(cfg.GetSection AuthSettings.SectionName)

  services.AddSingleton<IAuthService, AuthService>()

  services