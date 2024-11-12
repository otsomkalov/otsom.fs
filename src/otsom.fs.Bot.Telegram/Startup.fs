module otsom.fs.Bot.Telegram.Startup

open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Telegram.Bot
open otsom.fs.Bot
open otsom.fs.Extensions.DependencyInjection

let addTelegramBot (cfg: IConfiguration) (services: IServiceCollection) =
  services
    .BuildSingleton<DeleteBotMessage, ITelegramBotClient>(Workflows.deleteBotMessage)
