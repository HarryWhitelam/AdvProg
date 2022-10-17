// lexer.fs
//
// Author:      Irie Railton
// Description: 

//*************************************************************************

namespace interpreter

//*************************************************************************

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

let get_token character =
     match character with
     | "+" -> Token.Plus
     | "-" -> Token.Minus
     | "*" -> Token.Times     
     | "/" -> Token.Divide
     | "(" -> Token.L_Bracket
     | ")" -> Token.R_Bracket
     | "^" -> Token.Indice
    //  | :? int -> Token.Number
    //  | :? char -> Token.Variable
     | _ -> Token.Keyword

let lex input =
    printfn "%O" <| get_token input