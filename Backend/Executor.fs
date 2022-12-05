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
            System.Diagnostics.Debug.WriteLine("output stack count = " + (string)outputStack.Count)
            let operator = operatorStack.Pop()
            let value = outputStack.Pop()
            match operator with
            | Token.Assign ->   let value2 = outputStack.Pop()
                                Map.add (value2, value) variableStore |> ignore
                                System.Diagnostics.Debug.WriteLine("varStore = " + (string)variableStore)
                                outputStack.Push(value2 + ":=" + value)
            | Token.Indice ->   outputStack.Push(string (double (outputStack.Pop()) ** double value))
            | Token.Times ->    outputStack.Push(string (double (outputStack.Pop()) * double value))
            | Token.Divide ->   outputStack.Push(string (double (outputStack.Pop()) / double value))
            | Token.Plus ->     outputStack.Push(string (double (outputStack.Pop()) + double value))
            | Token.Minus ->    if outputStack.Count = 0 then outputStack.Push(string (0.0 - double value))
                                else outputStack.Push(string (double (outputStack.Pop()) - double value))
            | _ ->              failwith "Invalid operator here"
            //System.Diagnostics.Debug.WriteLine("outputStack.Peek = " + (string)(outputStack.Peek()))

    let shuntingYard (tokens: Token list) = 
        for token in tokens do
            match token with
            | Token.Number value -> 
                            outputStack.Push(value)
                            System.Diagnostics.Debug.WriteLine("Number {0} added to output stack", value)
            | Token.Variable value -> 
                            outputStack.Push(value)
                            System.Diagnostics.Debug.WriteLine("Variable {0} added to output stack", value)
            | Token.Reserved value -> failwith "Not Implemented"
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
            | Token.Assign -> 
                            while operatorStack.Count <> 0 && operatorStack.Peek() = Token.R_Bracket do
                                calculate()
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine(":= added to operator stack")
        while operatorStack.Count <> 0 do
            calculate()
        outputStack.Pop()
