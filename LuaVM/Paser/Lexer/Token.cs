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
        Minus,  // -
        Star,   // *
        Slash,  // /
        Len,// #
        Error,//~
        NotEqual,// ~=
        BiggerEqual, // >=
        Bigger,     // >
        Equal,     // ==
        SmallerEqual,   // <=
        Smaller,     // <

        SemiColon, // ;
        LeftParen, // (
        RightParen,// )
        Dawn, //.
        Comma,//,

        LeftSquare,//[
        RightSquare,//]
        LeftBig,//{
        RightBig,//}

        SlashAssignment,// /=
        PlusAssignment, // +=
        MinusAssignment, // -=
        StarAssignment, // *=
        Assignment,// =
        DoublePlus,//++
        DoubleMinus,//--
        Connect,// ..
        Identifier,     //标识符

        Nil, //无效值
        Number,     //表示双精度类型的实浮点数
        String, //字符串字面量
        Boolean,//布尔类型
        Keyword,//关键字
    }
    public class Token
    {
        private TokenType tokenType;
        private string tokenValue;
        private int line;

        public TokenType TokenType { get => tokenType; }
        public string TokenValue { get => tokenValue;}
        public int Line { get => line;}

        public Token(TokenType tokenType, string tokenValue, int line)
        {
            this.tokenType = tokenType;
            this.tokenValue = tokenValue;
            this.line = line;
        }
    }
}
