// Lexer.fs
//
// Author:      Irie Railton
// Description: 

//*************************************************************************

namespace Backend

//*************************************************************************

type Token = 
    Plus | Minus | Times | Divide | L_Bracket | R_Bracket | Indice | Assign | Number of double | Variable of string

module Lexer =

    let lexerError (reason:string) = System.Exception("Lexer machine broke: " + reason)

    let strToList str = [for s in str -> s]
    let intVal (c:char) = (double)((int)c - (int)'0')
    let charToStr (c:char) = System.Char.ToString(c)

    //let rec catchDecimals(rest, finVal) =
    //    System.Diagnostics.Debug.WriteLine("rest = {0}", string(rest))
    //    System.Diagnostics.Debug.WriteLine("finval = {0}", string(floor(finVal)))
    //    match rest with
    //    | num :: tail when (System.Char.IsDigit num) -> catchDecimals(tail, (10.0*finVal+(intVal num))/(10.0**(floor(finVal)).))
    //    | _ -> (rest, finVal)

    //let rec catchNum(rest, finVal) = 
    //    match rest with
    //    | num :: tail when (System.Char.IsDigit num) -> catchNum(tail, 10.0*finVal+(intVal num))
    //    | '.' :: tail -> catchDecimals(tail, finVal)
    //    | _ -> (rest, finVal)


    let rec catchNum(rest, finVal) = 
        System.Diagnostics.Debug.WriteLine("finval = "+finVal)
        match rest with
        | num :: tail when (System.Char.IsDigit num) -> catchNum(tail, finVal+(string)num)
        | '.' :: tail -> catchNum(tail, finVal+".")
        | _ -> (rest, (double)finVal)

    let rec catchVar(rest, finStr) =
        match rest with
        | var :: tail when (System.Char.IsLetter var) -> catchVar(tail, finStr+(charToStr var))
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
                            | _ -> raise (lexerError "Expected '=' after ':'")
            | num::tail when (System.Char.IsDigit num) ->   let (rest, finVal) = catchNum (tail, (string)num)
                                                            Token.Number ((double)finVal) :: consume rest
            | var::tail when (System.Char.IsLetter var) ->  let (rest, finStr) = catchVar (tail, charToStr var)
                                                            Token.Variable finStr :: consume rest
            | spc::tail when (System.Char.IsWhiteSpace spc) -> consume tail
            | _ -> raise (lexerError "Undefined character")
        consume (strToList input)
