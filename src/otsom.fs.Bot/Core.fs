namespace otsom.fs.Bot.Core

open System.Threading.Tasks
open Microsoft.FSharp.Core
open otsom.fs.Core

type BotMessageId = BotMessageId of int
type ChatMessageId = ChatMessageId of int

type MessageId =
  | Bot of BotMessageId
  | Chat of ChatMessageId

[<RequireQualifiedAccess>]
module BotMessageId =
  let value (BotMessageId id) = id

type MessageButton = string * string
type KeyboardButton = string

type SendMessage = string -> Task<BotMessageId>
type SendUserMessage = UserId -> SendMessage
type ReplyToMessage = string -> Task<BotMessageId>
type ReplyToUserMessage = UserId -> int -> ReplyToMessage
// type SendMessageButtons = string -> InlineKeyboardMarkup -> Task<unit>
// type SendUserMessageButtons = UserId -> SendMessageButtons
// type SendKeyboard = string -> ReplyKeyboardMarkup -> Task<unit>
// type SendUserKeyboard = UserId -> SendKeyboard
// type ReplyWithKeyboard = string -> ReplyKeyboardMarkup -> Task<unit>
// type ReplyUserWithKeyboard = UserId -> int -> ReplyWithKeyboard
type AskForReply = string -> Task<unit>
type AskUserForReply = UserId -> int -> AskForReply
type EditMessage = string -> Task<unit>
type EditBotMessage = UserId -> BotMessageId -> EditMessage
// type EditMessageButtons = string -> InlineKeyboardMarkup -> Task<unit>
// type EditBotMessageButtons = UserId -> BotMessageId -> EditMessageButtons

type ChatId = string

type ConversationId = string

type ConversationStepId = string

type ConversationStep =
  { Id: ConversationStepId
    Resource: string
    Next: ConversationStepId option }

type ConversationTemplateId = string

type ConversationTemplate =
  { Id: ConversationTemplateId
    FirstStep: ConversationStep
    Steps: ConversationStep list }

type CurrentStepData =
  { BotMessageId: BotMessageId
    ConversationStepId: ConversationStepId }

type OngoingConversationStep =
  { BotMessageId: BotMessageId
    ChatMessageId: ChatMessageId
    ConversationStepId: ConversationStepId
    Data: string }

[<RequireQualifiedAccess>]
module OngoingConversationStep =
  let fromCurrentStepData (currentStepData: CurrentStepData) chatMessageId data : OngoingConversationStep =
    { BotMessageId = currentStepData.BotMessageId
      Data = data
      ChatMessageId = chatMessageId
      ConversationStepId = currentStepData.ConversationStepId }

type FinishedConversationStep =
  { ConversationStepId: ConversationStepId
    Data: string }

[<RequireQualifiedAccess>]
module FinishedConversationStep =
  let fromCurrentStepData (currentStepData: CurrentStepData) data : FinishedConversationStep =
    { ConversationStepId = currentStepData.ConversationStepId
      Data = data }

  let fromOngoingStep (ongoing: OngoingConversationStep) =
    { ConversationStepId = ongoing.ConversationStepId
      Data = ongoing.Data }

[<RequireQualifiedAccess>]
module Conversation =
  type New =
    { Id: ConversationId
      TemplateId: ConversationTemplateId
      CurrentStepData: CurrentStepData }

  type Ongoing =
    { Id: ConversationId
      TemplateId: ConversationTemplateId
      CurrentStepData: CurrentStepData
      StepsData: OngoingConversationStep list }

  [<RequireQualifiedAccess>]
  module Ongoing =
    let fromNew (new': New) chatMessageId data stepData : Ongoing =
      { Id = new'.Id
        TemplateId = new'.TemplateId
        CurrentStepData = stepData
        StepsData = [ OngoingConversationStep.fromCurrentStepData new'.CurrentStepData chatMessageId data ] }

  let create =
    fun templateId botMessageId stepId ->
      { Id = ""
        TemplateId = templateId
        CurrentStepData =
          { BotMessageId = botMessageId
            ConversationStepId = stepId }
        StepsData = [] }

  // [<RequireQualifiedAccess>]
  // module Ongoing =

  type Finished =
    { Id: ConversationId
      TemplateId: ConversationTemplateId
      StepsData: FinishedConversationStep list }

  [<RequireQualifiedAccess>]
  module Finished =
    let fromNew (new': New) data : Finished =
      { Id = new'.Id
        StepsData = [ FinishedConversationStep.fromCurrentStepData new'.CurrentStepData data ]
        TemplateId = new'.TemplateId }

    let fromOngoing: Ongoing -> Finished =
      fun ongoing ->
        { Id = ongoing.Id
          TemplateId = ongoing.TemplateId
          StepsData = ongoing.StepsData |> List.map FinishedConversationStep.fromOngoingStep }

type Conversation =
  | New of Conversation.New
  | Ongoing of Conversation.Ongoing
  | Finished of Conversation.Finished

type CurrentConversationId = string

type CurrentConversation = { ConversationId: ConversationId }

type Chat =
  { Id: ChatId
    CurrentConversation: CurrentConversation option }
