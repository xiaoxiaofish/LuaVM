using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.Paser
{
    public class TokenReader
    {
        private List<Token> tokenList;
        private int position;
        readonly private int tokenNum;
        private int peekPosition;

        public Token ReadTokenUntil(TokenType type)
        {

            return null;
        }

        public bool IsBlockEnd(Token token)
        {
            switch (token.TokenType)
            {
                case TokenType.Else:
                case TokenType.Return:
                case TokenType.Elseif:
                case TokenType.End:
                case TokenType.Until:
                case TokenType.Eof:
                    return true;
            }
            return false;
        }
        public TokenReader(List<Token> tokenList)
        {
            this.tokenList = tokenList;
            position = 0;
            peekPosition = 0;
            tokenNum = tokenList.Count;
        }

        public Token ReadToken()
        {
            if(position < tokenNum)
            {
                return tokenList[position++];
            }
            else
            {
                return null;
            }
        }

        public void ResetPeekPosition()
        {
            peekPosition = position;
        }

        /// <summary>
        /// 无限前瞻，自动维护前瞻下标
        /// </summary>
        /// <returns></returns>
        public Token Peek()
        {
            if (peekPosition < tokenNum)
            {
                return tokenList[peekPosition++];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 前瞻一个token
        /// </summary>
        /// <returns></returns>
        public Token PeekOne()
        {
            if (position < tokenNum)
            {
                return tokenList[position + 1];
            }
            else
            {
                return null;
            }
        }

        public void UnRead()
        {
            position--;
        }
    }
}
