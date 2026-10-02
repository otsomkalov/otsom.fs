namespace otsom.fs.OAuth.Keycloak

open System.Net.Http
open System.Net.Http.Json
open System.Runtime.CompilerServices
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Options
open otsom.fs.OAuth

type KeycloakOAuthSettings() =
  inherit OAuthSettingsBase()

  member val OpenIdConfigurationUri = Unchecked.defaultof<string> with get, set

  static member SectionName = "OAuth:Keycloak"

type internal KeycloakOAuthClient(repo: IOAuthRepo, options: IOptions<KeycloakOAuthSettings>, httpClient: HttpClient) =
  inherit OAuthClientBase(repo, options.Value :> OAuthSettingsBase, httpClient)

  let settings = options.Value

  let oidcConfigurationTask =
    httpClient.GetFromJsonAsync<OpenIdConfiguration>(settings.OpenIdConfigurationUri)

  override this.GetAuthorizationEndpoint() = task {
    let! oidcConfiguration = oidcConfigurationTask

    return oidcConfiguration.AuthorizationEndpoint
  }

  override this.PrepareCodeExchangeRequest(content) = task {
    let! oidcConfiguration = oidcConfigurationTask

    return new HttpRequestMessage(HttpMethod.Post, oidcConfiguration.TokenEndpoint, Content = content)
  }

type OAuthBuilderExtensions =
  [<Extension>]
  static member AddKeycloak(builder: IOAuthBuilder, configuration: IConfiguration) =
    configuration.GetRequiredSection(KeycloakOAuthSettings.SectionName)
    |> builder.Services.Configure<KeycloakOAuthSettings>
    |> ignore

    builder.Services.AddSingleton<IOAuthClient, KeycloakOAuthClient>() |> ignore

    builder