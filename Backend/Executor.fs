namespace Backend

module Executor =
    
    open System.Collections.Generic

    let execError (reason:string) = System.Exception("Execution Error: " + reason)

    let variableStore = Map.empty<string, float>

    let storeVar variable value = variableStore.Add (variable, value)
    let getVar variable = 
        try variableStore.[variable]
        with :? KeyNotFoundException -> 
            raise (execError ("Variable " + variable + " does not have an assigned value"))

    let shuntingYard = 
        let outputQueue = Queue<string>()
        let operatorStack = Stack<Token>()
        0