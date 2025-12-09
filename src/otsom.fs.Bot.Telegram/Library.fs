namespace otsom.fs.Bot.Telegram

open Telegram.Bot.Types.Enums
open otsom.fs.Bot
open Telegram.Bot
open Telegram.Bot.Types
open otsom.fs.Bot.Telegram.Helpers
open Telegram.Bot.Types.ReplyMarkups
open FsToolkit.ErrorHandling

type BotService(bot: ITelegramBotClient, chatId: otsom.fs.Bot.ChatId) =
  let getReplyMarkup (keyboard: Keyboard) =
    keyboard |> Seq.map (Seq.map KeyboardButton) |> ReplyKeyboardMarkup

  let getInlineMarkup (buttons: MessageButtons) =
    buttons
    |> Seq.map (Seq.map InlineKeyboardButton.WithCallbackData)
    |> InlineKeyboardMarkup

  interface IBotService with
    member this.SendMessage(text) =
      bot.SendMessage((chatId.Value |> ChatId), text |> escapeMarkdownString, parseMode = ParseMode.MarkdownV2)
      |> Task.map (_.MessageId >> BotMessageId)

    member this.AskForReply(text) =
      bot.SendMessage(
        (chatId.Value |> ChatId),
        text |> escapeMarkdownString,
        parseMode = ParseMode.MarkdownV2,
        replyMarkup = ForceReplyMarkup()
      )
      |> Task.map ignore

    member this.DeleteBotMessage(botMessageId) =
      bot.DeleteMessage(chatId.Value, botMessageId.Value)

    member this.EditMessage(botMessageId, text) =
      bot.EditMessageText((chatId.Value |> ChatId), botMessageId.Value, text |> escapeMarkdownString, parseMode = ParseMode.MarkdownV2)
      |> Task.map ignore

    member this.EditMessageButtons(botMessageId, text, buttons) =
      bot.EditMessageText(
        (chatId.Value |> ChatId),
        botMessageId.Value,
        text |> escapeMarkdownString,
        replyMarkup = getInlineMarkup buttons,
        parseMode = ParseMode.MarkdownV2
      )
      |> Task.map ignore

    member this.ReplyToMessage(chatMessageId, text) =
      bot.SendMessage(
        (chatId.Value |> ChatId),
        text |> escapeMarkdownString,
        parseMode = ParseMode.MarkdownV2,
        replyParameters = chatMessageId.Value
      )
      |> Task.map (_.MessageId >> BotMessageId)

    member this.SendKeyboard(text, keyboard) =
      bot.SendMessage(
        (chatId.Value |> ChatId),
        text |> escapeMarkdownString,
        replyMarkup = getReplyMarkup keyboard,
        parseMode = ParseMode.MarkdownV2
      )
      |> Task.map (_.MessageId >> BotMessageId)

    member this.RemoveKeyboard(text) =
      bot.SendMessage(
        (chatId.Value |> ChatId),
        text |> escapeMarkdownString,
        replyMarkup = ReplyKeyboardRemove(),
        parseMode = ParseMode.MarkdownV2
      )
      |> Task.map (_.MessageId >> BotMessageId)

    member this.SendLink(text, linkTitle, link) =
      let linkButton = InlineKeyboardButton.WithUrl(linkTitle, link.ToString())

      bot.SendMessage(
        (chatId.Value |> ChatId),
        text |> escapeMarkdownString,
        parseMode = ParseMode.MarkdownV2,
        replyMarkup = InlineKeyboardMarkup linkButton
      )
      |> Task.map (_.MessageId >> BotMessageId)

    member this.SendMessageButtons(text, buttons) =
      bot.SendMessage(
        (chatId.Value |> ChatId),
        text |> escapeMarkdownString,
        replyMarkup = getInlineMarkup buttons,
        parseMode = ParseMode.MarkdownV2
      )
      |> Task.map (_.MessageId >> BotMessageId)

    member this.SendNotification(clickId, text) = task { do! bot.AnswerCallbackQuery(clickId.Value, text) }