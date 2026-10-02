namespace otsom.fs.OAuth.Telegram

open System
open System.Net.Http
open System.Net.Http.Headers
open System.Net.Http.Json
open System.Runtime.CompilerServices
open System.Text
open FsToolkit.ErrorHandling
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Options
open otsom.fs.OAuth

type TelegramOAuthSettings() =
  inherit OAuthSettingsBase()

  member val ClientSecret = Unchecked.defaultof<string> with get, set
  member val OpenIdConfigurationUri = Unchecked.defaultof<string> with get, set

  static member SectionName = "OAuth:Telegram"

type internal TelegramOAuthClient(httpClient: HttpClient, options: IOptions<TelegramOAuthSettings>, repo: IOAuthRepo) =
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

    let request =
      new HttpRequestMessage(HttpMethod.Post, oidcConfiguration.TokenEndpoint, Content = content)

    let authorizationHeaderValue =
      $"{settings.ClientId}:{settings.ClientSecret}"
      |> Encoding.UTF8.GetBytes
      |> Convert.ToBase64String

    request.Headers.Authorization <- AuthenticationHeaderValue("Basic", authorizationHeaderValue)

    return request
  }

type OAuthBuilderExtensions =
  [<Extension>]
  static member AddTelegram(builder: IOAuthBuilder, configuration: IConfiguration) =
    configuration.GetRequiredSection(TelegramOAuthSettings.SectionName)
    |> builder.Services.Configure<TelegramOAuthSettings>
    |> ignore

    builder.Services.AddSingleton<IOAuthClient, TelegramOAuthClient>() |> ignore

    builder