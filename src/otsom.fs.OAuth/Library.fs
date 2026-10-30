namespace otsom.fs.OAuth

open System
open System.Collections.Generic
open System.Net.Http
open System.Net.Http.Json
open System.Runtime.CompilerServices
open System.Security.Cryptography
open System.Text
open System.Text.Json.Serialization
open System.Threading.Tasks
open Microsoft.AspNetCore.WebUtilities
open FsToolkit.ErrorHandling
open Microsoft.Extensions.DependencyInjection

module internal Helpers =
  [<Literal>]
  let internal chars =
    "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"

  let internal generateRandomString length =
    String.init length (fun _ -> string chars[RandomNumberGenerator.GetInt32(chars.Length)])

[<AbstractClass>]
type OAuthSettingsBase() =
  member val ClientId = Unchecked.defaultof<string> with get, set
  member val RedirectUri = Unchecked.defaultof<string> with get, set
  member val Scope = Unchecked.defaultof<string array> with get, set

type AccountId =
  | AccountId of string

  member this.Value = let (AccountId value) = this in value

type CompleteError = | RequestNotFound

type StateHash =
  | StateHash of string

  member this.Value = let (StateHash value) = this in value

type State =
  | State of string

  member this.Value = let (State state) = this in state

  member this.Hash =
    this.Value
    |> Encoding.UTF8.GetBytes
    |> SHA256.HashData
    |> WebEncoders.Base64UrlEncode
    |> StateHash

  static member Create() = State(Helpers.generateRandomString 32)

type PKCEVerifier =
  | PKCEVerifier of string

  member this.Value = let (PKCEVerifier challenge) = this in challenge

  static member Create() =
    PKCEVerifier(Helpers.generateRandomString 128)

type PKCEChallenge =
  static member Create(verifier: PKCEVerifier) =
    verifier.Value
    |> Encoding.UTF8.GetBytes
    |> SHA256.HashData
    |> WebEncoders.Base64UrlEncode

type OAuthProvider =
  | OAuthProvider of string

  member this.Value = let (OAuthProvider provider) = this in provider

type Inited =
  {
    AccountId: AccountId
    StateHash: StateHash
    Verifier: PKCEVerifier
    Provider: OAuthProvider
  }

type Code =
  | Code of string

  member this.Value = let (Code code) = this in code

type AccessToken = { ExpiresAt: DateTime; Value: string }

type RefreshToken =
  | RefreshToken of string

  member this.Value = let (RefreshToken token) = this in token

type Completed =
  {
    AccountId: AccountId
    AccessToken: AccessToken
    RefreshToken: RefreshToken option
    Provider: OAuthProvider
  }

type IInitAuth =
  abstract InitAuth: accountId: AccountId -> Task<string>

type ICompleteAuth =
  abstract CompleteAuth: state: State * code: Code -> TaskResult<Completed, CompleteError>

type IGetCompletedAuth =
  abstract GetCompleted: accountId: AccountId -> Task<Completed option>

type IOAuthClient =
  inherit IInitAuth
  inherit ICompleteAuth
  inherit IGetCompletedAuth

type IOAuthRepo =
  abstract SaveInited: Inited -> Task<unit>

  /// <summary>
  /// Retrieves and removes the stored authentication request data
  /// </summary>
  abstract PopInited: StateHash -> Task<Inited option>

  abstract SaveCompleted: Completed -> Task<unit>
  abstract LoadCompleted: AccountId * OAuthProvider -> Task<Completed option>

[<CLIMutable>]
type TokenResponse =
  {
    [<JsonPropertyName "access_token">]
    AccessToken: string

    [<JsonPropertyName "token_type">]
    TokenType: string

    [<JsonPropertyName "expires_in">]
    ExpiresIn: int

    [<JsonPropertyName "id_token">]
    IdToken: string | null

    [<JsonPropertyName "refresh_token">]
    RefreshToken: string | null
  }

[<CLIMutable>]
type OpenIdConfiguration =
  {
    [<JsonPropertyName "authorization_endpoint">]
    AuthorizationEndpoint: string

    [<JsonPropertyName "token_endpoint">]
    TokenEndpoint: string
  }

