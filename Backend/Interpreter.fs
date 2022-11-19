namespace Backend

module Interpreter =

    let interpret expression = Parser.parse <| Lexer.lex expression
    let hello = "hello"