//<unexpr>  ::= -<expr> | <expr>
//<expr>    ::= <term><expr'>
//<expr'>   ::= +<term><expr'> | -<term><expr'> | �
//<term>    ::= <index><term'>
//<term'>   ::= *<index><term'> | /<index><term'> | �
//<index>   ::= <factor><index'>
//<index'>  ::= ^<factor><index'> | �
//<factor>  ::= Number | Variable | (<unexpr>)

//<unexpr>  ::= -<expr> | <expr>
//<expr>    ::= <term>  | <expr>+<term> | <expr>-<term>
//<term>    ::= <index> | <term>*<index>| <term>/<index>
//<index>   ::= <factor>| <index>^<factor>
//<factor>  ::= Number  | Variable      | (<unexpr>)
