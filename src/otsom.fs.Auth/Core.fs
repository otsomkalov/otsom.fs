namespace otsom.fs.Auth

open System
open System.Threading.Tasks
open shortid

type AccountId =
  | AccountId of string

  member this.Value = let (AccountId value) = this in value

type FulfillmentError = | StateNotFound

type CompleteError =
  | StateNotFound
  | StateDoesntBelongToUser

type State =
  private
  | State of string

  member this.Value = let (State state) = this in state
  static member Create() = State(ShortId.Generate())
  static member Parse str = State str

type Inited = { AccountId: AccountId; State: State }

type Code =
  | Code of string

  member this.Value = let (Code code) = this in code

type RefreshToken =
  | RefreshToken of string

  member this.Value = let (RefreshToken token) = this in token

type Fulfilled =
  { AccountId: AccountId
    State: State
    Code: Code }

type Completed = { AccountId: AccountId; Token: RefreshToken }

type IInitAuth =
  abstract InitAuth: accountId: AccountId -> Task<Uri>

type IFulfillAuth =
  abstract FulfillAuth: state: State * code: Code -> Task<Result<string, FulfillmentError>>

type ICompleteAuth =
  abstract CompleteAuth: accountId: AccountId * state: State -> Task<Result<unit, CompleteError>>

type IAuthService =
  inherit IInitAuth
  inherit IFulfillAuth
  inherit ICompleteAuth