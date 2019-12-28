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

        public TokenReader(List<Token> tokenList)
        {
            this.tokenList = tokenList;
            position = 0;
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

        public Token Peek()
        {
            if (position < tokenNum)
            {
                return tokenList[position];
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
