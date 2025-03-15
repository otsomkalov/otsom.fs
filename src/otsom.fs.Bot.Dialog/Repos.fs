namespace otsom.fs.Bot.Dialog.Repos

open System.Threading.Tasks
open otsom.fs.Bot
open otsom.fs.Bot.Dialog

type IUpdateChat =
  abstract UpdateChat: Chat -> Task<unit>

type ILoadChat =
  abstract LoadChat: ChatId -> Task<Chat>

type IChatRepo =
  inherit IUpdateChat
  inherit ILoadChat

type ILoadDialogTemplate =
  abstract LoadDialogTemplate: DialogTemplateId -> Task<DialogTemplate>

type IDialogTemplateRepo =
  inherit ILoadDialogTemplate

type ILoadOngoingDialog =
  abstract LoadOngoingDialog: DialogId -> Task<Dialog.Ongoing>

type IUpdateOngoingDialog =
  abstract UpdateOngoingDialog: Dialog.Ongoing -> Task

type ISaveFinishedDialog =
  abstract SaveFinishedDialog: Dialog.Finished -> Task

type IGenerateDialogId =
  abstract GenerateDialogId: unit -> DialogId

type IDialogRepo =
  inherit ILoadOngoingDialog
  inherit IUpdateOngoingDialog
  inherit ISaveFinishedDialog
  inherit IGenerateDialogId