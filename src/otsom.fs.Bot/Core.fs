namespace otsom.fs.Bot

open System.Threading.Tasks
open Microsoft.FSharp.Core
open System

type BotMessageId =
  | BotMessageId of int

  member this.Value = let (BotMessageId id) = this in id

type ChatMessageId =
  | ChatMessageId of int

  member this.Value = let (ChatMessageId id) = this in id

type ChatId =
  | ChatId of int64

  member this.Value = let (ChatId id) = this in id

type ISendMessage =
  abstract SendMessage: ChatId * string -> Task<BotMessageId>

type IDeleteBotMessage =
  abstract DeleteBotMessage: ChatId * BotMessageId -> Task

type KeyboardButton = string

type Keyboard = KeyboardButton seq seq

type ISendKeyboard =
  abstract SendKeyboard: ChatId * string * Keyboard -> Task<BotMessageId>

type IEditMessage =
  abstract EditMessage: ChatId * BotMessageId * string -> Task<unit>

type MessageButton = string * string
type MessageButtons = MessageButton seq seq

type ISendMessageButtons =
  abstract SendMessageButtons: ChatId * string * MessageButtons -> Task<BotMessageId>

type IAskForReply =
  abstract AskForReply: ChatId * string -> Task<unit>

type IEditMessageButtons =
  abstract EditMessageButtons: ChatId * BotMessageId * string * MessageButtons -> Task<unit>

type IReplyToMessage =
  abstract ReplyToMessage: ChatId * ChatMessageId * string -> Task<BotMessageId>

type ISendLink =
  abstract SendLink: ChatId * string * string * Uri -> Task<BotMessageId>

type ButtonClickId =
  | ButtonClickId of string

  member this.Value = let (ButtonClickId id) = this in id

type ISendNotification =
  abstract SendNotification: ChatId * ButtonClickId * string -> Task<unit>

type IBotService =
  inherit ISendMessage
  inherit ISendKeyboard
  inherit ISendMessageButtons
  inherit ISendLink

  inherit IReplyToMessage

  inherit IEditMessage
  inherit IEditMessageButtons

  inherit IDeleteBotMessage

  inherit IAskForReply

  inherit ISendNotification