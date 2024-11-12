namespace otsom.fs.Bot.Telegram.Settings

[<CLIMutable>]
type internal TelegramSettings =
  { Token: string
    ApiUrl: string }

  static member SectionName = "Telegram"
