// parser.fs
//
// Author:      Irie Railton
// Description: 

//*************************************************************************

namespace interpreter

//*************************************************************************

//Grammar in standard BNF
//<expr>    ::= <term>    | <expr>+<term>   | <expr>-<term>
//<term>    ::= <unary>   | <term>*<unary>  | <term>/<unary>
//<index>   ::= <unary>   | <index>^<unary>
//<unary>   ::= -<factor> | <factor>
//<factor>  ::= Number    | (<expr>)

//Grammar in LL(1) BNF
//<expr>    ::= <term><expr'>
//<expr'>   ::= +<term><expr'> | -<term><expr'> | ∅
//<term>    ::= <index><term'>
//<term'>   ::= *<index><term'> | /<index><term'> | ∅
//<index>   ::= <unary><index'>
//<index'>  ::= ^<unary><index'> | ∅
//<unary>   ::= -<factor> | <factor>
//<factor>  ::= Number | Variable | (<expr>)