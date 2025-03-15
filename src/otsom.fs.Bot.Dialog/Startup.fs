module otsom.fs.Bot.Dialog.Startup

open Microsoft.Extensions.DependencyInjection
open otsom.fs.Bot
open otsom.fs.Bot.Dialog.Workflows
open otsom.fs.Extensions.DependencyInjection
open otsom.fs.Resources

let private getChatDialogService dialogRepo chatRepo dialogTemplateRepo : GetChatDialogService =
  fun (botService: IBotService, resp: IResourceProvider) -> ChatDialogService(dialogRepo, chatRepo, dialogTemplateRepo, resp, botService)

let addDialogs (services: IServiceCollection) =
  services.BuildSingleton<GetChatDialogService, _, _, _>(getChatDialogService)