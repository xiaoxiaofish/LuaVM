using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.Paser.Lexer
{
    public abstract class ILexerState
    {
        protected static char ch;
        private static string keyWordPath;
        protected static List<char> tokenText = new List<char>();
        public static StartLexerState startLexerState = new StartLexerState();
        protected static NumberLexerState numberLexerState = new NumberLexerState();
        protected static IdentifierLexerState identifierLexerState = new IdentifierLexerState();
        protected static DoubleOperatorLexerState doubleOperatorLexerState = new DoubleOperatorLexerState();

        public string KeyWordPath
        {
            get => keyWordPath;
            set
            {
                using (StreamReader streamReader = new StreamReader(value))
                {
                    while (!streamReader.EndOfStream)
                    {
                        keyWordDic.Add(streamReader.ReadLine(), 0);
                    }
                    keyWordPath = value;
                }
            }
        }

        protected static Dictionary<string, int> keyWordDic = new Dictionary<string, int>();

        public void EndLexer(Lexer lexer)
        {
            tokenText.Add(' ');
            EnterEndState(lexer);
        }
        public void UpdateState(Lexer lexer)
        {
            try
            {
                ReadChar(lexer);
            }
            catch(Exception e)
            {
                throw e;
            }
            switch (ch)
            {
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    EnterEndState(lexer);
                    break;
                case '>':
                case '<':
                case '=':
                case '+':
                case '-':
                case '*':
                case '/':
                case '.':
                case '~':
                case ':':
                    EnterDoubleOperationState(lexer);
                    break;
                case '(':
                case ')':
                case ';':
                case ',':
                case '[':
                case ']':
                case '{':
                case '}':
                case '#':
                    EnterSingleOperationState(lexer);
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    EnterNumberState(lexer);
                    break;
                case '\'':
                case '\"':
                    EnterString(lexer);
                    break;
                default:
                    EnterIdentifierState(lexer);
                    break;
            }
        }
        //报错接口
        protected virtual void Error(Lexer lexer)
        {

        }
        //读取到'或者"按字符串处理
        protected virtual void EnterString(Lexer lexer)
        {
            Separator(lexer);
            char temp = ch;
            char flag = ' ';
            if (ch == '\'')
            {
                flag = '\"';
            }
            else
            {
                flag = '\'';
            }
            PopLastChar();
            while (true)
            {
                try
                {
                    ReadChar(lexer);
                    if (ch == '\\')
                    {
                        ReadChar(lexer);
                    }
                    else if (ch == temp)
                    {
                        PopLastChar();
                        Token token = new Token(TokenType.String, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        break;
                    }
                    else if (ch != temp && ch == flag)
                    {
                        throw new Exception("语法错误！ 在第" + lexer.Line + "行 " + ch + "未能正确匹配");
                    }
                    else
                    {
                        continue;
                    }
                }
                catch (Exception e)
                {
                    if (e.Message == null)
                    {
                        throw new Exception("语法错误！ 在第" + lexer.Line + "行。 \'或者\"未能正确匹配");
                    }
                    else
                    {
                        throw e;
                    }
                }
            }
        }
        //字符处理接口，所有子类通过重写这个方法实现读取字符时如何处理字符
        protected abstract void Separator(Lexer lexer);

        //所有二元操作符的处理接口
        protected virtual void EnterDoubleOperationState(Lexer lexer)
        {
            Separator(lexer);
            lexer.LexerState = doubleOperatorLexerState;
        }

        //读取到' '和'\t'字符
        protected virtual void EnterEndState(Lexer lexer)
        {
            Separator(lexer);
            lexer.LexerState = startLexerState;
            PopLastChar();
        }

        //所有一元操作符的处理接口
        protected void EnterSingleOperationState(Lexer lexer)
        {
            Separator(lexer);
            switch (ch)
            {
                case ',':
                    {
                        Token token = new Token(TokenType.Comma, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        break;
                    }
                case ';':
                    {
                        Token token = new Token(TokenType.SemiColon, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        break;
                    }
                case '(':
                    {
                        Token token = new Token(TokenType.LeftParen, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        break;
                    }
                case ')':
                    {
                        Token token = new Token(TokenType.RightParen, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        break;
                    }
                case '[':
                    {
                        Token token = new Token(TokenType.LeftSquare, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        break;
                    }
                case ']':
                    {
                        Token token = new Token(TokenType.RightSquare, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        break;
                    }
                case '{':
                    {
                        Token token = new Token(TokenType.LeftBig, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        break;
                    }
                case '}':
                    {
                        Token token = new Token(TokenType.RightBig, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        break;
                    }
                case '#':
                    {
                        Token token = new Token(TokenType.Len, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        break;
                    }
            }
            lexer.LexerState = startLexerState;
        }

        //读取到数字字符
        protected virtual void EnterNumberState(Lexer lexer)
        {
            Separator(lexer);
            lexer.LexerState = numberLexerState;
        }

        //读取到平凡字符
        protected virtual void EnterIdentifierState(Lexer lexer)
        {
            Separator(lexer);
            lexer.LexerState = identifierLexerState;
        }

        //将字符缓冲转化为double
        protected double GetTokenNumberValue()
        {
            double num = double.Parse(tokenText.ToString());
            tokenText.Clear();
            return num;
        }

        //将字符缓冲转换为字符串
        protected string GetTokenStringText()
        {
            string str = new string(tokenText.ToArray());
            tokenText.Clear();
            return str;
        }

        //读取一个字符，并放入字符缓冲中
        protected void ReadChar(Lexer lexer)
        {
            try
            {
                ch = lexer.GetChar();
                tokenText.Add(ch);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        //判断该token是否为关键字
        protected bool IsKeyWord(string token)
        {
            return keyWordDic.ContainsKey(token);
        }

        //弹出第一个字符
        protected void PopLastChar()
        {
            tokenText.RemoveAt(tokenText.Count - 1);
        }

    }

    public class StartLexerState : ILexerState
    {
        protected override void Separator(Lexer lexer)
        {

        }




    }

    public class NumberLexerState : ILexerState
    {
        int Commcount = 0;
        protected override void Separator(Lexer lexer)
        {
            if(ch == '.' && Commcount == 0)
            {
                Commcount = 1;
                return;
            }
            PopLastChar();
            Token token = new Token(TokenType.Number, GetTokenStringText(), lexer.Line);
            tokenText.Add(ch);
            lexer.AddToken(token);
            Commcount = 0;
        }
        protected override void EnterNumberState(Lexer lexer)
        {

        }

        protected override void EnterDoubleOperationState(Lexer lexer)
        {
            if (ch == '.' && Commcount == 0)
            {
                return;
            }
            else
            {
                base.EnterDoubleOperationState(lexer);
            }
        }
    }

    public class IdentifierLexerState : ILexerState
    {
        protected override void Separator(Lexer lexer)
        {
            PopLastChar();
            string identifier = GetTokenStringText();
            if (IsKeyWord(identifier))
            {
                string UpIdentifier = identifier.Substring(0, 1).ToUpper() + identifier.Substring(1, identifier.Length - 1);
                TokenType type = (TokenType)Enum.Parse(typeof(TokenType), UpIdentifier);
                Token token = new Token(type, identifier, lexer.Line);
                tokenText.Add(ch);
                lexer.AddToken(token);
            }
            else
            {
                Token token = new Token(TokenType.Identifier, identifier, lexer.Line);
                tokenText.Add(ch);
                lexer.AddToken(token);
            }
        }

        protected override void EnterIdentifierState(Lexer lexer)
        {

        }

        protected override void EnterNumberState(Lexer lexer)
        {

        }
    }

    public class DoubleOperatorLexerState : ILexerState
    {
        protected override void Separator(Lexer lexer)
        {
            string symbol = new string(tokenText.ToArray());
            switch (symbol)
            {
                case "++":
                    {
                        Token token = new Token(TokenType.DoublePlus, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        return;
                    }
                case "--":
                    {
                        Token token = new Token(TokenType.DoubleMinus, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        return;
                    }
                case "==":
                    {
                        Token token = new Token(TokenType.Equal, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        return;
                    }
                case "-=":
                    {
                        Token token = new Token(TokenType.MinusAssignment, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        return;
                    }
                case "+=":
                    {
                        Token token = new Token(TokenType.PlusAssignment, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        return;
                    }
                case "*=":
                    {
                        Token token = new Token(TokenType.StarAssignment, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        return;
                    }
                case "/=":
                    {
                        Token token = new Token(TokenType.SlashAssignment, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        return;
                    }
                case "//":
                    {
                        lexer.NextLine();
                        tokenText.Clear();
                        lexer.LexerState = startLexerState;
                        return;
                    }
                case "..":
                    {
                        Token token = new Token(TokenType.Connect, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        return;
                    }
                case ">=":
                    {
                        Token token = new Token(TokenType.BiggerEqual, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        return;
                    }
                case "<=":
                    {
                        Token token = new Token(TokenType.SmallerEqual, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        return;
                    }
                case "~=":
                    {
                        Token token = new Token(TokenType.NotEqual, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        return;
                    }
                case "::":
                    {
                        Token token = new Token(TokenType.DoubleLable, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        return;
                    }
                default:
                    break;
            }
            char firstCh = tokenText[0];
            switch (firstCh)
            {
                case '+':
                    {
                        Token token = new Token(TokenType.Plus, firstCh.ToString(), lexer.Line);
                        lexer.AddToken(token);
                        break;
                    }
                case '-':
                    {
                        Token token = new Token(TokenType.Minus, firstCh.ToString(), lexer.Line);
                        lexer.AddToken(token);
                        break;
                    }
                case '*':
                    {
                        Token token = new Token(TokenType.Dawn, firstCh.ToString(), lexer.Line);
                        lexer.AddToken(token);
                        break;
                    }
                case '/':
                    {
                        Token token = new Token(TokenType.Star, firstCh.ToString(), lexer.Line);
                        lexer.AddToken(token);
                        break;
                    }
                case '=':
                    {
                        Token token = new Token(TokenType.Assignment, firstCh.ToString(), lexer.Line);
                        lexer.AddToken(token);
                        break;
                    }
                case '~':
                    {
                        Token token = new Token(TokenType.Error, firstCh.ToString(), lexer.Line);
                        lexer.AddToken(token);
                        break;
                    }
                case ':':
                    {
                        Token token = new Token(TokenType.SingleLable, firstCh.ToString(), lexer.Line);
                        lexer.AddToken(token);
                        break;
                    }
            }
            char secondChar = tokenText[1];
            switch (secondChar)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    {
                        tokenText.Clear();
                        tokenText.Add(secondChar);
                        lexer.LexerState = numberLexerState;
                        break;
                    }
                case ',':
                    {
                        Token token = new Token(TokenType.Comma, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        break;
                    }
                case ';':
                    {
                        Token token = new Token(TokenType.SemiColon, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        break;
                    }
                case '(':
                    {
                        Token token = new Token(TokenType.LeftParen, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        break;
                    }
                case ')':
                    {
                        Token token = new Token(TokenType.RightParen, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        break;
                    }
                case '[':
                    {
                        Token token = new Token(TokenType.LeftSquare, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        break;
                    }
                case ']':
                    {
                        Token token = new Token(TokenType.RightSquare, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        break;
                    }
                case '{':
                    {
                        Token token = new Token(TokenType.LeftBig, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        break;
                    }
                case '}':
                    {
                        Token token = new Token(TokenType.RightBig, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        break;
                    }
                case '#':
                    {
                        Token token = new Token(TokenType.Len, GetTokenStringText(), lexer.Line);
                        lexer.AddToken(token);
                        lexer.LexerState = startLexerState;
                        break;
                    }
                default:
                    {
                        tokenText.Clear();
                        tokenText.Add(secondChar);
                        lexer.LexerState = identifierLexerState;
                        break;
                    }
            }
        }

        protected override void EnterEndState(Lexer lexer)
        {
            char firstCh = tokenText[0];

            switch (firstCh)
            {
                case '+':
                    {
                        Token token = new Token(TokenType.Plus, firstCh.ToString(), lexer.Line);
                        lexer.AddToken(token);
                        break;
                    }
                case '-':
                    {
                        Token token = new Token(TokenType.Minus, firstCh.ToString(), lexer.Line);
                        lexer.AddToken(token);
                        break;
                    }
                case '*':
                    {
                        Token token = new Token(TokenType.Dawn, firstCh.ToString(), lexer.Line);
                        lexer.AddToken(token);
                        break;
                    }
                case '/':
                    {
                        Token token = new Token(TokenType.Star, firstCh.ToString(), lexer.Line);
                        lexer.AddToken(token);
                        break;
                    }
                case '=':
                    {
                        Token token = new Token(TokenType.Assignment, firstCh.ToString(), lexer.Line);
                        lexer.AddToken(token);
                        break;
                    }
            }
            tokenText.Clear();
            lexer.LexerState = startLexerState;
        }

        protected override void EnterDoubleOperationState(Lexer lexer)
        {
            Separator(lexer);
        }
    }

}
