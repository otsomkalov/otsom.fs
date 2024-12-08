namespace otsom.fs.Bot.Telegram

open otsom.fs.Bot
open Telegram.Bot
open Telegram.Bot.Types
open otsom.fs.Extensions

module internal Workflows =

  let sendChatMessage (bot: ITelegramBotClient) : SendChatMessage =
    fun chatId text ->
      bot.SendTextMessageAsync((chatId.Value |> ChatId), text)
      &|> (_.MessageId >> BotMessageId)

  let deleteBotMessage (bot: ITelegramBotClient) : DeleteBotMessage =
    fun chatId messageId ->

      bot.DeleteMessageAsync((chatId.Value |> ChatId), messageId.Value)
