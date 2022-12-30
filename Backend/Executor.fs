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

    let funcException(funcName, noArgs) =
        $"No function, {funcName}() with {noArgs} arguments"

    let mutable variableStore = Map.empty<string, string>
    let outputStack = Stack<string>()
    let operatorStack = Stack<Token>()

    let stringToVec (input:string) =
        let stringVec = input.Substring(1, input.Length-2).Split(",")
        let mutable out:list<double> = []
        for s in stringVec do
            out <- out@[double (s)]
        out

    let vecToString (input:list<double>) =
        let mutable out = "["
        for i in input do
            out <- out + i.ToString() + ", "
        out.Remove(out.Length-2, 2) + "]"

    let isVector (value:string) =
        value.Contains(",")

    let replaceVar (var :byref<string>) =
        try double var |> ignore
        with :? System.FormatException -> 
            if variableStore.TryGetValue(var, &var) = false then
                raise (ExecError $"{var} does not have an assigned value")

    let handleFunc(funcName) =
        if outputStack.Count = 0 then
            match funcName with
            | "PI" -> double(System.Math.PI)
            | "E" -> double(System.Math.E)
            | _ -> raise (ExecError (funcException(funcName, 0)))
        else
            let mutable value = outputStack.Pop()
            let args = value.Split(",")
            match args.Length with 
            | 1 ->  match funcName with
                    | "LOG" ->  log(double (args[0]))
                    | "SQRT" -> sqrt(double (args[0]))
                    | _ -> raise (ExecError (funcException(funcName, 1)))
            | 2 ->  match funcName with
                    | "NROOT"-> double (args[0]) ** (1.0/ double (args[1]))
                    | "LOGN" -> System.Math.Log(double (args[0]), double (args[1]))
                    | _ -> raise (ExecError (funcException(funcName, 2)))
            | any -> raise (ExecError (funcException(funcName, any)))

    let handleAssign(value:byref<string>, value2:byref<string>) =
        try double value |> ignore
            variableStore <- variableStore.Add(value2, value)
        with :? System.FormatException ->
            if variableStore.TryGetValue(value, &value) then variableStore <- variableStore.Add(value2, value)
            else raise (ExecError $"{value} is not a number or a stored variable")

    let handleArgs value =
        let mutable out = "[" + outputStack.Pop() + "," + value 
        while operatorStack.Count <> 0 && operatorStack.Peek() = Token.Comma do
            operatorStack.Pop() |> ignore
            let mutable newVal = outputStack.Pop()
            replaceVar &newVal
            out <- newVal + "," + out
        out + "]"

    let calculate() =
        let operator = operatorStack.Pop()
        match operator with 
        | Token.Function funcName -> outputStack.Push(string (handleFunc(funcName)))
        | _ ->  let mutable value = outputStack.Pop()
                match operator with
                | Token.Comma ->
                        replaceVar &value
                        outputStack.Push(handleArgs value)                 
                | Token.Assign ->   
                        let mutable value2 = outputStack.Pop()
                        handleAssign(&value, &value2)
                        outputStack.Push(value2 + ":=" + value)
                | _ ->  
                    if (isVector value) then
                        if operator = Token.Minus && (outputStack.Count = 0 || (operatorStack.Count <> 0 && operatorStack.Peek() = Token.L_Parenth)) then 
                            outputStack.Push(vecToString (List.map(fun a -> 0.0-a) (stringToVec value))) 
                        else
                            let mutable value2 = stringToVec (outputStack.Pop())
                            match operator with
                            | Token.Indice ->   outputStack.Push(vecToString (List.map2(fun a b -> a**b) value2 (stringToVec(value))))
                            | Token.Times ->    outputStack.Push(vecToString (List.map2(fun a b -> a*b) value2 (stringToVec(value))))
                            | Token.Divide ->   outputStack.Push(vecToString (List.map2(fun a b -> a/b) value2 (stringToVec(value))))
                            | Token.Plus ->     outputStack.Push(vecToString (List.map2(fun a b -> a+b) value2 (stringToVec(value))))
                            | Token.Minus ->    outputStack.Push(vecToString (List.map2(fun a b -> a-b) value2 (stringToVec(value))))
                            | _ ->              failwith "Invalid operator here"
                    else
                        if operator = Token.Minus && (outputStack.Count = 0 || (operatorStack.Count <> 0 && operatorStack.Peek() = Token.L_Parenth)) then
                            replaceVar &value
                            outputStack.Push(string (0.0 - double value)) 
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
            | Token.Function value -> 
                            //while operatorStack.Count <> 0 && operatorStack.Peek() <> Token.L_Bracket && operatorStack.Peek() <> Token.Assign do
                            //    calculate()
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine(value + " added to operator stack")
            | Token.Plus -> 
                            while operatorStack.Count <> 0 && operatorStack.Peek() <> Token.L_Parenth && operatorStack.Peek() <> Token.Assign do
                                calculate()
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("+ added to operator stack")
            | Token.Minus -> 
                            while operatorStack.Count <> 0 && operatorStack.Peek() <> Token.L_Parenth && operatorStack.Peek() <> Token.Assign do
                                calculate()
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("- added to operator stack")
            | Token.Times -> 
                            while operatorStack.Count <> 0 && operatorStack.Peek() <> Token.L_Parenth && operatorStack.Peek() <> Token.Assign && operatorStack.Peek() <> Token.Plus && operatorStack.Peek() <> Token.Minus do
                                calculate()
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("* added to operator stack")
            | Token.Divide ->
                            while operatorStack.Count <> 0 && operatorStack.Peek() <> Token.L_Parenth && operatorStack.Peek() <> Token.Assign && operatorStack.Peek() <> Token.Plus && operatorStack.Peek() <> Token.Minus do
                                calculate()
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("/ added to operator stack")
            | Token.L_Parenth -> 
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("( added to operator stack")
            | Token.R_Parenth -> 
                            while operatorStack.Count <> 0 && operatorStack.Peek() <> Token.L_Parenth do
                                calculate()
                            System.Diagnostics.Debug.WriteLine("{0} removed from operator stack", operatorStack.Pop())
            | Token.L_Bracket ->
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("[ added to operator stack")
            | Token.R_Bracket ->
                            while operatorStack.Count <> 0 && operatorStack.Peek() <> Token.L_Parenth && operatorStack.Peek() <> Token.L_Bracket do
                                calculate()
                            System.Diagnostics.Debug.WriteLine("{0} removed from operator stack", operatorStack.Pop())
            | Token.Indice -> 
                            while operatorStack.Count <> 0 && (operatorStack.Peek() = Token.R_Parenth || operatorStack.Peek() = Token.Indice) do
                                calculate()
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("^ added to operator stack")
            | Token.Assign -> 
                            while operatorStack.Count <> 0 && operatorStack.Peek() = Token.R_Parenth do
                                calculate()
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine(":= added to operator stack")
            | Token.Comma -> 
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine(", added to operator stack")
        while operatorStack.Count <> 0 do
            calculate()
        outputStack.Pop()
