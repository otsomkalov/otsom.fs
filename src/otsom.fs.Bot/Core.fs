namespace otsom.fs.Bot

open System.Threading.Tasks
open Microsoft.FSharp.Core

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

type ChatId =
  | ChatId of int64

  member this.Value =
    let (ChatId id) = this
    id

type Keyboard = KeyboardButton seq seq

type MessageButtons = MessageButton seq seq

[<RequireQualifiedAccess>]
module Keyboard =
  let empty = Seq.empty<string seq>

type ISendMessage =
  abstract member SendMessage: chatId: ChatId * text: string -> Task<BotMessageId>

type IEditMessage =
  abstract member EditMessage: chatId: ChatId * messageId: BotMessageId * text: string -> Task

type IReplyToMessage =
  abstract member ReplyToMessage: chatId: ChatId * messageId: ChatMessageId * text: string -> Task<BotMessageId>

type ISendMessageButtons =
  abstract member SendMessageButtons: chatId: ChatId * text: string * buttons: MessageButtons -> Task<BotMessageId>

type ISendKeyboard =
  abstract member SendKeyboard: chatId: ChatId * text: string * buttons: Keyboard -> Task<BotMessageId>

type IDeleteMessage =
  abstract member DeleteMessage: chstId: ChatId * messageId: MessageId -> Task
