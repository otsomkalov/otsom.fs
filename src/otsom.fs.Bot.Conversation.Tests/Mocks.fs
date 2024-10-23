[<RequireQualifiedAccess>]
module otsom.fs.Bot.Conversation.Tests.Mocks

open otsom.fs.Bot.Conversation
open otsom.fs.Bot.Core

let chatId = "chat-id"

let chat: Chat =
  { Id = chatId
    CurrentConversation = None }

let templateFirstStepId = "conversation-template-first-step-id"
let templateFirstStepResource = "conversation-template-first-resource"

let templateSecondStepId = "conversation-template-second-step-id-1"
let templateSecondStepResource = "conversation-template-second-resource"

let templateFinalStepId = "conversation-template-final-step-id"
let templateFinalStepResource = "conversation-template-final-resource"

let templateId = "conversation-template-id"

let template: ConversationTemplate =
  { Id = templateId
    FirstStep =
      { Id = templateFirstStepId
        Next = Some templateSecondStepId
        Resource = templateFirstStepResource }
    Steps =
      [ { Id = templateSecondStepId
          Next = Some templateFinalStepId
          Resource = templateSecondStepResource }
        { Id = templateFinalStepId
          Next = None
          Resource = templateFinalStepResource } ] }

let resourceId = "resource-id"
let resourceValue = "resource-value"

let botMessageId = BotMessageId 1

[<RequireQualifiedAccess>]
module Conversation =
  let id = "conversation-id"

  let new': Conversation.New =
    { Id = id
      TemplateId = templateId
      CurrentStepData =
        { BotMessageId = botMessageId
          ConversationStepId = templateFirstStepId } }
