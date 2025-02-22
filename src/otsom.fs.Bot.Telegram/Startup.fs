module otsom.fs.Bot.Telegram.Startup

#nowarn "20"

open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open otsom.fs.Bot
open otsom.fs.Bot.Telegram.Settings

let addTelegramBot (cfg: IConfiguration) (services: IServiceCollection) =
  services.Configure<TelegramSettings>(cfg.GetSection TelegramSettings.SectionName)

  services.AddSingleton<IBotService, BotService>()