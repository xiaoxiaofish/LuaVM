using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.Paser
{
    public class Paser
    {
        private TokenReader tokenReader;
        private Lexer.Lexer lexer;
        public Paser(string codeFilePath,string keyWordFilePath)
        {
            lexer = new Lexer.Lexer(codeFilePath, keyWordFilePath);
            lexer.StartLexer();
            tokenReader = new TokenReader(lexer.TokenList);
        }

        public void StartPaser()
        {

        }
    }
}
