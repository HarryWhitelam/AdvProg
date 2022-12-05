// Lexer.fs
//
// Author:      Irie Railton
// Description: 

//*************************************************************************

namespace Backend

//*************************************************************************

type Token = 
    Plus | Minus | Times | Divide | L_Bracket | R_Bracket | Indice | Assign | Number of string | Variable of string | Reserved of string

module Lexer =
    
    exception LexerError of string

    let strToList str = [for s in str -> s]

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
        let rec consume input =
            match input with
            | [] -> []
            | '+'::tail -> Token.Plus     :: consume tail
            | '*'::tail -> Token.Times    :: consume tail
            | '-'::tail -> Token.Minus    :: consume tail
            | '/'::tail -> Token.Divide   :: consume tail
            | '('::tail -> Token.L_Bracket:: consume tail
            | ')'::tail -> Token.R_Bracket:: consume tail
            | '^'::tail -> Token.Indice   :: consume tail
            | ':'::tail -> match tail with
                            | '='::tail -> Token.Assign :: consume tail
                            | _ -> raise (LexerError "Expected '=' after ':'")
            | num::tail when (System.Char.IsDigit num) ->   let (rest, finVal) = catchNum (tail, (string)num)
                                                            Token.Number finVal :: consume rest
            | var::tail when (System.Char.IsLetter var) ->  let (rest, finStr) = catchVar (tail, (string)var)
                                                            match finStr with
                                                            | "sqr" ->  Token.Reserved finStr :: consume rest
                                                            | _ ->      Token.Variable finStr :: consume rest
            | spc::tail when (System.Char.IsWhiteSpace spc) -> consume tail
            | _ -> raise (LexerError "Undefined character")
        consume (strToList input)
