namespace Backend

module Interpreter =
    open System

    let interpret expression = 
        let tokens = Lexer.lex expression
        if Parser.parse(tokens).IsEmpty then
            (string) (Executor.shuntingYard tokens)
        else null

    let updateVarStore = Executor.variableStore

    let rec printTList (lst:list<Token>) : list<string> = 
        match lst with
        head::tail ->   Console.Write("{0} ",head.ToString())
                        printTList tail
        | [] ->         Console.Write("EOL\n")
                        []