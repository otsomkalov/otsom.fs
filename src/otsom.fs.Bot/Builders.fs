module otsom.fs.Bot.Builders

open System.Threading.Tasks
open otsom.fs.Bot

type ClickHandlersBuilder() =
  member _.Return(()) : ClickHandler<'chat> = fun _ _ -> Task.FromResult(None)
  member _.Zero() : ClickHandler<'chat> = fun _ _ -> Task.FromResult(None)

  member _.Yield(f: ClickHandler<'chat>) = f
  member _.YieldFrom(f: ClickHandler<'chat>) = f

  member _.Combine(h1: ClickHandler<'chat>, h2: ClickHandler<'chat>) : ClickHandler<'chat> =
    fun chat btnClick -> task {
      let! v1 = h1 chat btnClick
      and! v2 = h2 chat btnClick

      match v1, v2 with
      | Some(), _ -> return Some()
      | _, Some() -> return Some()
      | _ -> return None
    }

  member _.Delay(f: unit -> ClickHandler<'chat>) = f ()
  member _.Run(f: ClickHandler<'chat>) = f

let clickHandlers = ClickHandlersBuilder()

type MessageHandlersBuilder() =
  member _.Return(()) : MessageHandler<'chat, 'msg> = fun _ _ -> Task.FromResult(None)
  member _.Zero() : MessageHandler<'chat, 'msg> = fun _ _ -> Task.FromResult(None)

  member _.Yield(f: MessageHandler<'chat, 'msg>) = f
  member _.YieldFrom(f: MessageHandler<'chat, 'msg>) = f

  member _.Combine(h1: MessageHandler<'chat, 'msg>, h2: MessageHandler<'chat, 'msg>) : MessageHandler<'chat, 'msg> =
    fun chat msg -> task {
      let! v1 = h1 chat msg
      and! v2 = h2 chat msg

      match v1, v2 with
      | Some(), _ -> return Some()
      | _, Some() -> return Some()
      | _ -> return None
    }

  member _.Delay(f: unit -> MessageHandler<'chat, 'msg>) = f ()
  member _.Run(f: MessageHandler<'chat, 'msg>) = f

let messageHandlers = MessageHandlersBuilder()