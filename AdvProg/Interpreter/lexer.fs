// lexer.fs
//
// Author:      Irie Railton
// Description: 

//*************************************************************************

namespace interpreter

//*************************************************************************
open System.Text.RegularExpressions

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

let rx = Regex "[0-9]+"

let get_token character =
     match character with
     | "+" -> Token.Plus
     | "-" -> Token.Minus
     | "*" -> Token.Times     
     | "/" -> Token.Divide
     | "(" -> Token.L_Bracket
     | ")" -> Token.R_Bracket
     | "^" -> Token.Indice
     | rx.IsMatch -> Token.Number
    //  | Regex.Match(@"[a-zA-Z_]\w*") -> Token.Variable
     | _ -> Token.Keyword

let lex input =
    printfn "%O" <| get_token input