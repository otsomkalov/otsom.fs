namespace otsom.fs.Telegram.Bot

open System.Text.RegularExpressions
open Telegram.Bot
open Telegram.Bot.Types
open Core
open Telegram.Bot.Types.Enums
open Telegram.Bot.Types.ReplyMarkups
open otsom.fs.Extensions

module Workflows =
  let private escapeMarkdownString (str: string) =
    Regex.Replace(str, "([\(\)`\.#\-!+=&\?])", "\$1")

  let sendUserMessage (bot: ITelegramBotClient) : SendUserMessage =
    fun userId ->
      fun text ->
        bot.SendTextMessageAsync((userId |> UserId.value |> ChatId), text |> escapeMarkdownString, parseMode = ParseMode.MarkdownV2)
        |> Task.ignore

  let replyToUserMessage (bot: ITelegramBotClient) : ReplyToUserMessage =
    fun userId messageId ->
      fun text ->
        bot.SendTextMessageAsync(
          (userId |> UserId.value |> ChatId),
          text |> escapeMarkdownString,
          replyToMessageId = messageId,
          parseMode = ParseMode.MarkdownV2
        )
        |> Task.map (_.MessageId >> BotMessageId)

  let sendUserMessageButtons (bot: ITelegramBotClient) : SendUserMessageButtons =
    fun userId ->
      fun text buttons ->
        bot.SendTextMessageAsync(
          (userId |> UserId.value |> ChatId),
          text |> escapeMarkdownString,
          replyMarkup = buttons,
          parseMode = ParseMode.MarkdownV2
        )
        |> Task.ignore

  let sendUserKeyboard (bot: ITelegramBotClient) : SendUserKeyboard =
    fun userId ->
      fun text buttons ->
        bot.SendTextMessageAsync(
          (userId |> UserId.value |> ChatId),
          text |> escapeMarkdownString,
          parseMode = ParseMode.MarkdownV2,
          replyMarkup = buttons
        )
        |> Task.ignore

  let replyUserWithKeyboard (bot: ITelegramBotClient) : ReplyUserWithKeyboard =
    fun userId messageId ->
      fun text buttons ->
        bot.SendTextMessageAsync(
          (userId |> UserId.value |> ChatId),
          text |> escapeMarkdownString,
          replyToMessageId = messageId,
          parseMode = ParseMode.MarkdownV2,
          replyMarkup = buttons
        )
        |> Task.ignore

  let askUserForReply (bot: ITelegramBotClient) : AskUserForReply =
    fun userId messageId ->
      fun text ->
        bot.SendTextMessageAsync(
          (userId |> UserId.value |> ChatId),
          text |> escapeMarkdownString,
          parseMode = ParseMode.MarkdownV2,
          replyToMessageId = messageId,
          replyMarkup = ForceReplyMarkup()
        )
        |> Task.ignore

  let editUserMessage (bot: ITelegramBotClient) : EditBotMessage =
    fun userId messageId ->
      fun text ->
        bot.EditMessageTextAsync((userId |> UserId.value |> ChatId), (messageId |> BotMessageId.value), text |> escapeMarkdownString, ParseMode.MarkdownV2)
        |> Task.ignore

  let editUserMessageButtons (bot: ITelegramBotClient) : EditBotMessageButtons =
    fun userId messageId ->
      fun text buttons ->
        bot.EditMessageTextAsync(
          (userId |> UserId.value |> ChatId),
          (messageId |> BotMessageId.value),
          text |> escapeMarkdownString,
          ParseMode.MarkdownV2,
          replyMarkup = buttons
        )
        |> Task.ignore
