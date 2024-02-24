namespace otsom.fs.Telegram.Bot

open Microsoft.Extensions.DependencyInjection
open Core
open Telegram.Bot
open otsom.fs.Extensions.DependencyInjection

[<RequireQualifiedAccess>]
module Startup =
  let addTelegramBotCore (services: IServiceCollection) =
    services
      .BuildSingleton<SendUserMessage, ITelegramBotClient>(Workflows.sendUserMessage)
      .BuildSingleton<ReplyToUserMessage, ITelegramBotClient>(Workflows.replyToUserMessage)