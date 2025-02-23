namespace otsom.fs.Bot

open System.Runtime.InteropServices
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
  abstract SendMessage: string -> Task<BotMessageId>

type IDeleteBotMessage =
  abstract DeleteBotMessage: BotMessageId -> Task

type KeyboardButton = string

type Keyboard = KeyboardButton seq seq

type ISendKeyboard =
  abstract SendKeyboard: string * Keyboard -> Task<BotMessageId>

type IEditMessage =
  abstract EditMessage: BotMessageId * string -> Task<unit>

type MessageButton = string * string
type MessageButtons = MessageButton seq seq

type ISendMessageButtons =
  abstract SendMessageButtons: string * MessageButtons -> Task<BotMessageId>

type IAskForReply =
  abstract AskForReply: string -> Task<unit>

type IEditMessageButtons =
  abstract EditMessageButtons: BotMessageId * string * MessageButtons -> Task<unit>

type IReplyToMessage =
  abstract ReplyToMessage: ChatMessageId * string -> Task<BotMessageId>

type ISendLink =
  abstract SendLink: string * string * Uri -> Task<BotMessageId>

type ButtonClickId =
  | ButtonClickId of string

  member this.Value = let (ButtonClickId id) = this in id

type ISendNotification =
  abstract SendNotification: ButtonClickId * text: string -> Task<unit>

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

type BuildBotService = ChatId -> IBotService