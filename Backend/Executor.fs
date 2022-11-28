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

    let outputQueue = Queue()
    let operatorStack = Stack<Token>()

    let calculate =
        if outputQueue.Count <> 0 then
            let value = outputQueue.Dequeue()
            let value2 = outputQueue.Dequeue()
            match operatorStack.Pop() with
            | Token.Plus ->     outputQueue.Enqueue(value + value2)
            | Token.Minus -> failwith "Not Implemented"
            | Token.Times ->    outputQueue.Enqueue(value * value2)
            | Token.Divide ->   outputQueue.Enqueue(value / value2)
            | Token.Indice -> failwith "Not Implemented"
            | Token.Assign -> failwith "Not Implemented"
            | _ -> failwith "Invalid Token Here"

    let shuntingYard (tokens: Token list) = 
        for token in tokens do
            match token with
            | Token.Number value -> 
                            outputQueue.Enqueue(value)
                            System.Console.WriteLine("Number {0} added to queue", value)
            | Token.Variable value -> 
                            if operatorStack.Count <> 0 && operatorStack.Peek() <> Token.Assign then
                                outputQueue.Enqueue(getVar value)
                                System.Console.WriteLine("Variable {0} added to queue", value)
            | Token.Plus -> 
                            //check this
                            while operatorStack.Count <> 0 && (operatorStack.Peek() <> Token.L_Bracket || operatorStack.Peek() <> Token.Assign) do
                                //outputQueue.Enqueue(operatorStack.Pop())
                                //(containsToken, top) <- operatorStack.TryPeek()
                                calculate
                            operatorStack.Push(token)
                            System.Console.WriteLine("+ added to stack")
            | Token.Minus -> failwith "Not Implemented"
            | Token.Times -> 
                            while operatorStack.Count <> 0 && (operatorStack.Peek() <> Token.L_Bracket || operatorStack.Peek() <> Token.Assign || operatorStack.Peek() <> Token.Plus) do
                                calculate
                            operatorStack.Push(token)
                            System.Console.WriteLine("* added to stack")
            | Token.Divide ->
                            while operatorStack.Count <> 0 && (operatorStack.Peek() <> Token.L_Bracket || operatorStack.Peek() <> Token.Assign || operatorStack.Peek() <> Token.Plus) do
                                calculate
                            operatorStack.Push(token)
                            System.Console.WriteLine("/ added to stack")
            | Token.L_Bracket -> 
                            operatorStack.Push(token)
                            System.Console.WriteLine("( added to stack")
            | Token.R_Bracket -> 
                            while operatorStack.Count <> 0 && (operatorStack.Peek() <> Token.L_Bracket) do
                                calculate
                            operatorStack.Pop() |> ignore
                            System.Console.WriteLine("( removed from stack")
            | Token.Indice -> failwith "Not Implemented"
            | Token.Assign -> failwith "Not Implemented"
        while operatorStack.Count <> 0 do
            System.Console.WriteLine("{0}, {0}", operatorStack.Peek(), outputQueue.Peek())
            calculate
        outputQueue.Dequeue()
