module otsom.fs.Bot.Telegram.Startup

open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Options
open Telegram.Bot
open otsom.fs.Bot
open otsom.fs.Bot.Telegram.Settings
open otsom.fs.Extensions.DependencyInjection

let addTelegramBot (cfg: IConfiguration) (services: IServiceCollection) =
  services
    .Configure<TelegramSettings>(cfg.GetSection(TelegramSettings.SectionName))
    .BuildSingleton<TelegramSettings, IOptions<TelegramSettings>>(_.Value)

  services.BuildSingleton<ITelegramBotClient, TelegramSettings>(fun s ->
    TelegramBotClientOptions(s.Token, s.ApiUrl) |> TelegramBotClient :> ITelegramBotClient)

  services
    .BuildSingleton<SendChatMessage, _>(Workflows.sendUserMessage)
