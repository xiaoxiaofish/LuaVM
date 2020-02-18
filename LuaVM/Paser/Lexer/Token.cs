using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.Paser
{
    public enum TokenType
    {
        Plus = 0,   // +
        Star,   // *
        Slash,  // /      
        Percent,//%
        Connect,// ..
        Minus,  // -
        Len,// #
        Error,//~

        SemiColon, // ;
        LeftParen, // (
        RightParen,// )
        Dawn, //.
        Comma,//,

        LeftSquare,//[
        RightSquare,//]
        LeftBig,//{
        RightBig,//}

        Smaller,     // <
        NotEqual,// ~=
        BiggerEqual, // >=
        Bigger,     // >
        Equal,     // ==
        SmallerEqual,   // <=
        SlashAssignment,// /=
        PlusAssignment, // +=
        MinusAssignment, // -=
        StarAssignment, // *=

        Assignment,// =

        OrOperation,// |
        DoublePlus,//++
        DoubleMinus,//--
        DoubleLable,// ::
        SingleLable,// :
        Vararg, // ...
        Identifier,     //标识符

        Self,
        Nil, //无效值
        Number,     //表示双精度类型的实浮点数
        String, //字符串字面量
        Boolean,//布尔类型
        For,//关键字
        While,
        Function,
        Userdata,
        Thread,
        Table,
        Local,
        If,
        Else,
        Elseif,
        Break,
        True,
        False,
        Do,
        Return,
        End,
        And,
        Not,
        Or,
        In,
        Until,
        Then,
        Repeat,
        Eof,

       //元方法
        __add,
        __sub,
        __mul,
        __mod,
        __pow,
        __div,
        __idiv,
        __band,
        __bor,
        __bxor,
        __shl,
        __shr,
        __unm,
        __bnot,
    }
    public class Token
    {
        private TokenType tokenType;
        private string tokenValue;
        private int line;

        public TokenType TokenType { get => tokenType; }
        public string TokenValue { get => tokenValue; }
        public int Line { get => line; }

        public Token(TokenType tokenType, string tokenValue, int line)
        {
            this.tokenType = tokenType;
            this.tokenValue = tokenValue;
            this.line = line;
        }
    }
}
