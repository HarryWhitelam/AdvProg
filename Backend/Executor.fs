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
    let outputStack = Stack<string>()
    let operatorStack = Stack<Token>()

    let replaceVar (var :byref<string>) =
        try double var |> ignore
        with :? System.FormatException -> 
            if variableStore.TryGetValue(var, &var) = false then
                raise (ExecError ($"{var} does not have an assigned value"))

    let rec calculate() =
        let operator = operatorStack.Pop()
        let mutable value = outputStack.Pop()
        if operator = Token.Minus && (outputStack.Count = 0 || (operatorStack.Count <> 0 && operatorStack.Peek() = Token.L_Bracket)) then 
            replaceVar &value
            outputStack.Push(string (0.0 - double value))
        else if operator = Token.Assign then
            let mutable value2 = outputStack.Pop()        
            try double value |> ignore
                variableStore <- variableStore.Add(value2, value)
            with :? System.FormatException -> 
                if variableStore.TryGetValue(value, &value) then variableStore <- variableStore.Add(value2, value)
                else raise (ExecError ($"{value} is not a number or a stored variable"))                            
            System.Diagnostics.Debug.WriteLine("varStore = " + (string)variableStore)
            outputStack.Push(value2 + ":=" + value)
        else
            let mutable value2 = outputStack.Pop()
            replaceVar &value
            replaceVar &value2
            match operator with
            | Token.Indice ->   outputStack.Push(string (double value2 ** double value))
            | Token.Times ->    outputStack.Push(string (double value2 * double value))
            | Token.Divide ->   outputStack.Push(string (double value2 / double value))
            | Token.Plus ->     outputStack.Push(string (double value2 + double value))
            | Token.Minus ->    outputStack.Push(string (double value2 - double value))
            | _ ->              failwith "Invalid operator here"
        //System.Diagnostics.Debug.WriteLine("outputStack.Peek = " + (string)(outputStack.Peek()))

    let shuntingYard (tokens: Token list) = 
        for token in tokens do
            match token with
            | Token.Number value -> 
                            outputStack.Push(value)
                            System.Diagnostics.Debug.WriteLine("Number " + value + " added to output stack")
            | Token.Variable value -> 
                            if(tokens.Length = 1) then 
                                let mutable x = value
                                replaceVar &x
                                outputStack.Push(x)
                            else
                                outputStack.Push(value)
                            System.Diagnostics.Debug.WriteLine("Variable " + value + " added to output stack")
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
