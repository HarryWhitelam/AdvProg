// parser.fs
//
// Author:      Irie Railton
// Description: 

//*************************************************************************

namespace Backend

//*************************************************************************

module Parser =

    type ParseError (message:string) = inherit System.Exception(message)

    //broken new grammar incl. matrices in standard BNF
    //<assign>      ::= <expr>      | Variable:=<expr>
    //<expr>        ::= <mat_expr>  | <scl_expr>
    //<mat_expr>    ::= <mat_unary> | <mat_expr>+<mat_unary>    | <mat_expr>-<mat_term>     | <mat_expr>+<scl_expr> | <mat_expr>-<scl_expr>
    //<mat_unary>   ::= -<mat_term> | <mat_term>
    //<mat_term>    ::= <mat_index> | <mat_term>*<mat_index>    | <mat_term>*<scl_expr>     | <mat_term>/<scl_expr>
    //<mat_index>   ::= <mat_fun>   | <mat_index>^<scl_expr>
    //<mat_fun>     ::= <matrix>    | Function(<mat_fun_arg>)
    //<mat_fun_arg> ::= <matrix>    | <mat_fun_arg>,<matrix>
    //<matrix>      ::= [<mat_arg>]
    //<mat_arg>     ::= <final>     | <mat_arg>,<final>         | <mat_arg>;<final>
    //<scl_expr>    ::= <scl_unary> | <scl_expr>+<scl_unary>    | <scl_expr>-<scl_unary>
    //<scl_unary>   ::= -<scl_term> | <scl_term>
    //<scl_term>    ::= <scl_index> | <scl_term>*<scl_index>    | <scl_term>/<scl_index>  NEED TO ADD MATRIX DOT PRODUCT SOMEWHERE
    //<scl_index>   ::= <scl_fun>   | <scl_index>^<scl_fun>
    //<scl_fun>     ::= <final>     | Function(<scl_fun_arg>)
    //<scl_fun_arg> ::= <final>     | <scl_fun_arg>,<final>
    //<final>       ::= Number      | Variable                  | (<scl_expr>)              | <scl_fun>

    //broken new grammar incl. matrices in LL(1) BNF
    //<assign>      ::= <expr>                  | Variable:=<expr>
    //<expr>        ::= <mat_expr>              | <scl_expr>
    //<mat_expr>    ::= <mat_unary><mat_expr'>
    //<mat_expr'>   ::= +<mat_unary><mat_expr'> | -<mat_term><mat_expr'>    | +<scl_expr><mat_expr'>| -<scl_expr><mat_expr'> | ∅
    //<mat_unary>   ::= -<mat_term>             | <mat_term>
    //<mat_term>    ::= <mat_index><mat_term'>
    //<mat_term'>   ::= *<mat_index><mat_term'> | *<scl_expr><mat_term'>    | /<scl_expr><mat_term'>| ∅
    //<mat_index>   ::= <mat_fun><mat_index'>
    //<mat_index'>  ::= ^<scl_expr><mat_index'> | ∅
    //<mat_fun>     ::= <matrix>                | Function(<mat_fun_arg>)
    //<mat_fun_arg> ::= <matrix><mat_fun_arg'>
    //<mat_fun_arg'>::= ,<matrix><mat_fun_arg'> | ∅
    //<matrix>      ::= [<mat_arg>]
    //<mat_arg>     ::= <final><mat_arg'>
    //<mat_arg'>    ::= ,<final><mat_arg'>      | ;<final><mat_arg'>        | ∅
    //<scl_expr>    ::= <scl_unary><scl_expr'>
    //<scl_expr'>   ::=+<scl_unary><scl_expr'>  | -<scl_unary><scl_expr'>   | ∅
    //<scl_unary>   ::= -<scl_term>             | <scl_term>
    //<scl_term>    ::= <scl_index><scl_term'>
    //<scl_term'>   ::=*<scl_index><scl_term'>  | /<scl_index><scl_term'>   | ∅  NEED TO ADD MATRIX DOT PRODUCT SOMEWHERE
    //<scl_index>   ::= <scl_fun><scl_index'>
    //<scl_index'>  ::= ^<scl_fun><scl_index'>  | ∅
    //<scl_fun>     ::= <final>                 | Function(<scl_fun_arg>)
    //<scl_fun_arg> ::= <final><scl_fun_arg'>
    //<scl_fun_arg'>::=,<final><scl_fun_arg'>   | ∅
    //<final>       ::= Number                  | Variable                  | (<scl_expr>)          | <scl_fun> 


    //broken new grammar v2 incl. matrices in standard BNF
    //<assign>      ::= <expr>      | Variable:=<expr>
    //<expr>        ::= <mat_unary> | <expr>+<mat_unary>    | <expr>-<mat_term>     | <expr>+<scl_expr> | <expr>-<scl_expr>
    //<mat_unary>   ::= -<mat_term> | <mat_term>
    //<mat_term>    ::= <mat_index> | <mat_term>*<mat_index>| <mat_term>*<scl_expr> | <mat_term>/<scl_expr>
    //<mat_index>   ::= <func>      | <mat_index>^<scl_expr>
    //<func>        ::= <matrix>    | Function(<func_arg>)
    //<func_arg>    ::= <matrix>    | <func>,<func_arg>
    //<matrix>      ::= [<mat_arg>] | <mat_arg>
    //<mat_arg>     ::= <scl_expr>  | <mat_arg>,<scl_expr>  | <mat_arg>;<scl_expr>
    //<scl_expr>    ::= <scl_unary> | <scl_expr>+<scl_unary>| <scl_expr>-<scl_unary>
    //<scl_unary>   ::= -<scl_term> | <scl_term>
    //<scl_term>    ::= <scl_index> | <scl_term>*<scl_index>| <scl_term>/<scl_index>  NEED TO ADD MATRIX DOT PRODUCT SOMEWHERE
    //<scl_index>   ::= <final>     | <scl_index>^<final>
    //<final>       ::= Number      | Variable              | (<scl_expr>)

    //broken new grammar v2 incl. matrices in LL(1) BNF
    //<assign>      ::= <expr>                  | Variable:=<expr>
    //<expr>        ::= <mat_unary><expr'>
    //<expr'>       ::= +<mat_unary><expr'>     | -<mat_term><expr'>        | +<scl_expr><expr'>    | -<scl_expr><expr'> | ∅
    //<mat_unary>   ::= -<mat_term>             | <mat_term>
    //<mat_term>    ::= <mat_index><mat_term'>
    //<mat_term'>   ::= *<mat_index><mat_term'> | *<scl_expr><mat_term'>    | /<scl_expr><mat_term'>| ∅
    //<mat_index>   ::= <func><mat_index'>
    //<mat_index'>  ::= ^<scl_expr><mat_index'> | ∅
    //<func>        ::= <matrix>                | Function(<func_arg>)
    //<func_arg>    ::= <matrix><func_arg'>
    //<func_arg'>   ::= ,<func><func_arg'>      | ∅
    //<matrix>      ::= [<mat_arg>]             | <mat_arg>
    //<mat_arg>     ::= <final><mat_arg'>
    //<mat_arg'>    ::= ,<scl_expr><mat_arg'>   | ;<scl_expr><mat_arg'>     | ∅
    //<scl_expr>    ::= <scl_unary><scl_expr'>
    //<scl_expr'>   ::=+<scl_unary><scl_expr'>  | -<scl_unary><scl_expr'>   | ∅
    //<scl_unary>   ::= -<scl_term>             | <scl_term>
    //<scl_term>    ::= <scl_index><scl_term'>
    //<scl_term'>   ::=*<scl_index><scl_term'>  | /<scl_index><scl_term'>   | ∅  NEED TO ADD MATRIX DOT PRODUCT SOMEWHERE
    //<scl_index>   ::= <final><scl_index'>
    //<scl_index'>  ::= ^<final><scl_index'>    | ∅
    //<final>       ::= Number                  | Variable                  | (<scl_expr>)

    //broken new grammar v3 incl. matrices in standard BNF
    //<assign>  ::= <expr>      | Variable:=<expr>
    //<expr>    ::= <unary>     | <expr>+<unary>    | <expr>-<term>
    //<unary>   ::= -<term>     | <term>
    //<term>    ::= <index>     | <term>*<index>    | <term>/<index>
    //<index>   ::= <funcall>   | <index>^<funcall>
    //<funcall> ::= <matfin>    | Function(<funarg>)
    //<funarg>  ::= <matfin>    | <funarg>,<matfin>
    //<matfin>  ::= Number      | Variable          | (<expr>) | <funcall> | <matexpr>
    //<matexpr> ::= <matfunc>   | <matexpr>.<matfunc>
    //<matfunc> ::= <matrix>    | Function(<matfunarg>)
    //<matfunarg>::=<matfunc>   | <matfunarg>,<matfunc>
    //<matrix>  ::= [<mat_arg>]
    //<matarg>  ::= <scalfin>   | <matarg>,<scalfin>| <matarg>;<scalfin>
    //<scalfin> ::= Number      | Variable          | <funcall>

    //Final Grammar in Standard BNF
    //<assign>      ::= <mat_expr>  | Variable:=<mat_expr>
    //<mat_expr>    ::= <mat_term>  | <mat_expr>+<mat_term> | <mat_expr>-<mat_term>
    //<mat_term>    ::= <mat_index> | <mat_term>*<mat_index>| <mat_term>.<mat_index>
    //<mat_index>   ::= <func>      | <mat_index>^<func>
    //<func>        ::= <matrix>    | Function(<func_arg>)  | (<mat_expr>)
    //<func_arg>    ::= <matrix>    | <func>,<func_arg>
    //<matrix>      ::= [<mat_arg>] | <mat_arg>             | -[<mat_arg>]
    //<mat_arg>     ::= <scl_expr>  | <mat_arg>,<scl_expr>  | <mat_arg>;<scl_expr>
    //<scl_expr>    ::= <scl_unary> | <scl_expr>+<scl_unary>| <scl_expr>-<scl_term>
    //<scl_unary>   ::= -<scl_term> | <scl_term>
    //<scl_term>    ::= <scl_index> | <scl_term>*<scl_index>| <scl_term>/<scl_index>
    //<scl_index>   ::= <final>     | <scl_index>^<final>
    //<final>       ::= Number      | Variable              | Reserved              | (<scl_expr>)

    //Final Grammar in LL(1) BNF
    //<assign>      ::= <mat_expr>              | Variable:=<mat_expr>
    //<mat_expr>    ::= <mat_term><mat_expr'>
    //<mat_expr'>   ::= +<mat_term><mat_expr'> | -<mat_term><mat_expr'>    | ∅
    //<mat_term>    ::= <mat_index><mat_term'>
    //<mat_term'>   ::= *<mat_index><mat_term'> | .<mat_index><mat_term'>   | ∅
    //<mat_index>   ::= <func><mat_index'>
    //<mat_index'>  ::= ^<func><mat_index'>     | ∅
    //<func>        ::= <matrix>                | Function(<func_arg>)      | (<mat_expr>)
    //<func_arg>    ::= <matrix><func_arg'>
    //<func_arg'>   ::= ,<func><func_arg'>      | ∅
    //<matrix>      ::= [<mat_arg>]             | <mat_arg>
    //<mat_arg>     ::= <scl_expr>><mat_arg'>
    //<mat_arg'>    ::= ,<scl_expr><mat_arg'>   | ;<scl_expr><mat_arg'>     | ∅
    //<scl_expr>    ::= <scl_unary><scl_expr'>
    //<scl_expr'>   ::= +<scl_unary><scl_expr'> | -<scl_term><scl_expr>     | ∅
    //<scl_unary>   ::= -<scl_term>             | <scl_term>
    //<scl_term>    ::= <scl_index><scl_term'>
    //<scl_term'>   ::= *<scl_index><scl_term'> | /<scl_index><scl_term'>   | ∅
    //<scl_index>   ::= <final><scl_index'>
    //<scl_index'>  ::= ^<final><scl_index'>    | ∅
    //<final>       ::= Number                  | Variable                  | Reserved  | (<scl_expr>)

    let parse (tokens: Token list) =
        let ogTokens = tokens
        let rec assign tokens =
            System.Diagnostics.Debug.WriteLine($"assign tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.Variable _value :: Token.Assign :: tail -> mat_expr tail
            | Token.Reserved name :: Token.Assign :: _tail -> 
                raise (ParseError $"{name} is a reserved word. It cannot be assigned to")
            | Token.Function name :: Token.Assign :: _tail ->
                raise (ParseError $"{name} is a reserved word. It cannot be assigned to")
            | _ :: Token.Assign :: _tail -> 
                raise (ParseError $"Expected variable before assignment: {Token.pointToToken(ogTokens, 0)}")
            | _ -> mat_expr tokens
        and mat_expr tokens = (mat_term >> mat_expr_pr) tokens
        and mat_expr_pr tokens =
            System.Diagnostics.Debug.WriteLine($"mat_expr_pr tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.Plus  :: tail -> (mat_term >> mat_expr_pr) tail
            | Token.Minus :: tail -> (mat_term >> mat_expr_pr) tail
            | _ -> tokens
        //and mat_unary tokens = 
        //    System.Diagnostics.Debug.WriteLine($"mat unary tokens={Token.printTokens(tokens)}")
        //    match tokens with
        //    | Token.Minus :: tail -> mat_term tail
        //    | _ -> mat_term tokens
        and mat_term tokens = (mat_index >> mat_term_pr) tokens
        and mat_term_pr tokens =
            System.Diagnostics.Debug.WriteLine($"mat_term_pr tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.Times :: tail -> (mat_index >> mat_term_pr) tail
            | Token.Dot   :: tail -> (mat_index >> mat_term_pr) tail
            | _ -> tokens
        and mat_index tokens = (func >> mat_index_pr) tokens
        and mat_index_pr tokens = 
            System.Diagnostics.Debug.WriteLine($"mat_index_pr tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.Indice :: tail -> (func >> mat_index_pr) tail
            | _ -> tokens
        and func tokens =
            System.Diagnostics.Debug.WriteLine($"func tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.Function _value :: tail ->  match tail with 
                                                | Token.L_Parenth :: tail2->match func_arg tail2 with
                                                                            | Token.R_Parenth :: tail3 -> tail3
                                                                            | _ -> raise (ParseError $"Missing closing parenthesis: {Token.pointToToken(ogTokens, tail.Length+1)}")
                                                | _ -> raise (ParseError $"Missing opening parenthesis: {Token.pointToToken(ogTokens, tail.Length+1)}")
            | Token.L_Parenth :: tail ->match mat_expr tail with
                                        | Token.R_Parenth :: tail2 -> tail2
                                        | _ -> raise (ParseError $"Missing closing parenthesis: {Token.pointToToken(ogTokens, tail.Length+1)}")
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
            | Token.Minus :: Token.L_Bracket :: tail -> match mat_arg tail with
                                                        | Token.R_Bracket :: tail2 -> tail2
                                                        | toks -> raise (ParseError $"Unexpected {toks.[0]} here: {Token.pointToToken(ogTokens, ogTokens.Length-toks.Length)}")
            | _ -> mat_arg tokens
        and mat_arg tokens = (scl_expr >> mat_arg_pr) tokens
        and mat_arg_pr tokens =
            System.Diagnostics.Debug.WriteLine($"mat_arg_pr tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.Comma     :: tail -> (scl_expr >> mat_arg_pr) tail
            | Token.SemiColon :: tail -> (scl_expr >> mat_arg_pr) tail
            | _ -> tokens
        and scl_expr tokens = (scl_unary >> scl_expr_pr) tokens
        and scl_expr_pr tokens =
            System.Diagnostics.Debug.WriteLine($"scl_expr_pr tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.Plus  :: tail -> (scl_unary >> scl_expr_pr) tail
            | Token.Minus :: tail -> (scl_term >> scl_expr_pr) tail
            | _ -> tokens
        and scl_unary tokens =
            System.Diagnostics.Debug.WriteLine($"sl_unary tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.Minus :: tail -> scl_term tail
            | _ -> scl_term tokens
        and scl_term tokens = (scl_index >> scl_term_pr) tokens
        and scl_term_pr tokens =
            System.Diagnostics.Debug.WriteLine($"scl_term_pr tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.Times  :: tail -> (scl_index >> scl_term_pr) tail
            | Token.Divide :: tail -> (scl_index >> scl_term_pr) tail
            | _ -> tokens
        and scl_index tokens = (final >> scl_index_pr) tokens
        and scl_index_pr tokens =
            System.Diagnostics.Debug.WriteLine($"scl_index_pr tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.Indice :: tail -> (final >> scl_index_pr) tail
            | _ -> tokens
        and final tokens =
            System.Diagnostics.Debug.WriteLine($"final tokens={Token.printTokens(tokens)}")
            match tokens with
            | Token.Number _value   :: tail -> tail
            | Token.Variable _value :: tail -> tail
            | Token.Reserved _value :: tail -> tail
            | Token.L_Parenth :: tail ->match scl_expr tail with
                                        | Token.R_Parenth :: tail2 -> tail2
                                        | Token.Comma     :: tail2 -> raise (ParseError $"Unexpected comma here: {Token.pointToToken(ogTokens, ogTokens.Length-tail2.Length-1)}")
                                        | toks -> raise (ParseError $"Missing closing parenthesis: {Token.pointToToken(ogTokens, tail.Length+1)} \n toks = {Token.printTokens(toks)}")
            | _ -> raise (ParseError $"Expected number, variable, reserved word, or opening parenthesis here: {Token.pointToToken(ogTokens, ogTokens.Length - tokens.Length)}")
        let out = assign tokens
        if out.IsEmpty then
            out
        else
            raise (ParseError $"Unexpected token here: {Token.pointToToken(tokens, tokens.Length - out.Length)}")