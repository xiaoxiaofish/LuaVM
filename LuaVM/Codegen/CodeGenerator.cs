using LuaVM.Paser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuaVM.Codegen.Codegen;
using LuaVM.VM;
namespace LuaVM.Codegen
{
    public class CodeGenerator
    {
        SyntaxTreePaser paser;
        public CodeGenerator()
        {
            paser = new SyntaxTreePaser();
        }
        public Prototype GenPrototype(BlockNode block)
        {
            var mainFunc = new FuncdefExpNode();
            mainFunc.Block = block;
            mainFunc.IsVararg = false;
            mainFunc.ParList = new List<ConstExpNode>();
            FuncInfo funcInfo = new FuncInfo();
            funcInfo.UpVarDic.Add("_ENV", new UpValInfo(-1, 0, 0));
            paser.CGBlock(funcInfo, block);
            VM.LuaVM vm = new VM.LuaVM();
            //vm.e

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            var p = ToPrototype(funcInfo);
            watch.Start();  //开始监视代码运行时间
           // try
           // {
                vm.LuaMain(p);
          //  }
          //  catch(Exception e)
          //  {
//
          //  }
            watch.Stop();  //停止监视
            TimeSpan timespan = watch.Elapsed;  //获取当前实例测量得出的总时间
            Console.WriteLine("打开窗口代码执行时间：{0}(毫秒)", timespan.TotalMilliseconds);  //总毫秒数
            Console.WriteLine("生成完毕!");
            return null;
        }
        Prototype ToPrototype(FuncInfo funcInfo)
        {
            uint[] codes = new uint[funcInfo.InstructionList.Count];
            for (int i = 0; i < codes.Length; i++)
            {
                codes[i] = funcInfo.InstructionList[i].instruction;
            }
            return new Prototype("main", 0, 100, funcInfo.NParam, false, funcInfo.MaxReg, codes, GetUpVal(funcInfo), GetChildProtype(funcInfo), GetConstsVar(funcInfo));
        }

        Prototype.UpValue[] GetUpVal(FuncInfo funcInfo)
        {
            Prototype.UpValue[] upValues = new Prototype.UpValue[funcInfo.UpVarDic.Count];
            foreach (var upval in funcInfo.UpVarDic)
            {
                if (upval.Value.LocalVarRegIndex >= 0)
                {
                    upValues[upval.Value.Index] = new Prototype.UpValue(1, upval.Value.LocalVarRegIndex);
                }
                else
                {
                    upValues[upval.Value.Index] = new Prototype.UpValue(0, upval.Value.UpValIndex);
                }
            }
            return upValues;
        }

        Prototype[] GetChildProtype(FuncInfo funcInfo)
        {
            Prototype[] childs = new Prototype[funcInfo.ChildFunc.Count];
            int i = 0;
            foreach (var f in funcInfo.ChildFunc)
            {
                childs[i] = ToPrototype(f);
            }
            return childs;
        }

        LuaValue[] GetConstsVar(FuncInfo funcInfo)
        {
            LuaValue[] luaValues = new LuaValue[funcInfo.ConstDic.Count + 1];
            double i = 10.1;
            foreach (var value in funcInfo.ConstDic)
            {
                if (value.Key.GetType() == i.GetType())
                {
                    luaValues[value.Value] = new LuaValue((double)value.Key);
                }
                else
                {
                    luaValues[value.Value] = new LuaValue(value.Key as string, LuaValueType.String);
                }
            }
            return luaValues;
        }

        //public 
    }

}
