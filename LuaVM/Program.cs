using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM
{
    class Program
    {
        static void Main(string[] args)
        {
            Paser.Lexer.Lexer lexer = new Paser.Lexer.Lexer("Define/code.txt", "Define/keyWord.txt");
            lexer.StartLexer();
            lexer.PrintAllToken();
            Console.Read();
            //  StreamReader ss = new StreamReader("Define/s.txt");

            //   string s = ss.ReadToEnd();
            //   Console.WriteLine(s);
        }
    }
}
