// parser.fs
//
// Author:      Irie Railton
// Description: 

//*************************************************************************

namespace Backend

//*************************************************************************

module Parser =

    let parserError (reason:string) = System.Exception ("Parser Machine Broke: " + reason)

    //Grammar in standard BNF
    //<assign>  ::= <expr>    | Variable:=<expr>
    //<expr>    ::= <term>    | <expr>+<term>   | <expr>-<term>
    //<term>    ::= <index>   | <term>*<index>  | <term>/<index>
    //<index>   ::= <unary>   | <index>^<unary>
    //<unary>   ::= -<factor> | <factor>
    //<factor>  ::= Number | Variable | (<expr>)

    //Grammar in LL(1) BNF
    //<assign>  ::= <expr> | Variable:=<expr>
    //<expr>    ::= <term><expr'>
    //<expr'>   ::= +<term><expr'> | -<term><expr'> | ∅
    //<term>    ::= <index><term'>
    //<term'>   ::= *<index><term'> | /<index><term'> | ∅
    //<index>   ::= <unary><index'>
    //<index'>  ::= ^<unary><index'> | ∅
    //<unary>   ::= -<factor> | <factor>
    //<factor>  ::= Number | Variable | (<expr>)

    let parse tokens =
        let rec assign tokens = 
            match tokens with
            | Token.Variable _value :: Token.Assign :: tail -> expr tail
            | _ :: Token.Assign :: _tail -> raise (parserError "Expected variable before assignment")
            | _ -> expr tokens
        and expr tokens = (term >> expr_pr) tokens
        and expr_pr tokens =
            match tokens with
            | Token.Plus  :: tail -> (term >> expr_pr) tail
            | Token.Minus :: tail -> (term >> expr_pr) tail
            | _ -> tokens
        and term tokens = (index >> term_pr) tokens
        and term_pr tokens =
            match tokens with
            | Token.Times  :: tail -> (index >> term_pr) tail
            | Token.Divide :: tail -> (index >> term_pr) tail
            | _ -> tokens
        and index tokens = (unary >> index_pr) tokens
        and index_pr tokens =
            match tokens with
            | Token.Indice :: tail -> (unary >> index_pr) tail
            | _ -> tokens
        and unary tokens =
            match tokens with
            | Token.Minus :: tail -> factor tail
            | _ -> factor tokens
        and factor tokens =
            match tokens with
            | Token.Number value   :: tail -> tail
            | Token.Variable value :: tail -> tail
            | Token.L_Bracket ::tail -> match expr tail with
                                        | Token.R_Bracket :: tail -> tail
                                        | _ -> raise (parserError "Missing closing bracket")
            | _ -> raise (parserError "Expected number or variable but none supplied")
        assign tokens
