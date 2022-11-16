namespace Backend

module Interpreter =
    open System

    let interpret expression = Parser.parse <| Lexer.lex expression

    let getInputString : string = 
        Console.Write("Enter an expression: ")
        Console.ReadLine()

    let rec printTList (lst:list<Token>) : list<string> = 
        match lst with
        head::tail -> Console.Write("{0} ",head.ToString())
                      printTList tail
        | [] -> Console.Write("EOL\n")
                []