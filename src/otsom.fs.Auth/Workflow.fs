module otsom.fs.Auth.Workflow

open System.Threading.Tasks
open otsom.fs.Auth.Repo
open otsom.fs.Extensions
open FsToolkit.ErrorHandling

type AuthService(authRepo: IAuthRepo) =
  interface IAuthService with
    member this.InitAuth(accountId) = task {
      let initedAuth: Inited =
        { AccountId = accountId
          State = State.Create() }

      do! authRepo.SaveInitedAuth initedAuth

      return authRepo.GetAuthUrl(initedAuth.State)
    }

    member this.FulfillAuth(state, code) = task {
      let addCodeToAuth code =
        fun (initedAuth: Inited option) ->
          match initedAuth with
          | Some auth -> task {
              let fulfilledAuth: Fulfilled =
                { State = auth.State
                  AccountId = auth.AccountId
                  Code = code }

              do! authRepo.SaveFulfilledAuth fulfilledAuth

              return (auth.State.Value |> Ok)
            }
          | None -> FulfillmentError.StateNotFound |> Error |> Task.FromResult

      return! state |> authRepo.LoadInitedAuth |> Task.bind (addCodeToAuth code)
    }

    member this.CompleteAuth(accountId, state) =
      let createCompletedAuth (auth: Fulfilled) = task {

        let! refreshToken = authRepo.GetRefreshToken auth.Code

        let completed =
          { AccountId = auth.AccountId
            Token = refreshToken }

        do! authRepo.SaveCompletedAuth completed
      }

      let validateAuth accountId =
        fun (auth: Fulfilled) ->
          if accountId = auth.AccountId then
            Ok(auth)
          else
            Error(StateDoesntBelongToUser)

      authRepo.LoadFulfilledAuth state
      |> Task.map (Result.ofOption CompleteError.StateNotFound)
      |> Task.map (Result.bind (validateAuth accountId))
      |> Task.bind (Result.taskMap createCompletedAuth)