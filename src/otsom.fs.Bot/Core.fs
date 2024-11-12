namespace otsom.fs.Bot

open System.Threading.Tasks
open Microsoft.FSharp.Core

type ChatMessageId =
  | BotMessageId of int
  | UserMessageId of int

type ChatId =
  | UserId of int64
  | GroupId of int64
  | ChannelId of int64

type SendChatMessage = ChatId -> string -> Task<ChatMessageId>
