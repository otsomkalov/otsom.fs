module Tests

open System.Threading.Tasks
open Xunit
open FsUnit.Xunit
open otsom.fs.Bot
open otsom.fs.Bot.Builders

type MockChat() =
  interface IChat with
    member _.Id = ChatId 1L

type MockMessage() =
  interface IMessage with
    member _.Id = ChatMessageId 1

type ClickHandlersBuilderTests() =

  let chat = MockChat() :> IChat

  let click =
    { Id = ButtonClickId "1"
      MessageId = BotMessageId 1
      Data = [] }

  [<Fact>]
  member _.``Return and Zero return None``() = task {
    let handler1 = clickHandlers { return () }
    let handler2 = clickHandlers { () }

    let! result1 = handler1 chat click
    let! result2 = handler2 chat click

    result1 |> should equal None
    result2 |> should equal None
  }

  [<Fact>]
  member _.``and YieldFrom return the handler``() = task {
    let expected = Some()
    let handler: ClickHandler<IChat> = fun _ _ -> Task.FromResult(expected)

    let handler1 = clickHandlers { handler }
    let handler2 = clickHandlers { handler }

    let! result1 = handler1 chat click
    let! result2 = handler2 chat click

    result1 |> should equal expected
    result2 |> should equal expected
  }

  [<Fact>]
  member _.``Combine returns Some if first is Some``() = task {
    let handler1: ClickHandler<IChat> = fun _ _ -> Task.FromResult(Some())
    let handler2: ClickHandler<IChat> = fun _ _ -> Task.FromResult(None)

    let combinedHandler = clickHandlers {
      handler1
      handler2
    }

    let! result = combinedHandler chat click

    result |> should equal (Some())
  }

  [<Fact>]
  member _.``Combine returns Some if second is Some``() = task {
    let handler1: ClickHandler<IChat> = fun _ _ -> Task.FromResult(None)
    let handler2: ClickHandler<IChat> = fun _ _ -> Task.FromResult(Some())

    let combinedHandler = clickHandlers {
      handler1
      handler2
    }

    let! result = combinedHandler chat click

    result |> should equal (Some())
  }

  [<Fact>]
  member _.``Combine returns Some if both are Some``() = task {
    let handler1: ClickHandler<IChat> = fun _ _ -> Task.FromResult(Some())
    let handler2: ClickHandler<IChat> = fun _ _ -> Task.FromResult(Some())

    let combinedHandler = clickHandlers {
      handler1
      handler2
    }

    let! result = combinedHandler chat click

    result |> should equal (Some())
  }

  [<Fact>]
  member _.``Combine returns None if both are None``() = task {
    let handler1: ClickHandler<IChat> = fun _ _ -> Task.FromResult(None)
    let handler2: ClickHandler<IChat> = fun _ _ -> Task.FromResult(None)

    let combinedHandler = clickHandlers {
      handler1
      handler2
    }

    let! result = combinedHandler chat click

    result |> should equal None
  }

  [<Fact>]
  member _.``Multiple yields combine correctly``() = task {
    let handler1: ClickHandler<IChat> = fun _ _ -> Task.FromResult(None)
    let handler2: ClickHandler<IChat> = fun _ _ -> Task.FromResult(None)
    let handler3: ClickHandler<IChat> = fun _ _ -> Task.FromResult(Some())

    let combinedHandler = clickHandlers {
      handler1
      handler2
      handler3
    }

    let! result = combinedHandler chat click

    result |> should equal (Some())
  }

type MessageHandlersBuilderTests() =

  let chat = MockChat() :> IChat
  let message = MockMessage() :> IMessage

  [<Fact>]
  member _.``Return and Zero return None``() = task {
    let handler1 = messageHandlers { return () }
    let handler2 = messageHandlers { () }

    let! result1 = handler1 chat message
    let! result2 = handler2 chat message

    result1 |> should equal None
    result2 |> should equal None
  }

  [<Fact>]
  member _.``Yield and YieldFrom return the handler``() = task {
    let expected = Some()
    let handler: MessageHandler<IChat, IMessage> = fun _ _ -> Task.FromResult(expected)

    let handler1 = messageHandlers { handler }
    let handler2 = messageHandlers { yield! handler }

    let! result1 = handler1 chat message
    let! result2 = handler2 chat message

    result1 |> should equal expected
    result2 |> should equal expected
  }

  [<Fact>]
  member _.``Combine returns Some if first is Some``() = task {
    let handler1: MessageHandler<IChat, IMessage> = fun _ _ -> Task.FromResult(Some())
    let handler2: MessageHandler<IChat, IMessage> = fun _ _ -> Task.FromResult(None)

    let combinedHandler = messageHandlers {
      handler1
      handler2
    }

    let! result = combinedHandler chat message

    result |> should equal (Some())
  }

  [<Fact>]
  member _.``Combine returns Some if second is Some``() = task {
    let handler1: MessageHandler<IChat, IMessage> = fun _ _ -> Task.FromResult(None)
    let handler2: MessageHandler<IChat, IMessage> = fun _ _ -> Task.FromResult(Some())

    let combinedHandler = messageHandlers {
      handler1
      handler2
    }

    let! result = combinedHandler chat message

    result |> should equal (Some())
  }

  [<Fact>]
  member _.``Combine returns Some if both are Some``() = task {
    let handler1: MessageHandler<IChat, IMessage> = fun _ _ -> Task.FromResult(Some())
    let handler2: MessageHandler<IChat, IMessage> = fun _ _ -> Task.FromResult(Some())

    let combinedHandler = messageHandlers {
      handler1
      handler2
    }

    let! result = combinedHandler chat message

    result |> should equal (Some())
  }

  [<Fact>]
  member _.``Combine returns None if both are None``() = task {
    let handler1: MessageHandler<IChat, IMessage> = fun _ _ -> Task.FromResult(None)
    let handler2: MessageHandler<IChat, IMessage> = fun _ _ -> Task.FromResult(None)

    let combinedHandler = messageHandlers {
      handler1
      handler2
    }

    let! result = combinedHandler chat message

    result |> should equal None
  }

  [<Fact>]
  member _.``Multiple yields combine correctly``() = task {
    let handler1: MessageHandler<IChat, IMessage> = fun _ _ -> Task.FromResult(None)
    let handler2: MessageHandler<IChat, IMessage> = fun _ _ -> Task.FromResult(None)
    let handler3: MessageHandler<IChat, IMessage> = fun _ _ -> Task.FromResult(Some())

    let combinedHandler = messageHandlers {
      handler1
      handler2
      handler3
    }

    let! result = combinedHandler chat message

    result |> should equal (Some())
  }