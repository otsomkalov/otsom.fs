namespace otsom.fs.Bot.Telegram

open otsom.fs.Bot
open Telegram.Bot
open Telegram.Bot.Types
open otsom.fs.Extensions
open otsom.fs.Bot.Telegram.Helpers
open Telegram.Bot.Types.ReplyMarkups

module internal Workflows =

  let sendChatMessage (bot: ITelegramBotClient) : SendChatMessage =
    fun chatId text ->
      bot.SendTextMessageAsync((chatId.Value |> ChatId), text |> escapeMarkdownString)
      &|> (_.MessageId >> BotMessageId)

  let private getReplyMarkup (keyboard: Keyboard) =
    keyboard |> Seq.map (Seq.map KeyboardButton) |> ReplyKeyboardMarkup

  let sendChatKeyboard (bot: ITelegramBotClient) : SendChatKeyboard =
    fun chatId text keyboard ->
      bot.SendTextMessageAsync((chatId.Value |> ChatId), text |> escapeMarkdownString, replyMarkup = getReplyMarkup keyboard)
      &|> (_.MessageId >> BotMessageId)

  let deleteBotMessage (bot: ITelegramBotClient) : DeleteBotMessage =
    fun chatId messageId -> bot.DeleteMessageAsync((chatId.Value |> ChatId), messageId.Value)

  let editBotMessage (bot: ITelegramBotClient) : EditBotMessage =
    fun chatId messageId text ->
      bot.EditMessageTextAsync((chatId.Value |> ChatId), messageId.Value, text |> escapeMarkdownString)
      &|> ignore
