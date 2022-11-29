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

    let variableStore = Map.empty<string, double>

    let storeVar variable value = variableStore.Add (variable, value)
    let getVar variable = 
        try variableStore.[variable]
        with :? KeyNotFoundException -> 
            raise (execError ("Variable " + variable + " does not have an assigned value"))

    let outputStack = Stack<double>()
    let operatorStack = Stack<Token>()

    let calculate() =
        if outputStack.Count <> 0 then
            System.Diagnostics.Debug.WriteLine("output stack count = " + (string)outputStack.Count)
            let operator = operatorStack.Pop()
            let value = outputStack.Pop()
            match operator with
            | Token.Assign ->   failwith "Not Implemented"
            | Token.Indice ->   outputStack.Push(outputStack.Pop() ** value)
            | Token.Times ->    outputStack.Push(outputStack.Pop() * value)
            | Token.Divide ->   outputStack.Push(outputStack.Pop() / value)
            | Token.Plus ->     outputStack.Push(outputStack.Pop() + value)
            | Token.Minus ->    if outputStack.Count = 0 then outputStack.Push(0.0 - value)
                                else outputStack.Push(outputStack.Pop() - value)
            | _ ->              failwith "Invalid operator here"
            System.Diagnostics.Debug.WriteLine("outputStack.Peek = " + (string)(outputStack.Peek()))

    let shuntingYard (tokens: Token list) = 
        for token in tokens do
            match token with
            | Token.Number value -> 
                            outputStack.Push(value)
                            System.Diagnostics.Debug.WriteLine("Number {0} added to output stack", value)
            | Token.Variable value -> 
                            if operatorStack.Count <> 0 && operatorStack.Peek() <> Token.Assign then
                                outputStack.Push(getVar value)
                                System.Diagnostics.Debug.WriteLine("Variable {0} added to output stack", value)
            | Token.Plus -> 
                            while operatorStack.Count <> 0 && operatorStack.Peek() <> Token.L_Bracket && operatorStack.Peek() <> Token.Assign do
                                calculate()
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("+ added to operator stack")
            | Token.Minus -> 
                            while operatorStack.Count <> 0 && operatorStack.Peek() <> Token.L_Bracket && operatorStack.Peek() <> Token.Assign do
                                calculate()
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("- added to operator stack")
            | Token.Times -> 
                            while operatorStack.Count <> 0 && operatorStack.Peek() <> Token.L_Bracket && operatorStack.Peek() <> Token.Assign && operatorStack.Peek() <> Token.Plus do
                                calculate()
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("* added to operator stack")
            | Token.Divide ->
                            while operatorStack.Count <> 0 && operatorStack.Peek() <> Token.L_Bracket && operatorStack.Peek() <> Token.Assign && operatorStack.Peek() <> Token.Plus do
                                calculate()
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("/ added to operator stack")
            | Token.L_Bracket -> 
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("( added to operator stack")
            | Token.R_Bracket -> 
                            while operatorStack.Count <> 0 && operatorStack.Peek() <> Token.L_Bracket do
                                calculate()
                            System.Diagnostics.Debug.WriteLine("{0} removed from operator stack", operatorStack.Pop())
            | Token.Indice -> 
                            while operatorStack.Count <> 0 && (operatorStack.Peek() = Token.R_Bracket || operatorStack.Peek() = Token.Indice) do
                                calculate()
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("^ added to operator stack")
            | Token.Assign -> failwith "Not Implemented"
        while operatorStack.Count <> 0 do
            calculate()
        outputStack.Pop()
