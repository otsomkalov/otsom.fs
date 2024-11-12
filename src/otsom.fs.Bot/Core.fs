namespace otsom.fs.Bot

open System.Threading.Tasks
open Microsoft.FSharp.Core

type BotMessageId = BotMessageId of int

type ChatId =
  | UserId of int64
  | GroupId of int64
  | ChannelId of int64

type SendMessage = string -> Task<BotMessageId>
type SendChatMessage = ChatId -> SendMessage
