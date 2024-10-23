namespace otsom.fs.Bot.Conversation.Repos

open System.Threading.Tasks
open otsom.fs.Bot.Core

type ILoadChat =
  abstract member LoadChat: ChatId -> Task<Chat>

type IUpdateChat =
  abstract member UpdateChat: Chat -> Task<unit>

type ILoadConversation =
  abstract member LoadConversation: ChatId -> Task<Conversation>

type ICreateConversation =
  abstract member CreateConversation: Conversation.New -> Task<unit>

type IUpdateConversation =
  abstract member UpdateConversation: Conversation -> Task<unit>

type ILoadConversationTemplate =
  abstract member LoadConversationTemplate: ConversationTemplateId -> Task<ConversationTemplate>

type IAskForData =
  abstract member AskForData: ChatId * string -> Task<BotMessageId>

type IEditMessage =
  abstract member EditMessage: ChatId -> BotMessageId -> string -> Task<unit>

type IDeleteMessage =
  abstract member DeleteMessage: ChatId -> MessageId -> Task<unit>

type IGetResource =
  abstract member GetResource: string -> Task<string>

type IGenerateId =
  abstract member GenerateId: unit -> string