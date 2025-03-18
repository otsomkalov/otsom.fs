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
  abstract LoadOngoingDialog: DialogId -> Task<Ongoing>

type IUpdateOngoingDialog =
  abstract UpdateOngoingDialog: Ongoing -> Task<unit>

type ISaveFinishedDialog =
  abstract SaveFinishedDialog: Finished -> Task<unit>

type IGenerateDialogId =
  abstract GenerateDialogId: unit -> DialogId

type IDialogRepo =
  inherit ILoadOngoingDialog
  inherit IUpdateOngoingDialog
  inherit ISaveFinishedDialog
  inherit IGenerateDialogId