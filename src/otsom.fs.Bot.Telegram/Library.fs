namespace otsom.fs.Bot.Telegram

open Telegram.Bot.Types.Enums
open otsom.fs.Bot
open Telegram.Bot
open Telegram.Bot.Types
open otsom.fs.Extensions
open otsom.fs.Bot.Telegram.Helpers
open Telegram.Bot.Types.ReplyMarkups

type BotService(bot: ITelegramBotClient) =
  let getReplyMarkup (keyboard: Keyboard) =
    keyboard |> Seq.map (Seq.map KeyboardButton) |> ReplyKeyboardMarkup

  let getInlineMarkup (buttons: MessageButtons) =
    buttons
    |> Seq.map (Seq.map InlineKeyboardButton.WithCallbackData)
    |> InlineKeyboardMarkup

  interface IBotService with
    member this.SendMessage(chatId, text) =
      bot.SendTextMessageAsync((chatId.Value |> ChatId), text |> escapeMarkdownString, parseMode = ParseMode.MarkdownV2)
      &|> (_.MessageId >> BotMessageId)

    member this.AskForReply(chatId, text) =
      bot.SendTextMessageAsync(
        (chatId.Value |> ChatId),
        text |> escapeMarkdownString,
        parseMode = ParseMode.MarkdownV2,
        replyMarkup = ForceReplyMarkup()
      )
      &|> ignore

    member this.DeleteBotMessage(chatId, botMessageId) =
      bot.DeleteMessageAsync(chatId.Value, botMessageId.Value)

    member this.EditMessage(chatId, botMessageId, text) =
      bot.EditMessageTextAsync((chatId.Value |> ChatId), botMessageId.Value, text |> escapeMarkdownString, parseMode = ParseMode.MarkdownV2)
      &|> ignore

    member this.EditMessageButtons(chatId, botMessageId, text, buttons) =
      bot.EditMessageTextAsync(
        (chatId.Value |> ChatId),
        botMessageId.Value,
        text |> escapeMarkdownString,
        replyMarkup = getInlineMarkup buttons,
        parseMode = ParseMode.MarkdownV2
      )
      &|> ignore

    member this.ReplyToMessage(chatId, chatMessageId, text) =
      bot.SendTextMessageAsync(
        (chatId.Value |> ChatId),
        text |> escapeMarkdownString,
        parseMode = ParseMode.MarkdownV2,
        replyToMessageId = chatMessageId.Value
      )
      &|> (_.MessageId >> BotMessageId)

    member this.SendKeyboard(chatId, text, keyboard) =
      bot.SendTextMessageAsync(
        (chatId.Value |> ChatId),
        text |> escapeMarkdownString,
        replyMarkup = getReplyMarkup keyboard,
        parseMode = ParseMode.MarkdownV2
      )
      &|> (_.MessageId >> BotMessageId)

    member this.SendLink(chatId, text, linkTitle, link) =
      let linkButton = InlineKeyboardButton.WithUrl(linkTitle, link.ToString())

      bot.SendTextMessageAsync(
        (chatId.Value |> ChatId),
        text |> escapeMarkdownString,
        parseMode = ParseMode.MarkdownV2,
        replyMarkup = InlineKeyboardMarkup linkButton
      )
      &|> (_.MessageId >> BotMessageId)

    member this.SendMessageButtons(chatId, text, buttons) =
      bot.SendTextMessageAsync(
        (chatId.Value |> ChatId),
        text |> escapeMarkdownString,
        replyMarkup = getInlineMarkup buttons,
        parseMode = ParseMode.MarkdownV2
      )
      &|> (_.MessageId >> BotMessageId)