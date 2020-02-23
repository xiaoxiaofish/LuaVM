using LuaVM.Codegen;
using LuaVM.VM.LuaAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuaVM.Paser;
namespace LuaVM
{
    class Program
    {
        static void Main(string[] args)
        {
            //  Paser.Lexer.Lexer lexer = new Paser.Lexer.Lexer("Define/code.txt", "Define/keyWord.txt");
            //   lexer.StartLexer();
            //  lexer.PrintAllToken();
            Paser.Paser paser = new Paser.Paser("Define/code.txt", "Define/keyWord.txt");
            paser.StartPaser();
            CodeGenerator codeGenerator = new CodeGenerator();
            codeGenerator.GenPrototype(paser.ChunckNode.Block);
            /*System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();  //开始监视代码运行时间
           
            watch.Stop();  //停止监视
            TimeSpan timespan = watch.Elapsed;  //获取当前实例测量得出的总时间
             Console.WriteLine("打开窗口代码执行时间：{0}(毫秒)", timespan.TotalMilliseconds);  //总毫秒数
            Console.Read();*/
            //  StreamReader ss = new StreamReader("Define/s.txt");

            //   string s = ss.ReadToEnd();
            //   Console.WriteLine(s);

            Console.Read();
        }


    }
}
