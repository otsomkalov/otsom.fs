namespace otsom.fs.Bot.Telegram

open otsom.fs.Bot
open Telegram.Bot
open Telegram.Bot.Types


module internal Workflows =

  let deleteBotMessage (bot: ITelegramBotClient) : DeleteBotMessage =
    fun chatId messageId ->

      bot.DeleteMessageAsync((chatId.Value |> ChatId), messageId.Value)
