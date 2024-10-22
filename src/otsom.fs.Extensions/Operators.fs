[<AutoOpen>]
module otsom.fs.Extensions.Operators

/// Task.map
let (&|>) arg func = Task.map func arg

/// Task.tap
let (&|!) arg func = Task.tap func arg

/// Task.bind
let (&|&>) arg func = Task.bind func arg

/// Result.map
let (=|>) arg func = Result.map func arg

/// Result.taskMap
let (=|&>) arg func = Result.taskMap func arg

/// TaskResult.map
let (&=|>) arg func = TaskResult.map func arg

/// TaskResult.taskMap
let (&=|&>) arg func = TaskResult.taskMap func arg

/// TaskResult.taskTap
let (&=|&!) arg func = TaskResult.taskTap func arg
