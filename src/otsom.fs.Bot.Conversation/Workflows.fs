module otsom.fs.Bot.Conversation.Workflows

open System.Threading.Tasks
open otsom.fs.Bot.Conversation
open otsom.fs.Bot.Conversation.Repos
open otsom.fs.Bot.Core
open otsom.fs.Extensions.Operators
open otsom.fs.Extensions

module Chat =
  let startConversation
    (env: #IGenerateId & #ILoadChat & #ILoadConversationTemplate & #IAskForData & #IGetResource & #ICreateConversation & #IUpdateChat)
    : Chat.StartConversation =
    fun chatId templateId ->
      task {
        let! template = templateId |> env.LoadConversationTemplate

        let! botMessageId =
          env.GetResource template.FirstStep.Resource
          &|&> (fun resource -> env.AskForData(chatId, resource))

        let conversation: Conversation.New =
          { Id = env.GenerateId()
            TemplateId = templateId
            CurrentStepData =
              { BotMessageId = botMessageId
                ConversationStepId = template.FirstStep.Id } }

        do! env.CreateConversation conversation

        let! chat = env.LoadChat chatId

        let updatedChat =
          { chat with
              CurrentConversation = Some { ConversationId = conversation.Id } }

        do! env.UpdateChat updatedChat

        return ()
      }

  let private cleanupConversationMessages (env: #IDeleteMessage & #IGetResource) =
    fun chatId (conversation: Conversation.Ongoing) ->
      conversation.StepsData
      |> List.map (fun complete ->
        task {
          do! env.DeleteMessage chatId (MessageId.Bot complete.BotMessageId)
          do! env.DeleteMessage chatId (MessageId.Chat complete.ChatMessageId)
        })
      |> Task.WhenAll
      |> Task.ignore

  let private processStep (env: #IAskForData & #IGetResource & #IUpdateConversation) =
    fun (chat: Chat) (conversation: Conversation.Ongoing) (template: ConversationTemplate) (nextStepId: ConversationStepId) ->
      task {
        let nextStep = template.Steps |> List.find (fun s -> s.Id = nextStepId)

        let! botMessageId =
          env.GetResource nextStep.Resource
          &|&> (fun resource -> env.AskForData(chat.Id, resource))

        let updatedConversation =
          { conversation with
              CurrentStepData =
                { BotMessageId = botMessageId
                  ConversationStepId = nextStepId } }

        do! env.UpdateConversation(Conversation.Ongoing updatedConversation)

        return ()
      }

  let private finishConversation (env: #IEditMessage & #IGetResource & #IUpdateChat) =
    fun (chat: Chat) (conversation: Conversation.Ongoing) (step: ConversationStep) ->
      task {
        do! cleanupConversationMessages env chat.Id conversation

        let completedConversation = Conversation.Finished.fromOngoing conversation

        let updatedChat = { chat with CurrentConversation = None }

        do! env.UpdateChat updatedChat

        do!
          env.GetResource step.Resource
          &|&> env.EditMessage chat.Id conversation.CurrentStepData.BotMessageId
      }

  let private processConversation (env: #ILoadConversation & #ILoadConversationTemplate) =
    fun chat chatMessageId data ->
      function
      | New conversation ->
        conversation.TemplateId
        |> env.LoadConversationTemplate
        &|&> (fun template ->
          let currentStep =
            template.Steps |> List.find (fun s -> s.Id = conversation.CurrentStepData.ConversationStepId)

          match currentStep.Next with
          | Some nextStepId ->

            let ongoingConversion = Conversation.Ongoing.fromNew conversation chatMessageId data {BotMessageId =  }

            processStep env chat conversation template nextStepId
          | None ->
            finishConversation env chat conversation currentStep)
      | Ongoing conversation ->
        let ongoingConversationStep =
          OngoingConversationStep.fromCurrentStepData conversation.CurrentStepData chatMessageId data

        let updatedConversationData =
          conversation.StepsData @ [ongoingConversationStep]

        conversation.TemplateId
        |> env.LoadConversationTemplate
        &|&> (fun template ->
          let currentStep =
            template.Steps |> List.find (fun s -> s.Id = conversation.CurrentStepData.ConversationStepId)

          match currentStep.Next with
          | Some nextStepId ->
            processStep env chat conversation template nextStepId
          | None ->
            finishConversation env chat conversation currentStep)
      | Finished c ->
        failwith "Trying to run a completed conversation"

  let runCurrentConversation
    (env: #ILoadChat & #ILoadConversation & #ILoadConversationTemplate & #IAskForData & #IGetResource & #IUpdateConversation)
    (chatMessageId: ChatMessageId)
    : Chat.RunCurrentConversation =
    fun chatId data ->
      chatId
      |> env.LoadChat
      &|&> (fun chat ->
        chat.CurrentConversation
        |> Option.map (fun currentConversation ->
          currentConversation.ConversationId
          |> env.LoadConversation
          &|&> (processConversation env chat chatMessageId data))
        |> Option.defaultValue(Task.FromResult()))

  let cancelCurrentConversation
    (env: #ILoadChat & #ILoadConversation & #ILoadConversationTemplate & #IAskForData & #IGetResource & #IUpdateChat)
    : Chat.CancelCurrentConversation =
    fun chatId ->
      task {
        let! chat = env.LoadChat chatId

        match chat.CurrentConversation with
        | Some currentConversation ->
          let! conversation = env.LoadConversation currentConversation.ConversationId

          match conversation with
          | Finished _ -> failwith "Cannot cancel a finished conversation"
          | Ongoing conversation -> ()
          | New conversation -> ()

          // do! cleanupConversationMessages env chatId conversation

          let updatedChat = { chat with CurrentConversation = None }

          do! env.UpdateChat updatedChat
        | None -> Task.FromResult()
      }
