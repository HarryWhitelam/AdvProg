namespace Backend

module Interpreter =
    open System

    let interpret expression = 
        let tokens = Lexer.lex expression
        if Parser.parse(tokens).IsEmpty then
            Executor.shuntingYard tokens

    let getInputString : string = 
        Console.Write("Enter an expression: ")
        Console.ReadLine()

    let rec printTList (lst:list<Token>) : list<string> = 
        match lst with
        head::tail -> Console.Write("{0} ",head.ToString())
                      printTList tail
        | [] -> Console.Write("EOL\n")
                []