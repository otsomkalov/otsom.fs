module otsom.fs.Bot.Telegram.Startup

#nowarn "20"

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
        member this.DeleteBotMessage = deleteBotMessage bot
        member this.SendKeyboard = sendChatKeyboard bot chatId

        member this.BuildBotMessageContext botMessageId =
          { new IBotMessageContext with
              member this.EditMessage = editBotMessage bot chatId botMessageId
              member this.EditMessageButtons = editBotMessageButtons bot chatId botMessageId } }

let addTelegramBot (cfg: IConfiguration) (services: IServiceCollection) =
  services
    .BuildSingleton<DeleteBotMessage, ITelegramBotClient>(deleteBotMessage)
    .BuildSingleton<SendChatMessage, ITelegramBotClient>(sendChatMessage)
    .BuildSingleton<SendChatKeyboard, ITelegramBotClient>(sendChatKeyboard)
    .BuildSingleton<EditBotMessage, ITelegramBotClient>(editBotMessage)
    .BuildSingleton<EditBotMessageButtons, ITelegramBotClient>(editBotMessageButtons)

  services.BuildSingleton<BuildChatContext, ITelegramBotClient>(buildChatContext)
