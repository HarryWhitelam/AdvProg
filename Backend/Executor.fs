// Executor.fs
//
// Author:      Irie Railton
// Description: 

//*************************************************************************

namespace Backend

//*************************************************************************

module Executor =
    
    open System.Collections.Generic

    exception ExecError of string

    let mutable variableStore = Map.empty<string, string>

    let getVar variable = 
        try variableStore.[variable]
        with :? KeyNotFoundException -> 
            raise (ExecError ("Variable " + variable + " does not have an assigned value"))

    let outputStack = Stack<string>()
    let operatorStack = Stack<Token>()

    let calculate() =
        if outputStack.Count <> 0 then
            //System.Diagnostics.Debug.WriteLine("output stack count = " + (string)outputStack.Count)
            let operator = operatorStack.Pop()
            let mutable value = outputStack.Pop()
            match operator with
            //TODO: add check for value being a var not a number if it cant be found in varStore
            | Token.Assign ->   let value2 = outputStack.Pop()
                                variableStore.TryGetValue(value, &value) |> ignore
                                variableStore <- variableStore.Add(value2, value)

                                System.Diagnostics.Debug.WriteLine("varStore = " + (string)variableStore)
                                outputStack.Push(value2 + ":=" + value)
            | Token.Indice ->   outputStack.Push(string (double (outputStack.Pop()) ** double value))
            | Token.Times ->    outputStack.Push(string (double (outputStack.Pop()) * double value))
            | Token.Divide ->   outputStack.Push(string (double (outputStack.Pop()) / double value))
            | Token.Plus ->     outputStack.Push(string (double (outputStack.Pop()) + double value))
            | Token.Minus ->    if outputStack.Count = 0 then outputStack.Push(string (0.0 - double value))
                                else outputStack.Push(string (double (outputStack.Pop()) - double value))
            | _ ->              failwith "Invalid operator here"
            System.Diagnostics.Debug.WriteLine("outputStack.Peek = " + (string)(outputStack.Peek()))

    let shuntingYard (tokens: Token list) = 
        for token in tokens do
            match token with
            Token.Number value -> 
                            outputStack.Push(value)
                            System.Diagnostics.Debug.WriteLine("Number " + value + " added to output stack")
            | Token.Variable value -> 
                            outputStack.Push(value)
                            System.Diagnostics.Debug.WriteLine("Variable " + value + " added to output stack")
            | Token.Reserved value -> failwith "Not Implemented"
            | Token.Plus -> 
                            while operatorStack.Count <> 0 && operatorStack.Peek() <> Token.L_Bracket && operatorStack.Peek() <> Token.Assign do
                                calculate()
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("+ added to operator stack")
            //TODO: Fix -- issue
            | Token.Minus -> 
                            while operatorStack.Count <> 0 && operatorStack.Peek() <> Token.L_Bracket && operatorStack.Peek() <> Token.Assign do
                                calculate()
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("- added to operator stack")
            | Token.Times -> 
                            while operatorStack.Count <> 0 && operatorStack.Peek() <> Token.L_Bracket && operatorStack.Peek() <> Token.Assign && operatorStack.Peek() <> Token.Plus && operatorStack.Peek() <> Token.Minus do
                                calculate()
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("* added to operator stack")
            | Token.Divide ->
                            while operatorStack.Count <> 0 && operatorStack.Peek() <> Token.L_Bracket && operatorStack.Peek() <> Token.Assign && operatorStack.Peek() <> Token.Plus && operatorStack.Peek() <> Token.Minus do
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
            | Token.Assign -> 
                            while operatorStack.Count <> 0 && operatorStack.Peek() = Token.R_Bracket do
                                calculate()
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine(":= added to operator stack")
        while operatorStack.Count <> 0 do
            calculate()
        outputStack.Pop()
