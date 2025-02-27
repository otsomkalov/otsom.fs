[<AutoOpen>]
module otsom.fs.Extensions.Operators

open System.Diagnostics

/// Task.map
[<StackTraceHidden>]
let (&|>) arg func = Task.map func arg

/// Task.tap
[<StackTraceHidden>]
let (&|!) arg func = Task.tap func arg

/// Task.bind
[<StackTraceHidden>]
let (&|&>) arg func = Task.bind func arg

/// Result.map
[<StackTraceHidden>]
let (=|>) arg func = Result.map func arg

/// Result.bind
[<StackTraceHidden>]
let (=|=>) arg func = Result.bind func arg

/// Result.taskMap
[<StackTraceHidden>]
let (=|&>) arg func = Result.taskMap func arg

/// TaskResult.map
[<StackTraceHidden>]
let (&=|>) arg func = TaskResult.map func arg

/// TaskResult.taskMap
[<StackTraceHidden>]
let (&=|&>) arg func = TaskResult.taskMap func arg

/// TaskResult.taskTap
[<StackTraceHidden>]
let (&=|&!) arg func = TaskResult.taskTap func arg
