namespace otsom.fs.Telegram.Bot.Core

open Microsoft.Extensions.DependencyInjection
open Telegram.Bot
open otsom.fs.Extensions.DependencyInjection

[<RequireQualifiedAccess>]
module Startup =
  let addTelegramBotCore (services: IServiceCollection) =
    services
      .BuildSingleton<SendUserMessage, ITelegramBotClient>(Workflows.sendUserMessage)
      .BuildSingleton<ReplyToUserMessage, ITelegramBotClient>(Workflows.replyToUserMessage)