// Parser.fs
//
// Author:      Irie Railton
// Description: LL parser for Baltam

namespace Backend

module Parser =

    type ParseError (message:string) = inherit System.Exception(message)

    //Final Grammar in Standard BNF
    //<assign>  ::= <expr>      | Variable:=<expr>
    //<expr>    ::= <unary>     | <expr>+<unary>        | <expr>-<term>
    //<unary>   ::= -<term>     | <term>    
    //<term>    ::= <index>     | <term>*<index>        | <term>.<index>    | <term>/<index>
    //<index>   ::= <func>      | <index>^<func>
    //<func>    ::= <matrix>    | Function(<func_arg>)
    //<func_arg>::= <matrix>    | <func>,<func_arg>
    //<matrix>  ::= [<mat_arg>] | <mat_arg>             | -[<mat_arg>]
    //<mat_arg> ::= <final>     | <mat_arg>,<final>    | <mat_arg>;<final>
    //<final>   ::= Number      | Variable              | Reserved          | (<expr>)

    //Final Grammar in LL(1) BNF
    //<assign>      ::= <expr>              | Variable:=<expr>
    //<expr>        ::= <unary><expr'>
    //<expr'>       ::= +<unary><expr'>     | -<term><expr'>    | ∅
    //<unary>       ::= -<term>             | <term>    
    //<term>        ::= <index><term'>
    //<term'>       ::= *<index><term'>     | /<index><term'>   | .<index><term'>   | ∅
    //<index>       ::= <func><index'>
    //<index'>      ::= ^<func><index'>     | ∅
    //<func>        ::= <matrix>            | Function(<func_arg>)
    //<func_arg>    ::= <matrix><func_arg'>
    //<func_arg'>   ::= ,<func><func_arg'>  | ∅
    //<matrix>      ::= [<mat_arg>]         | <mat_arg>
    //<mat_arg>     ::= <final>><mat_arg'>
    //<mat_arg'>    ::= ,<final><mat_arg'>  | ;<final><mat_arg'>| ∅,
    //<final>       ::= Number  | Variable  | Reserved  | (<expr>)

    let parse (tokens: Token list) =
        let ogTokens = tokens
        let rec assign tokens =
            System.Diagnostics.Debug.WriteLine($"assign tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.Variable _value :: Token.Assign :: tail -> expr tail
            | Token.Reserved name :: Token.Assign :: _tail -> 
                raise (ParseError $"{name} is a reserved word. It cannot be assigned to")
            | Token.Function name :: Token.Assign :: _tail ->
                raise (ParseError $"{name} is a reserved word. It cannot be assigned to")
            | _ :: Token.Assign :: _tail -> 
                raise (ParseError $"Expected variable before assignment: {Token.pointToToken(ogTokens, 0)}")
            | _ -> expr tokens
        and expr tokens = (unary >> expr_pr) tokens
        and expr_pr tokens =
            System.Diagnostics.Debug.WriteLine($"expr_pr tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.Plus   :: tail -> (unary >> expr_pr) tail
            | Token.Minus  :: tail -> (term >> expr_pr) tail
            | _ -> tokens
        and unary tokens =
            System.Diagnostics.Debug.WriteLine($"unary tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.Minus :: tail -> term tail
            | _ -> term tokens
        and term tokens = (index >> term_pr) tokens
        and term_pr tokens =
            System.Diagnostics.Debug.WriteLine($"term_pr tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.Times  :: tail -> (index >> term_pr) tail
            | Token.Divide :: tail -> (index >> term_pr) tail
            | Token.Dot    :: tail -> (index >> term_pr) tail
            | _ -> tokens
        and index tokens = (func >> index_pr) tokens
        and index_pr tokens = 
            System.Diagnostics.Debug.WriteLine($"index_pr tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.Indice :: tail -> (func >> index_pr) tail
            | _ -> tokens
        and func tokens =
            System.Diagnostics.Debug.WriteLine($"func tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.Function _value :: tail ->  match tail with 
                                                | Token.L_Parenth :: tail2->match func_arg tail2 with
                                                                            | Token.R_Parenth :: tail3 -> tail3
                                                                            | _ -> raise (ParseError $"Missing closing parenthesis: {Token.pointToToken(ogTokens, tail.Length+1)}")
                                                | _ -> raise (ParseError $"Missing opening parenthesis: {Token.pointToToken(ogTokens, tail.Length+1)}")
            | _ -> matrix tokens
        and func_arg tokens = (matrix >> func_arg_pr) tokens
        and func_arg_pr tokens =
            System.Diagnostics.Debug.WriteLine($"func_arg_pr tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.Comma :: tail -> (func >> func_arg_pr) tail
            | _ -> tokens
        and matrix tokens =
            System.Diagnostics.Debug.WriteLine($"matrix tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.L_Bracket :: tail ->match mat_arg tail with
                                        | Token.R_Bracket :: tail2 -> tail2
                                        | toks -> raise (ParseError $"Unexpected {toks.[0]} here: {Token.pointToToken(ogTokens, ogTokens.Length-toks.Length)}")
            | _ -> mat_arg tokens
        and mat_arg tokens = (final >> mat_arg_pr) tokens
        and mat_arg_pr tokens =
            System.Diagnostics.Debug.WriteLine($"mat_arg_pr tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.Comma     :: tail -> (final >> mat_arg_pr) tail
            | Token.SemiColon :: tail -> (final >> mat_arg_pr) tail
            | _ -> tokens
        and final tokens =
            System.Diagnostics.Debug.WriteLine($"final tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.Number _value   :: tail -> tail
            | Token.Variable _value :: tail -> tail
            | Token.Reserved _value :: tail -> tail
            | Token.L_Parenth :: tail ->match expr tail with
                                        | Token.R_Parenth :: tail2 -> tail2
                                        | Token.Comma     :: tail2 -> raise (ParseError $"Unexpected comma here: {Token.pointToToken(ogTokens, ogTokens.Length-tail2.Length-1)}")
                                        | toks -> raise (ParseError $"Missing closing parenthesis: {Token.pointToToken(ogTokens, tail.Length+1)} \n toks = {Token.printTokens(toks)}")
            | _ -> raise (ParseError $"Expected number, variable, reserved word, or opening parenthesis here: {Token.pointToToken(ogTokens, ogTokens.Length - tokens.Length)}")
        let out = assign tokens
        if out.IsEmpty then
            out
        else
            raise (ParseError $"Unexpected {out[0]} here: {Token.pointToToken(tokens, tokens.Length - out.Length)}")