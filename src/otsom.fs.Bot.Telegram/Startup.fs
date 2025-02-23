module otsom.fs.Bot.Telegram.Startup

#nowarn "20"

open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Telegram.Bot
open otsom.fs.Bot
open otsom.fs.Bot.Telegram.Settings
open otsom.fs.Extensions.DependencyInjection

let private buildBotService (bot: ITelegramBotClient) : BuildBotService =
  fun chatId -> BotService(bot, chatId) :> IBotService

let addTelegramBot (cfg: IConfiguration) (services: IServiceCollection) =
  services.Configure<TelegramSettings>(cfg.GetSection TelegramSettings.SectionName)

  services.BuildSingleton<BuildBotService, ITelegramBotClient>(buildBotService)