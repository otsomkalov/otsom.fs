[<RequireQualifiedAccess>]
module otsom.fs.Bot.Dialog.Startup

open Microsoft.Extensions.DependencyInjection
open otsom.fs.Extensions.DependencyInjection

let private getChatDialogService dialogRepo chatRepo dialogTemplateRepo : GetChatDialogService =
  fun (botService, resp) -> ChatDialogService(dialogRepo, chatRepo, dialogTemplateRepo, resp, botService)

let addDialogs (services: IServiceCollection) =
  services.BuildSingleton<GetChatDialogService, _, _, _>(getChatDialogService)