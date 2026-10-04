namespace otsom.fs.OAuth.Spotify

open System.Net.Http
open System.Runtime.CompilerServices
open FsToolkit.ErrorHandling
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Options
open otsom.fs.OAuth

type SpotifyOAuthSettings() =
  inherit OAuthSettingsBase()

  member val AuthorizationEndpoint = Unchecked.defaultof<string> with get, set
  member val TokenEndpoint = Unchecked.defaultof<string> with get, set

  static member SectionName = "OAuth:Spotify"

type internal SpotifyOAuthClient(repo: IOAuthRepo, options: IOptions<SpotifyOAuthSettings>, httpClient: HttpClient) =
  inherit OAuthClientBase(repo, options.Value :> OAuthSettingsBase, httpClient)

  let settings = options.Value

  override this.GetAuthorizationEndpoint() =
    Task.singleton settings.AuthorizationEndpoint

  override this.PrepareCodeExchangeRequest(content) =
    Task.singleton (new HttpRequestMessage(HttpMethod.Post, settings.TokenEndpoint, Content = content))

  override this.Provider = OAuthProvider "Spotify"

type OAuthBuilderExtensions =
  [<Extension>]
  static member AddSpotify(builder: IOAuthBuilder, configuration: IConfiguration) =
    configuration.GetRequiredSection(SpotifyOAuthSettings.SectionName)
    |> builder.Services.Configure<SpotifyOAuthSettings>
    |> ignore

    builder.Services.AddSingleton<IOAuthClient, SpotifyOAuthClient>() |> ignore

    builder