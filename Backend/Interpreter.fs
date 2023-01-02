namespace Backend

module Interpreter =
    open System

    let interpret expression = 
        if Seq.length expression = 0 then null
        else 
            let tokens = Lexer.lex expression
            if Parser.parse(tokens).IsEmpty then
                (string) (Executor.shuntingYard tokens)
            else null


    let updateVarStore = Executor.variableStore

    let getVarStore() = 
        Executor.variableStore

    let removeVarStore(key:string) = 
        Executor.variableStore <- Executor.variableStore.Remove(key)

    let rec printTList (lst:list<Token>) : list<string> = 
        match lst with
        head::tail ->   Console.Write("{0} ",head.ToString())
                        printTList tail
        | [] ->         Console.Write("EOL\n")
                        []