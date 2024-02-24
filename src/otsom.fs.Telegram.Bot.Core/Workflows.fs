namespace otsom.fs.Telegram.Bot

open Telegram.Bot
open Telegram.Bot.Types
open Core
open otsom.fs.Extensions

module Workflows =
  let sendUserMessage (bot: ITelegramBotClient) : SendUserMessage =
    fun userId ->
      fun text ->
        bot.SendTextMessageAsync((userId |> UserId.value |> ChatId), text)
        |> Task.map ignore

  let replyToUserMessage (bot: ITelegramBotClient) : ReplyToUserMessage =
    fun userId messageId ->
      fun text ->
        bot.SendTextMessageAsync((userId |> UserId.value |> ChatId), text, replyToMessageId = messageId)
        |> Task.map ignore
