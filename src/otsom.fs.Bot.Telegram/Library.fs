namespace otsom.fs.Bot.Telegram

open Telegram.Bot.Types.Enums
open otsom.fs.Bot
open Telegram.Bot
open Telegram.Bot.Types
open otsom.fs.Extensions
open otsom.fs.Bot.Telegram.Helpers
open Telegram.Bot.Types.ReplyMarkups

module internal Workflows =

  let sendChatMessage (bot: ITelegramBotClient) : SendChatMessage =
    fun chatId text ->
      bot.SendTextMessageAsync((chatId.Value |> ChatId), text |> escapeMarkdownString, parseMode = ParseMode.MarkdownV2)
      &|> (_.MessageId >> BotMessageId)

  let private getReplyMarkup (keyboard: Keyboard) =
    keyboard |> Seq.map (Seq.map KeyboardButton) |> ReplyKeyboardMarkup

  let sendChatKeyboard (bot: ITelegramBotClient) : SendChatKeyboard =
    fun chatId text keyboard ->
      bot.SendTextMessageAsync(
        (chatId.Value |> ChatId),
        text |> escapeMarkdownString,
        replyMarkup = getReplyMarkup keyboard,
        parseMode = ParseMode.MarkdownV2
      )
      &|> (_.MessageId >> BotMessageId)

  let deleteBotMessage (bot: ITelegramBotClient) : DeleteBotMessage =
    fun chatId messageId -> bot.DeleteMessageAsync((chatId.Value |> ChatId), messageId.Value)

  let editBotMessage (bot: ITelegramBotClient) : EditBotMessage =
    fun chatId messageId text ->
      bot.EditMessageTextAsync((chatId.Value |> ChatId), messageId.Value, text |> escapeMarkdownString, parseMode = ParseMode.MarkdownV2)
      &|> ignore

  let private getInlineMarkup (buttons: MessageButtons) =
    buttons
    |> Seq.map (Seq.map InlineKeyboardButton.WithCallbackData)
    |> InlineKeyboardMarkup

  let sendChatMessageButtons (bot: ITelegramBotClient) : SendChatMessageButtons =
    fun chatId text buttons ->
      bot.SendTextMessageAsync(
        (chatId.Value |> ChatId),
        text |> escapeMarkdownString,
        replyMarkup = getInlineMarkup buttons,
        parseMode = ParseMode.MarkdownV2
      )
      &|> (_.MessageId >> BotMessageId)

  let editBotMessageButtons (bot: ITelegramBotClient) : EditBotMessageButtons =
    fun chatId messageId text buttons ->
      bot.EditMessageTextAsync(
        (chatId.Value |> ChatId),
        messageId.Value,
        text |> escapeMarkdownString,
        replyMarkup = getInlineMarkup buttons,
        parseMode = ParseMode.MarkdownV2
      )
      &|> ignore

  let askForReply (bot: ITelegramBotClient) (chatId: otsom.fs.Bot.ChatId) : AskForReply =
    fun text ->
      bot.SendTextMessageAsync(
        (chatId.Value |> ChatId),
        text |> escapeMarkdownString,
        parseMode = ParseMode.MarkdownV2,
        replyMarkup = ForceReplyMarkup()
      )
      &|> ignore