[<AbstractClass>]
type OAuthClientBase(repo: IOAuthRepo, settings: OAuthSettingsBase, httpClient: HttpClient) =
  let createAccessToken token expiresIn : AccessToken =
    {
      ExpiresAt = DateTime.UtcNow.AddSeconds expiresIn
      Value = token
    }

  let createRefreshToken refreshToken =
    refreshToken |> Option.ofObj |> Option.map RefreshToken

  abstract GetAuthorizationEndpoint: unit -> Task<string>
  abstract PrepareTokenRequest: HttpContent -> Task<HttpRequestMessage>
  abstract Provider: OAuthProvider

  member private this.SendTokenRequest(formData) = task {
    use content = new FormUrlEncodedContent(formData)

    use! request = this.PrepareTokenRequest content

    use! response = httpClient.SendAsync(request)

    response.EnsureSuccessStatusCode() |> ignore

    return! response.Content.ReadFromJsonAsync<TokenResponse>()
  }

  interface IOAuthClient with
    member this.InitAuth(accountId) = task {
      let state = State.Create()

      let initedAuth: Inited =
        {
          AccountId = accountId
          StateHash = state.Hash
          Verifier = PKCEVerifier.Create()
          Provider = this.Provider
        }

      let queryParams =
        [
          KeyValuePair("client_id", settings.ClientId)
          KeyValuePair("scope", settings.Scope |> String.concat " ")
          KeyValuePair("redirect_uri", settings.RedirectUri)
          KeyValuePair("state", state.Value)
          KeyValuePair("code_challenge", PKCEChallenge.Create(initedAuth.Verifier))
          KeyValuePair("response_type", "code")
          KeyValuePair("code_challenge_method", "S256")
        ]

      let! authorizationEndpoint = this.GetAuthorizationEndpoint()

      let redirectUri = QueryHelpers.AddQueryString(authorizationEndpoint, queryParams)

      do! repo.SaveInited initedAuth

      return redirectUri
    }

    member this.CompleteAuth(state, code) = taskResult {
      let! authRequest =
        repo.PopInited(state.Hash)
        |> TaskResult.requireSome CompleteError.RequestNotFound

      let formData =
        [
          KeyValuePair("grant_type", "authorization_code")
          KeyValuePair("client_id", settings.ClientId)
          KeyValuePair("code", code.Value)
          KeyValuePair("redirect_uri", settings.RedirectUri)
          KeyValuePair("code_verifier", authRequest.Verifier.Value)
        ]

      let! tokenResponse = this.SendTokenRequest(formData)

      let completedAuth: Completed =
        {
          AccountId = authRequest.AccountId
          AccessToken = createAccessToken tokenResponse.AccessToken tokenResponse.ExpiresIn
          RefreshToken = createRefreshToken tokenResponse.RefreshToken
          Provider = this.Provider
        }

      do! repo.SaveCompleted completedAuth

      return completedAuth
    }

    member this.GetCompleted(accountId) = taskOption {
      match! repo.LoadCompleted(accountId, this.Provider) with
      | { AccessToken = accessToken } as activeAuth when accessToken.ExpiresAt > DateTime.UtcNow -> return activeAuth
      | { RefreshToken = None } -> return! None
      | { RefreshToken = Some refreshToken } as expiredAuth ->
        let formData =
          [
            KeyValuePair("grant_type", "refresh_token")
            KeyValuePair("client_id", settings.ClientId)
            KeyValuePair("refresh_token", refreshToken.Value)
          ]

        let! refreshTokenResponse = this.SendTokenRequest(formData)

        let refreshedAuth =
          { expiredAuth with
              AccessToken = createAccessToken refreshTokenResponse.AccessToken refreshTokenResponse.ExpiresIn
              RefreshToken =
                createRefreshToken refreshTokenResponse.RefreshToken
                |> Option.orElse (Some refreshToken)
          }

        do! repo.SaveCompleted refreshedAuth

        return refreshedAuth
    }

type IOAuthBuilder =
  abstract Services: IServiceCollection

type internal OAuthBuilder(services: IServiceCollection) =
  interface IOAuthBuilder with
    member this.Services = services

type ServiceCollectionExtensions =
  [<Extension>]
  static member AddOAuth(services: IServiceCollection) =
    services.AddHttpClient<OAuthClientBase>() |> ignore

    OAuthBuilder(services) :> IOAuthBuilder