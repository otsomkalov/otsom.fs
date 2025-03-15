namespace otsom.fs.Bot.Dialog

open System.Threading.Tasks
open otsom.fs.Bot

type DialogId =
  | DialogId of string

  member this.Value = let (DialogId id) = this in id

type DialogStepId =
  | DialogStepId of string

  member this.Value = let (DialogStepId id) = this in id

type DialogStep =
  { Id: DialogStepId
    Resource: string
    Field: string
    Next: DialogStepId }

type DialogLastStep = { Id: DialogStepId; Resource: string }

type DialogTemplateId =
  | DialogTemplateId of string

  member this.Value = let (DialogTemplateId id) = this in id

type DialogTemplate =
  { Id: DialogTemplateId
    FirstStep: DialogStep
    Steps: DialogStep list
    LastStep: DialogLastStep }

type IDialogResult =
  abstract FromDialogData: Map<string, obj> -> 'T

[<RequireQualifiedAccess>]
module Dialog =
  type IsStopCommand = string -> bool

  type Ongoing =
    { Id: DialogId
      TemplateId: DialogTemplateId
      CurrentStepId: DialogStepId
      Data: Map<string, obj> }

  type Finished =
    { Id: DialogId
      TemplateId: DialogTemplateId
      Data: Map<string, obj> }

type CurrentDialogId = string

type CurrentDialog = { DialogId: DialogId }

[<CLIMutable>]
type Chat =
  { Id: ChatId
    CurrentDialog: CurrentDialog option }

type IStartDialog =
  abstract StartDialog: ChatId * DialogTemplateId -> Task

type DialogRunResult =
  | Move of Dialog.Ongoing
  | End of Dialog.Finished

type DialogRunError =
  | NoDialog

type ITryRunCurrentDialog =
  abstract TryRunCurrentDialog: ChatId * 'a -> Task<Result<DialogRunResult, DialogRunError>>

type StopDialogError =
  | NoDialog

type IStopCurrentDialog =
  abstract StopCurrentDialog: ChatId -> Task<Result<unit, StopDialogError>>

type IChatDialogService =
  inherit IStartDialog
  inherit ITryRunCurrentDialog
  inherit IStopCurrentDialog