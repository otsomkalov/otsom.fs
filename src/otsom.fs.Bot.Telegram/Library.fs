namespace otsom.fs.Bot.Telegram

open System.Threading.Tasks
open Telegram.Bot
open Telegram.Bot.Types.Enums
open Telegram.Bot.Types.ReplyMarkups
open otsom.fs.Extensions
open otsom.fs.Bot

module internal Workflows =
  let sendChatMessage (bot: ITelegramBotClient) : SendChatMessage =
    fun chatId text ->
      ()