namespace otsom.fs.Bot.Telegram

open System.Text.RegularExpressions
open Telegram.Bot.Types
open Telegram.Bot.Types.Enums
open otsom.fs.Extensions

module Helpers =
  let internal escapeMarkdownString (str: string) =
    Regex.Replace(str, "([\(\)`\.#\-!+=&\?])", "\$1")

  let (|Message|_|) (update: Update) =
    update
    |> Option.someIf (fun u -> u.Type = UpdateType.Message)
    |> Option.map (_.Message)
