namespace otsom.fs.Telegram.Bot

open System.Threading.Tasks

module Core =
  type UserId = UserId of int64

  [<RequireQualifiedAccess>]
  module UserId =
    let value (UserId id) = id

  type MessageButton = string * string
  type KeyboardButton = string

  type SendMessage = string -> Task<unit>
  type SendUserMessage = UserId -> SendMessage
  type ReplyToMessage = string -> Task<unit>
  type ReplyToUserMessage = UserId -> int -> ReplyToMessage