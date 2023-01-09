// Lexer.fs
//
// Author:      Irie Railton
// Description: 

//*************************************************************************

namespace Backend

//*************************************************************************

type Token = 
    Plus | Minus | Times | Divide | L_Parenth | R_Parenth | Indice | Assign | Comma | Dot | SemiColon | L_Bracket | R_Bracket | Number of string | Variable of string | Function of string | Reserved of string
    
    override this.ToString() = 
            match this with
            | Plus -> "+"
            | Minus -> "-"
            | Times -> "*"
            | Divide -> "/"
            | L_Parenth -> "("
            | R_Parenth -> ")"
            | Indice -> "^"
            | Assign -> ":="
            | Comma -> ","
            | Dot -> "."
            | SemiColon -> ";"
            | L_Bracket -> "["
            | R_Bracket -> "]"
            | Number value -> value
            | Variable name -> name
            | Function name -> name
            | Reserved name -> name

    static member printTokens tokens =
        let mutable out = ""
        for t in tokens do
            out <- out + t.ToString()
        out

    static member pointToToken((tokens: list<Token>), position) =
        let mutable buffer = ""
        if position > 0 then
            for t in tokens.[..position-1] do
                let len = match t with
                          | Number value -> value.Length
                          | Variable name -> name.Length
                          | Function name -> name.Length
                          | Reserved name -> name.Length
                          | Assign -> 2
                          | _ -> 1
                buffer <- buffer + String.replicate len " "
        "\r   " + Token.printTokens(tokens) + "\r   " + buffer + "^"


module Lexer =
    
    exception LexerError of string

    let showExceptionPosition((input: string), position) =
        let mutable buffer = ""
        if position > 0 then
            buffer <- buffer + String.replicate position " "
        "\r   " + input + "\r   " + buffer + "^"

    let rec catchNum(rest, finVal) = 
        match rest with
        | num :: tail when (System.Char.IsDigit num) -> catchNum(tail, finVal+(string)num)
        | '.' :: tail -> catchNum(tail, finVal+".")
        | _ -> (rest, finVal)

    let rec catchVar(rest, finStr) =
        match rest with
        | var :: tail when (System.Char.IsLetter var) -> catchVar(tail, finStr+(string)var)
        | _ -> (rest, finStr)

    let lex input =
        let ogInput = input
        let rec consume input =
            match input with
            | [] -> []
            | '+'::tail -> Token.Plus     :: consume tail
            | '*'::tail -> Token.Times    :: consume tail
            | '-'::tail -> Token.Minus    :: consume tail
            | '/'::tail -> Token.Divide   :: consume tail
            | '^'::tail -> Token.Indice   :: consume tail
            | '('::tail -> Token.L_Parenth:: consume tail
            | ')'::tail -> Token.R_Parenth:: consume tail
            | '['::tail -> Token.L_Bracket:: consume tail
            | ']'::tail -> Token.R_Bracket:: consume tail
            | ':'::tail -> match tail with
                            | '='::tail -> Token.Assign :: consume tail
                            | _ -> raise (LexerError $"Expected '=' after ':': {showExceptionPosition(ogInput, ogInput.Length-input.Length+1)}")
            | ','::tail -> Token.Comma    :: consume tail
            | '.'::tail -> Token.Dot      :: consume tail
            | ';'::tail -> Token.SemiColon:: consume tail
            | num::tail when (System.Char.IsDigit num) ->   let (rest, finVal) = catchNum (tail, (string)num)
                                                            Token.Number finVal :: consume rest
            | var::tail when (System.Char.IsLetter var) ->  let (rest, finStr) = catchVar (tail, (string)var)
                                                            let finStrUp = finStr.ToUpper()
                                                            match finStrUp with
                                                            | "SQRT" -> Token.Function finStrUp :: consume rest
                                                            | "NROOT" ->Token.Function finStrUp :: consume rest
                                                            | "LOG" ->  Token.Function finStrUp :: consume rest
                                                            | "LOGN" -> Token.Function finStrUp :: consume rest
                                                            | "E" ->    Token.Reserved finStrUp :: consume rest
                                                            | "PI" ->   Token.Reserved finStrUp :: consume rest
                                                            | _ ->      Token.Variable finStr   :: consume rest
            | spc::tail when (System.Char.IsWhiteSpace spc) -> consume tail
            | _ -> raise (LexerError $"Undefined character: {showExceptionPosition(ogInput, ogInput.Length-input.Length)}")
            
        consume (Seq.toList input)
