namespace otsom.fs.Bot

open System.Threading.Tasks
open Microsoft.FSharp.Core

type BotMessageId =
  | BotMessageId of int

  member this.Value = let (BotMessageId id) = this in id

type ChatMessageId =
  | ChatMessageId of int

  member this.Value = let (ChatMessageId id) = this in id

type ChatId =
  | ChatId of int64

  member this.Value = let (ChatId id) = this in id

type DeleteBotMessage = ChatId -> BotMessageId -> Task

type SendMessage = string -> Task<BotMessageId>
type SendChatMessage = ChatId -> SendMessage

type ISendMessage =
  abstract SendMessage: SendMessage

type IDeleteBotMessage =
  abstract DeleteBotMessage: DeleteBotMessage

type KeyboardButton = string

type Keyboard = KeyboardButton seq seq
type SendKeyboard = string -> Keyboard -> Task<BotMessageId>
type SendChatKeyboard = ChatId -> SendKeyboard

type ISendKeyboard =
  abstract SendKeyboard: SendKeyboard

type EditMessage = string -> Task<unit>
type EditBotMessage = ChatId -> BotMessageId -> EditMessage

type IEditMessage =
  abstract EditMessage: EditMessage

type MessageButton = string * string
type MessageButtons = MessageButton seq seq
type SendMessageButtons = string -> MessageButtons -> Task<BotMessageId>
type SendChatMessageButtons = ChatId -> SendMessageButtons

type ISendMessageButtons =
  abstract SendMessageButtons: SendMessageButtons

type EditMessageButtons = string -> MessageButtons -> Task<unit>
type EditBotMessageButtons = ChatId -> BotMessageId -> EditMessageButtons

type AskForReply = string -> Task<unit>
type AskChatForReply = ChatId -> AskForReply

type IAskForReply =
  abstract AskForReply: AskForReply

type IEditMessageButtons =
  abstract EditMessageButtons: EditMessageButtons

type IBotMessageContext =
  inherit IEditMessage
  inherit IEditMessageButtons

type IBuildBotMessageContext =
  abstract BuildBotMessageContext: BotMessageId -> IBotMessageContext

type ReplyToMessage = string -> Task<BotMessageId>
type ReplyToChatMessage = ChatId -> ChatMessageId -> ReplyToMessage

type IReplyToMessage =
  abstract ReplyToMessage: ReplyToMessage

type IChatMessageContext =
  inherit IReplyToMessage

type BuildChatMessageContext = ChatMessageId -> IChatMessageContext

type IBuildChatMessageContext =
  abstract BuildChatMessageContext: BuildChatMessageContext

type IChatContext =
  inherit ISendMessage
  inherit ISendKeyboard
  inherit ISendMessageButtons

  inherit IDeleteBotMessage

  inherit IAskForReply

  inherit IBuildBotMessageContext
  inherit IBuildChatMessageContext

type BuildChatContext = ChatId -> IChatContext

type SendNotification = string -> Task<unit>

type ISendNotification =
  abstract SendNotification: SendNotification

type IButtonClickContext =
  inherit ISendNotification

type ButtonClickId =
  | ButtonClickId of string

  member this.Value = let (ButtonClickId id) = this in id

type BuildButtonClickContext = ButtonClickId -> IButtonClickContext