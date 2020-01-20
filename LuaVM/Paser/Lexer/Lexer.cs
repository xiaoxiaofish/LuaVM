using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.Paser.Lexer
{
    public class Lexer
    {
        private List<string> codeStringList;
        private int index;
        private int line;
        private ILexerState lexerState;
        private List<Token> tokenList;
        public int Line { get => line; }
        public ILexerState LexerState { get => lexerState; set => lexerState = value; }
        public List<Token> TokenList { get => tokenList;}


        public Lexer(string codeFilePath, string keyWordFilePath)
        {
            codeStringList = new List<string>();
            using (StreamReader streamReader = new StreamReader(codeFilePath))
            {
                string codeString = streamReader.ReadToEnd();
                codeStringList = codeString.Split('\n').ToList<string>();
            }
            lexerState = ILexerState.startLexerState;
            lexerState.KeyWordPath = keyWordFilePath;
            line = 0;
            tokenList = new List<Token>();
            index = 0;
        }

        public void AddToken(Token token)
        {
            TokenList.Add(token);
        }

        //获取缓冲中获取一个单字符
        public char GetChar()
        {
            if (index < codeStringList[line].Length)
                return codeStringList[line][index++];
            else if (line < codeStringList.Count - 1)
            {
                index = 0;
                return codeStringList[++line][index++];
            }
            else
            {
                lexerState.EndLexer(this);
                throw new Exception("词法分析完成！");
            }
        }

        public void Undo()
        {
            index--;
        }

        public void PrintAllToken()
        {
            foreach (var token in TokenList)
            {
                Console.WriteLine(token.TokenType + ":" + token.TokenValue + " " + token.Line);
            }
        }

        public string Error(string errorInfo)
        {
            return errorInfo;
        }

        //放弃这一行的所有代码，读取下一行
        public void NextLine()
        {
            line++;
        }

        //开始词法分析
        public void StartLexer()
        {
            while (true)
            {
                try
                {
                    lexerState.UpdateState(this);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }
            }
            Token endToken = new Token(TokenType.Eof, "", line);
            tokenList.Add(endToken);
        }
    }
}
