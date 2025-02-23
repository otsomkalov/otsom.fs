module otsom.fs.Auth.Repo

open System
open System.Threading.Tasks

type ILoadInitedAuth =
  abstract LoadInitedAuth: state: State -> Task<Inited option>

type ISaveInitedAuth =
  abstract SaveInitedAuth: auth: Inited -> Task<unit>

type IGetAuthUrl =
  abstract GetAuthUrl: state: State -> Uri

type ISaveFulfilledAuth =
  abstract SaveFulfilledAuth: auth: Fulfilled -> Task<unit>

type IGetToken =
  abstract GetRefreshToken: code: Code -> Task<RefreshToken>

type ILoadFulfilledAuth =
  abstract LoadFulfilledAuth: State -> Task<Fulfilled option>

type ISaveCompletedAuth =
  abstract SaveCompletedAuth: auth: Completed -> Task<unit>

type ILoadCompletedAuth =
  abstract LoadCompletedAuth: accountId: AccountId -> Task<Completed option>

type IAuthRepo =
  inherit ILoadInitedAuth
  inherit ISaveInitedAuth
  inherit IGetAuthUrl

  inherit ISaveFulfilledAuth
  inherit ILoadFulfilledAuth

  inherit ISaveCompletedAuth
  inherit ILoadCompletedAuth

  inherit IGetToken