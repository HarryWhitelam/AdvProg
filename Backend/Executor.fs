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

    let isVar (var:string) =
        System.Char.IsLetter(var.[0])

    let isNumber value =
        try double value |> ignore
            true
        with :? System.FormatException -> false

    let replaceVar (var :byref<string>) =
        if not(isNumber var) then
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
        replaceVar &newVal
        let mutable out = newVal + "," + value
        System.Diagnostics.Debug.WriteLine($"out={out}")
        while operatorStack.Count <> 0 && operatorStack.Peek() = Token.Comma do
            operatorStack.Pop() |> ignore
            let mutable newVal = outputStack.Pop()
            replaceVar &newVal
            out <- newVal + "," + out
        "[" + out + "]"

    let operateOnVecs(vec1str, vec2str, op) =
        let vec1 = stringToVec vec1str
        let vec2 = stringToVec vec2str
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
                        replaceVar &value
                        outputStack.Push(handleArgs value)                 
                | Token.Assign ->
                        let mutable value2 = outputStack.Pop()
                        handleAssign(&value, &value2)
                        outputStack.Push(value2 + ":=" + value)
                | _ -> // TODO: needs re-doing so that it checks if value, and if necessary value2, are variables and then replaces them with their value if so and then chooses how to handle them based on their type                    
                    if isVar value then replaceVar &value
                    if (isVector value) then
                        if operator = Token.Minus && (outputStack.Count = 0 || (operatorStack.Count <> 0 && operatorStack.Peek() = Token.L_Parenth)) then 
                            outputStack.Push(operateOnVec(value, (fun a -> 0.0-a)))
                        else
                            let mutable value2 = outputStack.Pop()
                            if isVar value2 then
                                replaceVar &value2
                            if isVector value2 then
                                match operator with
                                //| Token.Indice ->   outputStack.Push(operateOnVecs(value2, value, (fun a b -> a**b))) // dont think this should be a thing
                                | Token.Times ->    outputStack.Push(operateOnVecs(value2, value, (fun a b -> a*b)))
                                //| Token.Divide ->   outputStack.Push(operateOnVecs(value2, value, (fun a b -> a/b))) // neither should this
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
                            if isVar value2 then replaceVar &value2
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
