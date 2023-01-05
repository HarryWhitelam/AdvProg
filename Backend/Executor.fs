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
        System.Diagnostics.Debug.WriteLine($"vecToString input: {input}")
        let mutable out = "["
        for i in input do
            out <- out + i.ToString() + ","
        out.Remove(out.Length-1, 1) + "]"

    let isVector (value:string) =
        value.Contains("[")

    let isVar (var:string) =
        System.Char.IsLetter(var.[0])

    let isNumber value =
        try double value |> ignore
            true
        with :? System.FormatException -> false

    let replaceVariable (var :byref<string>) =
        if not(isNumber var) then
            if variableStore.TryGetValue(var, &var) = false then
                raise (ExecError $"{var} does not have an assigned value")

    let replaceReserved name =
        match name with
        | "PI" -> string(double(System.Math.PI))
        | "E" -> string(double(System.Math.E))
        | _ -> raise (ExecError "How on earth did you raise this")

    let handleFunc(funcName) =
        let args = stringToVec(outputStack.Pop())
        match args.Length with
        | 1 ->  match funcName with
                | "LOG" ->  log(args.[0])
                | "SQRT" -> sqrt(args.[0])
                | _ -> raise (ExecError (funcException(funcName, 1)))
        | 2 ->  match funcName with
                | "NROOT"-> args.[0] ** (1.0/ args.[1])
                | "LOGN" -> System.Math.Log(args.[0], args.[1])
                | _ -> raise (ExecError (funcException(funcName, 2)))
        | any -> raise (ExecError (funcException(funcName, any)))

    let handleAssign(value:byref<string>, value2:byref<string>) =
        System.Diagnostics.Debug.WriteLine($"value = {value}, value2 = {value2}")
        try double value |> ignore
            variableStore <- variableStore.Add(value2, value)
        with :? System.FormatException ->
            if variableStore.TryGetValue(value, &value) then
                variableStore <- variableStore.Add(value2, value)
            else if isVector value then
                variableStore <- variableStore.Add(value2, value)
            else raise (ExecError $"{value} is not a number or a stored variable")

    let handleArgs value =
        let mutable newVal = outputStack.Pop()
        if isVar newVal then replaceVariable &newVal
        let mutable out = newVal + "," + value
        System.Diagnostics.Debug.WriteLine($"out={out}")
        while operatorStack.Count <> 0 && operatorStack.Peek() = Token.Comma do
            operatorStack.Pop() |> ignore
            let mutable newVal = outputStack.Pop()
            if isVar newVal then replaceVariable &newVal
            out <- newVal + "," + out
        "[" + out + "]"

    let operateOnVecs(vec1str, vec2str, op) =
        System.Diagnostics.Debug.WriteLine($"vec1str: {vec1str}, vec2str: {vec2str}")
        let vec1 = stringToVec vec1str
        let vec2 = stringToVec vec2str
        System.Diagnostics.Debug.WriteLine($"vec1: {vec1}")
        System.Diagnostics.Debug.WriteLine($"vec1: {vec2}")
        if vec1.Length = vec2.Length then
            vecToString (List.map2(op) vec1 vec2)
        else
            raise (ExecError $"Vector {vec2str} of size {vec2.Length} is not the same size as vector {vec1str} of size {vec1.Length}") 

    let operateOnVec(vec, op) =
        vecToString (List.map(op) (stringToVec(vec)))

    let calculate() =
        let operator = operatorStack.Pop()
        match operator with
        | Token.Function funcName -> outputStack.Push(string (handleFunc(funcName)))
        | _ ->  let mutable value = outputStack.Pop()
                match operator with
                | Token.Comma ->
                        if isVar value then replaceVariable &value
                        outputStack.Push(handleArgs value)                 
                | Token.Assign ->
                        let mutable value2 = outputStack.Pop()
                        handleAssign(&value, &value2)
                        outputStack.Push(value2 + ":=" + value)
                | _ ->         
                    if isVar value then replaceVariable &value
                    if (isVector value) then
                        if operator = Token.Minus && (outputStack.Count = 0 || (operatorStack.Count <> 0 && operatorStack.Peek() = Token.L_Parenth)) then 
                            outputStack.Push(operateOnVec(value, (fun a -> 0.0-a)))
                        else
                            let mutable value2 = outputStack.Pop()
                            if isVar value2 then replaceVariable &value2
                            if isVector value2 then
                                match operator with
                                | Token.Times ->    outputStack.Push(operateOnVecs(value2, value, (fun a b -> a*b)))
                                | Token.Plus ->     outputStack.Push(operateOnVecs(value2, value, (fun a b -> a+b)))
                                | Token.Minus ->    outputStack.Push(operateOnVecs(value2, value, (fun a b -> a-b)))
                                | _ ->              failwith $"Invalid {operator.ToString()} here"
                            else 
                                raise (ExecError "I'll figure this out later")
                    else
                        if operator = Token.Minus && (outputStack.Count = 0 || (operatorStack.Count <> 0 && operatorStack.Peek() = Token.L_Parenth)) then
                            outputStack.Push(string (0.0 - double value))
                        else
                            let mutable value2 = outputStack.Pop()
                            if isVar value2 then replaceVariable &value2
                            if isVector value2 then
                                match operator with
                                | Token.Indice ->   outputStack.Push(operateOnVec(value2, (fun a -> a**(double value))))
                                | Token.Times ->    outputStack.Push(operateOnVec(value2, (fun a -> a*(double value))))
                                | Token.Divide ->   outputStack.Push(operateOnVec(value2, (fun a -> a/(double value))))
                                | Token.Plus ->     outputStack.Push(operateOnVec(value2, (fun a -> a+(double value))))
                                | Token.Minus ->    outputStack.Push(operateOnVec(value2, (fun a -> a-(double value))))
                                | _ ->              failwith $"Invalid {operator.ToString()} here"
                            else
                                match operator with
                                | Token.Indice ->   outputStack.Push(string (double value2 ** double value))
                                | Token.Times ->    outputStack.Push(string (double value2 * double value))
                                | Token.Divide ->   outputStack.Push(string (double value2 / double value))
                                | Token.Plus ->     outputStack.Push(string (double value2 + double value))
                                | Token.Minus ->    outputStack.Push(string (double value2 - double value))
                                | _ ->              failwith $"Invalid {operator.ToString()} here"

    let shuntingYard (tokens: Token list) =
        for token in tokens do
            match token with
            | Token.Number value -> 
                            outputStack.Push(value)
                            System.Diagnostics.Debug.WriteLine($"Number {value} added to output stack")
            | Token.Variable name -> 
                            if(tokens.Length = 1) then 
                                let mutable value = name
                                replaceVariable &value
                                outputStack.Push(value)
                            else
                                outputStack.Push(name)
                            System.Diagnostics.Debug.WriteLine($"Variable {name} added to output stack")
            | Token.Reserved name -> 
                            let value = replaceReserved(name);
                            outputStack.Push(value)
                            System.Diagnostics.Debug.WriteLine($"{name} added to output stack as {value}")
            | Token.Function value ->
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine($"{value} added to operator stack")
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
                            //if operatorStack.Count <> 0 then
                            //    match operatorStack.Peek() with 
                            //    | Token.Function _ -> calculate()
                            //    //| _ -> ()
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
            | Token.SemiColon -> 
                            operatorStack.Push(token)
                            System.Diagnostics.Debug.WriteLine("; added to operator stack")
        while operatorStack.Count <> 0 do
            calculate()
        outputStack.Pop()
