namespace otsom.fs.Bot

open System.Threading.Tasks
open Microsoft.FSharp.Core

type BotMessageId =
  | BotMessageId of int

  member this.Value = let (BotMessageId id) = this in id

type ChatId =
  | ChatId of int64

  member this.Value = let (ChatId id) = this in id

type DeleteBotMessage = ChatId -> BotMessageId -> Task

type SendMessage = string -> Task<BotMessageId>

type SendChatMessage = ChatId -> SendMessage

type ISendMessage =
  abstract SendMessage: SendMessage

type IChatContext =
  inherit ISendMessage

type BuildChatContext = ChatId -> IChatContext
