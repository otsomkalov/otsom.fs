module otsom.fs.Bot.Conversation.Tests.Chat

open Xunit
open FsUnit.Xunit
open otsom.fs.Bot.Conversation.Repos
open otsom.fs.Bot.Conversation.Workflows
open Moq

module StartConversation =

  type IStartConversationEnv =
    inherit IGenerateId
    inherit ILoadChat
    inherit ILoadConversationTemplate
    inherit IAskForData
    inherit IGetResource
    inherit ICreateConversation
    inherit IUpdateChat

  let private getMock () =
    let mock = Mock<IStartConversationEnv>()

    mock.Setup(fun m -> m.GenerateId()).Returns(Mocks.Conversation.id)

    mock.Setup(fun m -> m.LoadChat(Mocks.chatId)).ReturnsAsync(Mocks.chat)

    mock
      .Setup(fun m -> m.LoadConversationTemplate(Mocks.templateId))
      .ReturnsAsync(Mocks.template)

    mock
      .Setup(fun m -> m.AskForData(Mocks.chatId, Mocks.resourceValue))
      .ReturnsAsync(Mocks.botMessageId)

    mock
      .Setup(fun m -> m.CreateConversation(Mocks.Conversation.new'))
      .ReturnsAsync(())

    mock

  [<Fact>]
  let ``sends the message, saves new conversation and updates chat's current conversation to it `` () =
    let env = getMock ()

    env.Setup(fun m -> m.GetResource(Mocks.templateFirstStepResource))
      .ReturnsAsync(Mocks.resourceValue)

    let sut = Chat.startConversation env.Object

    task {
      let expectedChat =
        { Mocks.chat with
            CurrentConversation = Some({ ConversationId = Mocks.Conversation.id }) }

      do! sut Mocks.chatId Mocks.templateId

      env.Verify(fun m -> m.GenerateId())
      env.Verify(fun m -> m.LoadConversationTemplate(Mocks.templateId))
      env.Verify(fun m -> m.GetResource(Mocks.templateFirstStepResource))
      env.Verify(fun m -> m.AskForData(Mocks.chatId, Mocks.resourceValue))
      env.Verify(fun m -> m.LoadChat(Mocks.chatId))
      env.Verify(fun m -> m.CreateConversation(Mocks.Conversation.new'))
      env.Verify(fun m -> m.UpdateChat(expectedChat))
    }

module RunCurrentConversation =
  type IRunCurrentConversationEnv =
    inherit ILoadChat
    inherit ILoadConversation

  let getEnv() =
    let env = Mock<IRunCurrentConversationEnv>()

    env.Setup(fun m -> m.LoadChat(Mocks.chatId))
      .ReturnsAsync(Mocks.chat)

    env.Setup(fun m -> m.LoadConversation(Mocks.Conversation.id))
      .ReturnsAsync(Mocks.Conversation.new')

    env

  [<Fact>]
  let ``q``()  =
    let env = getEnv()

    let sut = Chat.runCurrentConversation env.Object