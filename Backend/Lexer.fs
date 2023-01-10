// Lexer.fs
//
// Author:      Irie Railton
// Description: Lexer for all tokens in Baltam

namespace Backend

/// Type to define each token that is used in the lexer, parser, and shunting yard algorithms
type Token = 
    | Plus | Minus | Times | Divide  | Indice | Dot
    | L_Parenth | R_Parenth | Comma  | SemiColon | L_Bracket | R_Bracket 
    | Number of string | Function of string | Reserved of string
    | Variable of string | Assign
    
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

    /// Prints a list of tokens
    static member printTokens tokens =
        let mutable out = ""
        for t in tokens do
            out <- out + t.ToString()
        out

    ///<summary>Prints list of tokens and an arrow below this in the specified position in the list of tokens</summary>
    ///<param name="tokens">list of tokens to print</param>
    ///<param name="position">position of token to point to</param>
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
    
    type LexerError (message:string) = inherit System.Exception(message)

    let showExceptionPosition((input: string), position) =
        let mutable buffer = ""
        if position > 0 then
            buffer <- buffer + String.replicate position " "
        "\r   " + input + "\r   " + buffer + "^"

    let rec catchNum(rest, finVal) = 
        match rest with
        | num :: tail when (System.Char.IsDigit num) -> 
            catchNum(tail, finVal+(string)num)
        | '.' :: tail -> catchNum(tail, finVal+".")
        | _ -> (rest, finVal)

    let rec catchVar(rest, finStr) =
        match rest with
        | var :: tail when (System.Char.IsLetter var) -> 
            catchVar(tail, finStr+(string)var)
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
                                                            | "SIN" ->  Token.Function finStrUp :: consume rest
                                                            | "COS" ->  Token.Function finStrUp :: consume rest
                                                            | "TAN" ->  Token.Function finStrUp :: consume rest
                                                            | "E" ->    Token.Reserved finStrUp :: consume rest
                                                            | "PI" ->   Token.Reserved finStrUp :: consume rest
                                                            | _ ->      Token.Variable finStr   :: consume rest
            | spc::tail when (System.Char.IsWhiteSpace spc) -> consume tail
            | _ -> raise (LexerError $"Undefined character: {showExceptionPosition(ogInput, ogInput.Length-input.Length)}")
            
        consume (Seq.toList input)
