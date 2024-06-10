namespace otsom.fs.Telegram.Bot

open System.Threading.Tasks
open Telegram.Bot.Types.ReplyMarkups
open otsom.fs.Core

module Core =
  type BotMessageId = BotMessageId of int

  [<RequireQualifiedAccess>]
  module BotMessageId =
    let value (BotMessageId id) = id

  type MessageButton = string * string
  type KeyboardButton = string

  type SendMessage = string -> Task<unit>
  type SendUserMessage = UserId -> SendMessage
  type ReplyToMessage = string -> Task<BotMessageId>
  type ReplyToUserMessage = UserId -> int -> ReplyToMessage
  type SendMessageButtons = string -> InlineKeyboardMarkup -> Task<unit>
  type SendUserMessageButtons = UserId -> SendMessageButtons
  type SendKeyboard = string -> ReplyKeyboardMarkup -> Task<unit>
  type SendUserKeyboard = UserId -> SendKeyboard
  type ReplyWithKeyboard = string -> ReplyKeyboardMarkup -> Task<unit>
  type ReplyUserWithKeyboard = UserId -> int -> ReplyWithKeyboard
  type AskForReply = string -> Task<unit>
  type AskUserForReply = UserId -> int -> AskForReply
  type EditMessage = string -> Task<unit>
  type EditBotMessage = UserId -> BotMessageId -> EditMessage
  type EditMessageButtons = string -> InlineKeyboardMarkup -> Task<unit>
  type EditBotMessageButtons = UserId -> BotMessageId -> EditMessageButtons
