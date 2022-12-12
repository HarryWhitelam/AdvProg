// parser.fs
//
// Author:      Irie Railton
// Description: 

//*************************************************************************

namespace Backend

//*************************************************************************

module Parser =

    exception ParseError of string

    //Grammar in standard BNF
    //<assign>  ::= <expr>    | Variable:=<expr>
    //<expr>    ::= <unary>   | <expr>+<unary>   | <expr>-<term>
    //<unary>   ::= -<term>   | <term>
    //<term>    ::= <index>   | <term>*<index>  | <term>/<index>
    //<index>   ::= <factor>  | <index>^<factor>
    //<factor>  ::= Number | Variable | (<expr>)

    //Grammar in LL(1) BNF
    //<assign>  ::= <expr> | Variable:=<expr>
    //<expr>    ::= <unary><expr'>
    //<expr'>   ::= +<unary><expr'> | -<term><expr'> | ∅
    //<unary>   ::= -<term> | <term>
    //<term>    ::= <index><term'>
    //<term'>   ::= *<index><term'> | /<index><term'> | ∅
    //<index>   ::= <factor><index'>
    //<index'>  ::= ^<factor><index'> | ∅
    //<factor>  ::= Number | Variable | (<expr>)

    let showExceptionPosition((tokens: Token list), position) =
        let mutable buffer = ""
        if position > 0 then
            for t in tokens[..position-1] do
                let len = match t with
                          | Token.Number value -> value.Length
                          | Token.Variable value -> value.Length
                          | Token.Reserved value -> value.Length
                          | Token.Assign -> 2
                          | _ -> 1
                buffer <- buffer + String.replicate len " "
        "\n   " + Token.printTokens(tokens) + "\n   " + buffer + "^"

    let parse (tokens: Token list) =
        let ogTokens = tokens
        let rec assign tokens = 
            match tokens with
            | Token.Variable _value :: Token.Assign :: tail -> expr tail
            | _ :: Token.Assign :: _tail -> 
                raise (ParseError $"Expected variable before assignment: {showExceptionPosition(ogTokens, 0)}")
            | _ -> expr tokens
        and expr tokens = (unary >> expr_pr) tokens
        and expr_pr tokens =
            match tokens with
            | Token.Plus  :: tail -> (unary >> expr_pr) tail
            | Token.Minus :: tail -> (term >> expr_pr) tail
            | _ -> tokens
        and unary tokens =
            match tokens with
            | Token.Minus :: tail -> term tail
            | _ -> term tokens
        and term tokens = (index >> term_pr) tokens
        and term_pr tokens =
            match tokens with
            | Token.Times  :: tail -> (index >> term_pr) tail
            | Token.Divide :: tail -> (index >> term_pr) tail
            | _ -> tokens
        and index tokens = (factor >> index_pr) tokens
        and index_pr tokens =
            match tokens with
            | Token.Indice :: tail -> (factor >> index_pr) tail
            | _ -> tokens
        and factor tokens =
            match tokens with
            | Token.Number value   :: tail -> tail
            | Token.Variable value :: tail -> tail
            | Token.L_Bracket ::tail -> match expr tail with
                                        | Token.R_Bracket :: tail -> tail
                                        | _ -> raise (ParseError $"Missing closing bracket: {showExceptionPosition(ogTokens, tail.Length)}")
            | _ ->         
                raise (ParseError $"Expected number or variable here: {showExceptionPosition(ogTokens, ogTokens.Length - tokens.Length)}")
        let out = assign tokens
        if out.IsEmpty then
            out
        else
            raise (ParseError $"Unexpected token here: {showExceptionPosition(tokens, tokens.Length - out.Length)}")
