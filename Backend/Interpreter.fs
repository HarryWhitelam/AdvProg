namespace Backend

module Interpreter =
    let interpret expression = 
        if Seq.length expression = 0 then null
        else 
            let tokens = Lexer.lex expression
            if Parser.parse(tokens).IsEmpty then
                (string) (Executor.shuntingYard tokens)
            else null

    let updateVarStore = Executor.variableStore