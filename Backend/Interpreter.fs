namespace Backend

module Interpreter =
    let interpret(expression) = 
        if Seq.length expression = 0 then null
        else 
            let tokens = Lexer.lex expression
            if Parser.parse(tokens).IsEmpty then
                (string) (Executor.shuntingYard tokens)
            else null

    let getVarStore() = Executor.variableStore

    let removeVarStore(key:string) = 
        Executor.variableStore <- Executor.variableStore.Remove(key)
