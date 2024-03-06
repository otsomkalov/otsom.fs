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
      .BuildSingleton<SendUserMessageButtons, ITelegramBotClient>(Workflows.sendUserMessageButtons)
      .BuildSingleton<SendUserKeyboard, ITelegramBotClient>(Workflows.sendUserKeyboard)
      .BuildSingleton<ReplyUserWithKeyboard, ITelegramBotClient>(Workflows.replyUserWithKeyboard)
      .BuildSingleton<AskUserForReply, ITelegramBotClient>(Workflows.askUserForReply)
      .BuildSingleton<EditBotMessage, ITelegramBotClient>(Workflows.editUserMessage)
      .BuildSingleton<EditBotMessageButtons, ITelegramBotClient>(Workflows.editUserMessageButtons)
