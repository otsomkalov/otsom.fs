[<AutoOpen>]
module otsom.fs.Extensions.Operators

let (!>) = Task.map
let (!!>) = Task.bind
let (=>) = Result.map
let (==>) = Result.bind
let (!@>) = TaskResult.map