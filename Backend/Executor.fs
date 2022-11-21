// Executor.fs
//
// Author:      Irie Railton
// Description: 

//*************************************************************************

namespace Backend

//*************************************************************************

module Executor =
    
    open System.Collections.Generic

    let execError (reason:string) = System.Exception("Execution Error: " + reason)

    let variableStore = Map.empty<string, float>

    let storeVar variable value = variableStore.Add (variable, value)
    let getVar variable = 
        try variableStore.[variable]
        with :? KeyNotFoundException -> 
            raise (execError ("Variable " + variable + " does not have an assigned value"))

    let shuntingYard (tokens: Token list) = 
        let outputQueue = Queue()
        let operatorStack = Stack<Token>()

        for token in tokens do
            match token with
            | Token.Number value -> 
                            outputQueue.Enqueue(Token.Number value)
            | Token.Variable value -> 
                            outputQueue.Enqueue(Token.Variable value)
            | Token.Plus -> 
                            //check this
                            let containsToken, top = operatorStack.TryPeek()
                            while containsToken = true && top = Token.Times || top = Token.Divide || top = Token.Indice || top = Token.Assign do
                                outputQueue.Enqueue(operatorStack.Pop())
                                (containsToken, top) <- operatorStack.TryPeek()
                            operatorStack.Push(Token.Plus)
            | Token.Minus -> failwith "Not Implemented"
            | Token.Times -> failwith "Not Implemented"
            | Token.Divide -> failwith "Not Implemented"
            | Token.L_Bracket -> failwith "Not Implemented"
            | Token.R_Bracket -> failwith "Not Implemented"
            | Token.Indice -> failwith "Not Implemented"
            | Token.Assign -> failwith "Not Implemented"
