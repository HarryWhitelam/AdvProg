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
    //<assign>  ::= <expr>      | Variable:=<expr>
    //<expr>    ::= <unary>     | <expr>+<unary>      | <expr>-<term>
    //<unary>   ::= -<term>     | <term>
    //<term>    ::= <index>     | <term>*<index>      | <term>/<index>
    //<index>   ::= <funcall>   | <index>^<funcall>
    //<funcall> ::= <final>     | Function(<args>) | Function()
    //<args>    ::= <final>     | <final> , <args> | ∅
    //<final>   ::= Number | Variable | (<expr>) | <funcall> | [<args>]

    //Grammar in LL(1) BNF
    //<assign>  ::= <expr> | Variable:=<expr>
    //<expr>    ::= <unary><expr'>
    //<expr'>   ::= +<unary><expr'> | -<term><expr'> | ∅
    //<unary>   ::= -<term> | <term>
    //<term>    ::= <index><term'>
    //<term'>   ::= *<index><term'> | /<index><term'> | ∅
    //<index>   ::= <funcall><index'>
    //<index'>  ::= ^<funcall><index'> | ∅
    //<funcall> ::= <final> | Function(<args>) | Function()
    //<args>    ::= <final><args'>
    //<args'>   ::= ,<final><args'> | ∅
    //<final>   ::= Number | Variable | (<expr>) | <funcall> | [<args>]

    let parse (tokens: Token list) =
        let ogTokens = tokens
        let rec assign tokens = 
            match tokens with
            | Token.Variable _value :: Token.Assign :: tail -> expr tail
            | _ :: Token.Assign :: _tail -> 
                raise (ParseError $"Expected variable before assignment: {Token.pointToToken(ogTokens, 0)}")
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
        and index tokens = (funcall >> index_pr) tokens
        and index_pr tokens =
            match tokens with
            | Token.Indice :: tail -> (funcall >> index_pr) tail
            | _ -> tokens
        and funcall tokens =
            match tokens with
            | Token.Function _value :: tail ->  match tail with 
                                                | Token.L_Parenth :: tail2->match tail2 with
                                                                            | Token.R_Parenth :: tail3 -> tail3
                                                                            | _ ->  match args tail2 with
                                                                                    | Token.R_Parenth :: tail3 -> tail3
                                                                                    | _ -> raise (ParseError $"Missing closing parenthesis: {Token.pointToToken(ogTokens, tail.Length+1)}")
                                                | _ -> raise (ParseError $"Missing opening parenthesis: {Token.pointToToken(ogTokens, tail.Length+1)}")
            | _ -> final tokens
        and args tokens = (final >> args_pr) tokens
        and args_pr tokens =
            match tokens with
            | Token.Comma :: tail -> (final >> args_pr) tail
            | _ -> tokens
        and final tokens =
            match tokens with
            | Token.Number _value   :: tail -> tail
            | Token.Variable _value :: tail -> tail
            | Token.L_Parenth :: tail ->match expr tail with
                                        | Token.R_Parenth :: tail2 -> tail2
                                        | Token.Comma :: tail2 -> raise (ParseError $"Unexpected comma here: {Token.pointToToken(ogTokens, ogTokens.Length-tail2.Length-1)}")
                                        | toks -> raise (ParseError $"Missing closing parenthesis: {Token.pointToToken(ogTokens, tail.Length+1)} \n toks = {Token.printTokens(toks)}")
            | Token.Function value :: tail -> funcall (Token.Function value::tail)
            | Token.L_Bracket :: tail ->match args tail with
                                        | Token.R_Bracket :: tail2 -> tail2
                                        | toks -> raise (ParseError $"Missing closing bracket: {Token.pointToToken(ogTokens, tail.Length+1)} \n toks = {Token.printTokens(toks)}")
            | _ -> raise (ParseError $"Expected number, variable, or closing bracket here: {Token.pointToToken(ogTokens, ogTokens.Length - tokens.Length)}")
        let out = assign tokens
        if out.IsEmpty then
            out
        else
            raise (ParseError $"Unexpected token here: {Token.pointToToken(tokens, tokens.Length - out.Length)}")
