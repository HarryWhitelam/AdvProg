// lexer.fs
//
// Author:      Irie Railton
// Description: 

//*************************************************************************

namespace interpreter

//*************************************************************************
open System

type Token =
    | Plus = 0
    | Minus = 1
    | Times = 2
    | Divide = 3
    | L_Bracket = 4
    | R_Bracket =5
    | Indice = 6
    | Number = 7
    | Variable = 8
    | Equals = 9
    | Keyword = 10

let lexerBroke = System.Exception("Lexer machine broke")

let strToList str = [for s in str -> s]

//let rec catchNum input = 
//    match input with
//    | num :: rest ->

//let rec catchVar

let lexer input =
    let rec lex input =
        match input with
        | [] -> []
        | '+'::rest -> Token.Plus     :: lex rest
        | '-'::rest -> Token.Minus    :: lex rest
        | '*'::rest -> Token.Times    :: lex rest
        | '/'::rest -> Token.Divide   :: lex rest
        | '('::rest -> Token.L_Bracket:: lex rest
        | ')'::rest -> Token.R_Bracket:: lex rest
        | '^'::rest -> Token.Indice   :: lex rest
        //| num::rest when System.Char.IsDigit num -> catchNum 
        //| var::rest when 
        | _ -> raise lexerBroke
    lex (strToList input)


let test input =
    printfn "%O" <| lexer input