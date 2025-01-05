module otsom.fs.Bot.Telegram.Startup

#nowarn "20"

open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Telegram.Bot
open otsom.fs.Bot
open otsom.fs.Extensions.DependencyInjection
open otsom.fs.Bot.Telegram.Workflows

let internal buildBotMessageContext bot chatId =
  fun botMessageId ->
    { new IBotMessageContext with
        member this.EditMessage = editBotMessage bot chatId botMessageId
        member this.EditMessageButtons = editBotMessageButtons bot chatId botMessageId }

let internal buildChatMessageContext bot chatId : BuildChatMessageContext =
  fun chatMessageId ->
    { new IChatMessageContext with
        member this.ReplyToMessage = replyToChatMessage bot chatId chatMessageId }

let internal buildChatContext bot : BuildChatContext =
  fun chatId ->
    { new IChatContext with
        member this.SendMessage = sendChatMessage bot chatId
        member this.DeleteBotMessage = deleteBotMessage bot
        member this.SendKeyboard = sendChatKeyboard bot chatId
        member this.SendMessageButtons = sendChatMessageButtons bot chatId
        member this.AskForReply = askForReply bot chatId

        member this.BuildBotMessageContext botMessageId =
          buildBotMessageContext bot chatId botMessageId

        member this.BuildChatMessageContext = buildChatMessageContext bot chatId }

let addTelegramBot (cfg: IConfiguration) (services: IServiceCollection) =
  services
    .BuildSingleton<SendChatMessage, ITelegramBotClient>(sendChatMessage)
    .BuildSingleton<SendChatKeyboard, ITelegramBotClient>(sendChatKeyboard)
    .BuildSingleton<SendChatMessageButtons, ITelegramBotClient>(sendChatMessageButtons)

    .BuildSingleton<AskChatForReply, ITelegramBotClient>(askForReply)

    .BuildSingleton<EditBotMessage, ITelegramBotClient>(editBotMessage)
    .BuildSingleton<EditBotMessageButtons, ITelegramBotClient>(editBotMessageButtons)

    .BuildSingleton<DeleteBotMessage, ITelegramBotClient>(deleteBotMessage)

  services.BuildSingleton<BuildChatContext, ITelegramBotClient>(buildChatContext)