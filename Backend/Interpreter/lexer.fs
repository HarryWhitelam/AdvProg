// lexer.fs
//
// Author:      Irie Railton
// Description: 

//*************************************************************************

// namespace interpreter

//*************************************************************************
open System

type Token = 
    Plus | Minus | Times | Divide | L_Bracket | R_Bracket | Indice | Equals | Assign | Number of int | Variable of string

let lexerError (reason:string) = System.Exception("Lexer machine broke: " + reason)

let strToList str = [for s in str -> s]
let intVal (c:char) = (int)((int)c - (int)'0')
let charToStr (c:char) = System.Char.ToString(c)

let rec catchNum(rest, finVal) = 
    match rest with
    | num :: tail when (System.Char.IsDigit num) -> catchNum(tail, 10*finVal+(intVal num))
    | _ -> (rest, finVal)

let rec catchVar(rest, finStr) =
    match rest with
    | var :: tail when (System.Char.IsLetter var) -> catchVar(tail, finStr+(charToStr var))
    | _ -> (rest, finStr)

let lexer input =
    let rec lex input =
        match input with
        | [] -> []
        | '+'::tail -> Token.Plus     :: lex tail
        | '*'::tail -> Token.Times    :: lex tail
        | '-'::tail -> Token.Minus    :: lex tail
        | '/'::tail -> Token.Divide   :: lex tail
        | '('::tail -> Token.L_Bracket:: lex tail
        | ')'::tail -> Token.R_Bracket:: lex tail
        | '^'::tail -> Token.Indice   :: lex tail
        | '='::tail -> Token.Equals   :: lex tail
        | ':'::tail -> match tail with
                        | '='::tail -> Token.Assign :: lex tail
                        | _ -> raise (lexerError "Expected '=' after ':'")
        | num::tail when (System.Char.IsDigit num) ->   let (rest, finVal) = catchNum (tail, intVal num)
                                                        Token.Number finVal :: lex rest
        | var::tail when (System.Char.IsLetter var) ->  let (rest, finStr) = catchVar (tail, charToStr var)
                                                        Token.Variable finStr :: lex rest
        | spc::tail when (System.Char.IsWhiteSpace spc) -> lex tail
        | _ -> raise (lexerError "Undefined character")
    lex (strToList input)
