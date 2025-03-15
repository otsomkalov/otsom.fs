namespace otsom.fs.Bot.Dialog.Workflows

open System.Threading.Tasks
open Microsoft.FSharp.Core
open otsom.fs.Bot
open otsom.fs.Bot.Dialog
open otsom.fs.Bot.Dialog.Repos
open otsom.fs.Extensions.Operators
open otsom.fs.Resources

module internal Dialog =
  let internal sendKeyboardWithStopButton (resp: IResourceProvider) (botService: #ISendKeyboard) =
    fun textRes keyboard ->
      let keyboardWithStopDialogButton = seq {
        yield! keyboard

        seq { KeyboardButton(resp["stop"]) }
      }

      botService.SendKeyboard(resp[textRes], keyboardWithStopDialogButton) &|> ignore

  let internal run
    (dialogTemplateRepo: #ILoadDialogTemplate)
    (dialogRepo: #ISaveFinishedDialog & #IUpdateOngoingDialog)
    (resp: IResourceProvider)
    (botService: #IAskForReply & #ISendMessage & #ISendKeyboard)
    =
    fun data (dialog: Dialog.Ongoing) ->
      dialog.TemplateId |> dialogTemplateRepo.LoadDialogTemplate
      &|&> (fun template ->
        let currentStep =
          match dialog.CurrentStepId with
          | currentId when currentId = template.FirstStep.Id -> template.FirstStep
          | currentId -> template.Steps |> List.find (fun s -> s.Id = currentId)

        if currentStep.Next = template.LastStep.Id then
          task {
            let finishedDialog: Dialog.Finished =
              { Id = dialog.Id
                TemplateId = dialog.TemplateId
                Data = dialog.Data }

            do! dialogRepo.SaveFinishedDialog(finishedDialog)

            do! botService.SendMessage(resp[template.LastStep.Resource]) &|> ignore

            return DialogRunResult.End finishedDialog
          }
        else
          task {
            let nextStep = template.Steps |> List.find (fun s -> s.Id = currentStep.Next)

            do! sendKeyboardWithStopButton resp botService nextStep.Resource []

            let updatedDialog =
              { dialog with
                  Data = dialog.Data |> Map.add currentStep.Field data
                  CurrentStepId = nextStep.Id }

            do! dialogRepo.UpdateOngoingDialog(updatedDialog)

            return DialogRunResult.Move updatedDialog
          })

type ChatDialogService
  (dialogRepo: IDialogRepo, chatRepo: IChatRepo, dialogTemplateRepo: IDialogTemplateRepo, resp: IResourceProvider, botService: IBotService)
  =
  let sendKeyboardWithStopButton = Dialog.sendKeyboardWithStopButton resp botService

  interface IChatDialogService with
    member this.StartDialog(chatId, templateId) = task {
      let! template = dialogTemplateRepo.LoadDialogTemplate templateId

      do! sendKeyboardWithStopButton template.FirstStep.Resource []

      let dialog: Dialog.Ongoing =
        { Id = dialogRepo.GenerateDialogId()
          TemplateId = templateId
          Data = Map.empty
          CurrentStepId = template.FirstStep.Id }

      do! dialogRepo.UpdateOngoingDialog dialog

      let! chat = chatRepo.LoadChat chatId

      let updatedChat =
        { chat with
            CurrentDialog = Some { DialogId = dialog.Id } }

      do! chatRepo.UpdateChat updatedChat

      return ()
    }

    member this.TryRunCurrentDialog(chatId, data) =
      chatId |> chatRepo.LoadChat
      &|&> (function
      | { CurrentDialog = Some { DialogId = dialogId } } as chat ->
        dialogRepo.LoadOngoingDialog dialogId
        &|&> (Dialog.run dialogTemplateRepo dialogRepo resp botService data)
        &|&> (function
        | DialogRunResult.Move d -> DialogRunResult.Move d |> Ok |> Task.FromResult
        | DialogRunResult.End d -> task {
            do! chatRepo.UpdateChat { chat with CurrentDialog = None }
            return Ok(DialogRunResult.End d)
          })
      | _ -> Error DialogRunError.NoDialog |> Task.FromResult)

    member this.StopCurrentDialog(chatId) = task {
      let! chat = chatId |> chatRepo.LoadChat

      match chat with
      | { CurrentDialog = Some _ } as chat ->
        let updatedChat = { chat with CurrentDialog = None }

        do! chatRepo.UpdateChat updatedChat

        return Ok()
      | _ -> return Error StopDialogError.NoDialog
    }

type GetChatDialogService = IBotService * IResourceProvider -> IChatDialogService