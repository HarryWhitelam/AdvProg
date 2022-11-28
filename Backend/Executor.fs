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

    let outputStack = Stack<float>()
    let operatorStack = Stack<Token>()

    let calculate() =
        if outputStack.Count <> 0 then
            let value = outputStack.Pop()
            let value2 = outputStack.Pop()
            System.Diagnostics.Debug.WriteLine("value = {0}, value2 = {1}", value, value2)
            match operatorStack.Pop() with
            | Token.Indice -> failwith "Not Implemented"
            | Token.Assign -> failwith "Not Implemented"
            | Token.Times ->    outputStack.Push(value * value2)
            | Token.Divide ->   outputStack.Push(value / value2)
            | Token.Plus ->     outputStack.Push(value + value2)
            | Token.Minus -> failwith "Not Implemented"
            | _ -> failwith "Invalid Token Here"
            System.Diagnostics.Debug.WriteLine(outputStack.Peek())

    let shuntingYard (tokens: Token list) = 
        for token in tokens do
            match token with
            | Token.Number value -> 
                            outputStack.Push(value)
                            System.Diagnostics.Debug.WriteLine("Number {0} added to queue", value)
            | Token.Variable value -> 
                            if operatorStack.Count <> 0 && operatorStack.Peek() <> Token.Assign then
                                outputStack.Push(getVar value)
                                System.Diagnostics.Debug.WriteLine("Variable {0} added to queue", value)
            | Token.Plus -> 
                            //check this
                            while operatorStack.Count <> 0 && operatorStack.Peek() <> Token.L_Bracket && operatorStack.Peek() <> Token.Assign do
                                System.Diagnostics.Debug.WriteLine(operatorStack.Peek())
                                calculate()
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("+ added to stack")
            | Token.Minus -> failwith "Not Implemented"
            | Token.Times -> 
                            while operatorStack.Count <> 0 && operatorStack.Peek() <> Token.L_Bracket && operatorStack.Peek() <> Token.Assign && operatorStack.Peek() <> Token.Plus do
                                calculate()
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("* added to stack")
            | Token.Divide ->
                            while operatorStack.Count <> 0 && operatorStack.Peek() <> Token.L_Bracket && operatorStack.Peek() <> Token.Assign && operatorStack.Peek() <> Token.Plus do
                                calculate()
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("/ added to stack")
            | Token.L_Bracket -> 
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("( added to stack")
            | Token.R_Bracket -> 
                            while operatorStack.Count <> 0 && operatorStack.Peek() <> Token.L_Bracket do
                                calculate()
                            System.Diagnostics.Debug.WriteLine("{0} removed from stack", operatorStack.Pop())
            | Token.Indice -> failwith "Not Implemented"
            | Token.Assign -> failwith "Not Implemented"
        while operatorStack.Count <> 0 do
            System.Diagnostics.Debug.WriteLine("{0}, {1}", operatorStack.Peek(), outputStack.Peek())
            calculate()
        outputStack.Pop()
