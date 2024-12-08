module otsom.fs.Bot.Telegram.Startup

open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Telegram.Bot
open otsom.fs.Bot
open otsom.fs.Extensions.DependencyInjection
open otsom.fs.Bot.Telegram.Workflows

let internal buildChatContext (bot: ITelegramBotClient) : BuildChatContext =
  fun chatId ->
    { new IChatContext with
        member this.SendMessage = sendChatMessage bot chatId
        member this.DeleteBotMessage = deleteBotMessage bot }

let addTelegramBot (cfg: IConfiguration) (services: IServiceCollection) =
  services
    .BuildSingleton<DeleteBotMessage, ITelegramBotClient>(deleteBotMessage)
    .BuildSingleton<SendChatMessage, ITelegramBotClient>(sendChatMessage)

  services.BuildSingleton<BuildChatContext, ITelegramBotClient>(buildChatContext)
