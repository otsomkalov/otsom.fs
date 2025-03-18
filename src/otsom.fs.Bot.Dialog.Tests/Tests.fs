module otsom.fs.Bot.Dialog

open Moq
open Xunit
open otsom.fs.Bot
open otsom.fs.Bot.Dialog
open otsom.fs.Bot.Dialog.Repos
open otsom.fs.Resources
open FsUnit.Xunit

let chatId = ChatId 1
let templateId = DialogTemplateId "test-template"

let template: DialogTemplate =
  { Id = templateId
    FirstStep =
      { Id = DialogStepId "step1"
        Resource = "step1"
        Field = "field1"
        Next = DialogStepId "step2" }
    Steps =
      [ { Id = DialogStepId "step2"
          Resource = "step2"
          Field = "field2"
          Next = DialogStepId "step3" } ]
    LastStep =
      { Id = DialogStepId "step3"
        Resource = "step3" } }

let botMessageId = BotMessageId 1

let dialogId = DialogId "test-dialog-id"

let chat: Chat =
  { Id = chatId
    CurrentDialog = Some { DialogId = dialogId } }

let startedDialog: Ongoing =
  { Id = dialogId
    Data = Map.empty
    TemplateId = templateId
    CurrentStepId = template.FirstStep.Id }

let data1 = "test-data-1"

let ongoingDialog: Ongoing =
  { startedDialog with
      Data = startedDialog.Data |> Map.add template.FirstStep.Field data1
      CurrentStepId = template.Steps[0].Id }

let data2 = "test-data-2"

let finishedDialog: Finished =
  { Id = dialogId
    TemplateId = templateId
    Data =
      [ (template.FirstStep.Field, data1 :> obj)
        (template.Steps[0].Field, data2 :> obj) ]
      |> Map<string, obj> }

type ChatDialogServiceTests() =
  let dialogRepo = Mock<IDialogRepo>()

  let chatRepo = Mock<IChatRepo>()

  do chatRepo.Setup(_.LoadChat(chatId)).ReturnsAsync(chat) |> ignore

  let dialogTemplateRepo = Mock<IDialogTemplateRepo>()

  do
    dialogTemplateRepo
      .Setup(_.LoadDialogTemplate(templateId))
      .ReturnsAsync(template)
    |> ignore

  let resp = Mock<IResourceProvider>()

  do resp.Setup(_.Item(It.IsAny<string>())).Returns(id) |> ignore

  let botService = Mock<IBotService>()

  let sut =
    ChatDialogService(dialogRepo.Object, chatRepo.Object, dialogTemplateRepo.Object, resp.Object, botService.Object) :> IChatDialogService

  [<Fact>]
  let ``Starts dialog if chat has no current dialog`` () =
    dialogRepo.Setup(_.UpdateOngoingDialog(startedDialog)).ReturnsAsync(())

    chatRepo
      .Setup(_.LoadChat(chatId))
      .ReturnsAsync({ chat with CurrentDialog = None })

    botService
      .Setup(_.SendKeyboard(template.FirstStep.Resource, It.IsAny<Keyboard>()))
      .ReturnsAsync(botMessageId)

    dialogTemplateRepo
      .Setup(_.LoadDialogTemplate(templateId))
      .ReturnsAsync(template)

    dialogRepo.Setup(_.GenerateDialogId()).Returns(dialogId)

    let expectedChat: Chat =
      { chat with
          CurrentDialog = Some { DialogId = dialogId } }

    chatRepo.Setup(_.UpdateChat(expectedChat)).ReturnsAsync(())

    task {
      let! result = sut.StartDialog(chatId, templateId)

      result |> should equal (Result<_, StartDialogError>.Ok())

      dialogTemplateRepo.VerifyAll()
      chatRepo.VerifyAll()
      dialogRepo.VerifyAll()
      botService.VerifyAll()
    }

  [<Fact>]
  let ``Returns error if chat has current dialog`` () = task {
    let! result = sut.StartDialog(chatId, templateId)

    result
    |> should equal (Result<unit, _>.Error StartDialogError.DialogAlreadyStarted)

    chatRepo.VerifyAll()
  }

  [<Fact>]
  let ``Moves to the dialog next step and saves the data`` () =
    dialogRepo.Setup(_.LoadOngoingDialog(dialogId)).ReturnsAsync(startedDialog)

    botService
      .Setup(_.SendKeyboard(template.Steps[0].Resource, It.IsAny<Keyboard>()))
      .ReturnsAsync(botMessageId)

    task {
      let! result = sut.TryRunCurrentDialog(chatId, data1)

      result
      |> should equal (Result<_, Dialog.DialogRunError>.Ok(DialogRunResult.Move ongoingDialog))

      dialogRepo.VerifyAll()
      chatRepo.VerifyAll()
      botService.VerifyAll()
    }

  [<Fact>]
  let ``Finishes the dialog if the last step is reached`` () =
    dialogRepo.Setup(_.LoadOngoingDialog(dialogId)).ReturnsAsync(ongoingDialog)

    task {
      let! result = sut.TryRunCurrentDialog(chatId, data2)

      result
      |> should equal (Result<_, Dialog.DialogRunError>.Ok(DialogRunResult.End finishedDialog))

      dialogRepo.VerifyAll()
    }

  [<Fact>]
  let ``Returns error if chat has no current dialog`` () =
    chatRepo
      .Setup(_.LoadChat(chatId))
      .ReturnsAsync({ chat with CurrentDialog = None })

    task {
      let! result = sut.TryRunCurrentDialog(chatId, data1)

      result
      |> should equal (Result<DialogRunResult, _>.Error DialogRunError.NoDialog)

      chatRepo.VerifyAll()
    }