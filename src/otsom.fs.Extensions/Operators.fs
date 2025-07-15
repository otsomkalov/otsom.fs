[<AutoOpen>]
module otsom.fs.Extensions.Operators

open System.Diagnostics

/// Task.map
[<StackTraceHidden>]
let inline (&|>) arg ([<InlineIfLambda>] func) = Task.map func arg

/// Task.tap
[<StackTraceHidden>]
let inline (&|!) arg ([<InlineIfLambda>] func) = Task.tap func arg

/// Task.bind
[<StackTraceHidden>]
let inline (&|&>) arg ([<InlineIfLambda>] func) = Task.bind func arg

/// Result.map
[<StackTraceHidden>]
let inline (=|>) arg ([<InlineIfLambda>] func) = Result.map func arg

/// Result.bind
[<StackTraceHidden>]
let inline (=|=>) arg ([<InlineIfLambda>] func) = Result.bind func arg

/// Result.taskMap
[<StackTraceHidden>]
let inline (=|&>) arg ([<InlineIfLambda>] func) = Result.taskMap func arg

/// TaskResult.map
[<StackTraceHidden>]
let inline (&=|>) arg ([<InlineIfLambda>] func) = TaskResult.map func arg

/// TaskResult.taskMap
[<StackTraceHidden>]
let inline (&=|&>) arg ([<InlineIfLambda>] func) = TaskResult.taskMap func arg

/// TaskResult.taskTap
[<StackTraceHidden>]
let inline (&=|&!) arg ([<InlineIfLambda>] func) = TaskResult.taskTap func arg