module Tests

open System.Threading.Tasks
open Xunit
open otsom.fs.Bot
open otsom.fs.Bot.Builders

type MockChat() =
  interface IChat with
    member _.Id = ChatId 1L

type ClickHandlersBuilderTests() =

  let chat = MockChat() :> IChat

  let click =
    { Id = ButtonClickId "1"
      MessageId = BotMessageId 1
      Data = [] }

  [<Fact>]
  member _.``Return and Zero return None``() = task {
    let h1 = clickHandlers { return () }
    let h2 = clickHandlers { () }

    let! r1 = h1 chat click
    let! r2 = h2 chat click

    Assert.Equal(None, r1)
    Assert.Equal(None, r2)
  }

  [<Fact>]
  member _.``and YieldFrom return the handler``() = task {
    let expected = Some()
    let handler: ClickHandler<IChat> = fun _ _ -> Task.FromResult(expected)

    let h1 = clickHandlers { handler }
    let h2 = clickHandlers { handler }

    let! r1 = h1 chat click
    let! r2 = h2 chat click

    Assert.Equal(expected, r1)
    Assert.Equal(expected, r2)
  }

  [<Fact>]
  member _.``Combine returns Some if first is Some``() = task {
    let h1: ClickHandler<IChat> = fun _ _ -> Task.FromResult(Some())
    let h2: ClickHandler<IChat> = fun _ _ -> Task.FromResult(None)

    let h = clickHandlers {
      h1
      h2
    }

    let! r = h chat click

    Assert.Equal(Some(), r)
  }

  [<Fact>]
  member _.``Combine returns Some if second is Some``() = task {
    let h1: ClickHandler<IChat> = fun _ _ -> Task.FromResult(None)
    let h2: ClickHandler<IChat> = fun _ _ -> Task.FromResult(Some())

    let h = clickHandlers {
      h1
      h2
    }

    let! r = h chat click

    Assert.Equal(Some(), r)
  }

  [<Fact>]
  member _.``Combine returns Some if both are Some``() = task {
    let h1: ClickHandler<IChat> = fun _ _ -> Task.FromResult(Some())
    let h2: ClickHandler<IChat> = fun _ _ -> Task.FromResult(Some())

    let h = clickHandlers {
      h1
      h2
    }

    let! r = h chat click

    Assert.Equal(Some(), r)
  }

  [<Fact>]
  member _.``Combine returns None if both are None``() = task {
    let h1: ClickHandler<IChat> = fun _ _ -> Task.FromResult(None)
    let h2: ClickHandler<IChat> = fun _ _ -> Task.FromResult(None)

    let h = clickHandlers {
      h1
      h2
    }

    let! r = h chat click

    Assert.Equal(None, r)
  }

  [<Fact>]
  member _.``Multiple yields combine correctly``() = task {
    let h1: ClickHandler<IChat> = fun _ _ -> Task.FromResult(None)
    let h2: ClickHandler<IChat> = fun _ _ -> Task.FromResult(None)
    let h3: ClickHandler<IChat> = fun _ _ -> Task.FromResult(Some())

    let h = clickHandlers {
      h1
      h2
      h3
    }

    let! r = h chat click

    Assert.Equal(Some(), r)
  }