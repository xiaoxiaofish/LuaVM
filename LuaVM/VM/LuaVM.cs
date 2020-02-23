using LuaVM.Codegen;
using LuaVM.Paser;
using LuaVM.VM.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.VM
{
    public class LuaVM
    {
        private LuaAPI.LuaState luaState;

        /// <summary>
        /// 将源寄存器值赋值到目标寄存器，源寄存器由b指定，目标寄存器为a，c无效
        /// </summary>
        /// <param name="i"></param>
        public void Move(Instruction i)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            luaState.CopyTo(b + 1, a + 1);
        }

        /// <summary>
        /// 跳转到指定指令处，由bx指定
        /// </summary>
        /// <param name="i"></param>
        public void JMP(Instruction i)
        {
            int a = 0, bx = 0;
            i.ASBX(ref a, ref bx);
            luaState.AddPC(bx);
            if (a != 0)
            {

            }
        }

        /// <summary>
        /// 连续为寄存器置n个nil值，寄存器起始索引为a，数量为b,c无效
        /// </summary>
        /// <param name="i"></param>
        public void LoadNil(Instruction i)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            luaState.Push(new LuaValue());
            for (int j = a + 1; j <= a + b; j++)
            {
                luaState.CopyTo(-1, j);
            }
            luaState.Pop(1);
        }

        /// <summary>
        /// 设置给定寄存器值为bool, a 为寄存器索引，b为bool值，c非0则跳过下一条指令
        /// </summary>
        /// <param name="i"></param>
        public void LoadBool(Instruction i)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            a += 1;
            luaState.Push(new LuaValue(b != 0, LuaValueType.Bool));
            luaState.Replace(a, luaState.GetTopValue());
            if (c != 0)
            {
                luaState.AddPC();
            }
        }

        /// <summary>
        /// 将常量表的某个常量加载到指定寄存器，寄存器索引由a指定，常量表索引由b指定
        /// </summary>
        /// <param name="i"></param>
        public void LoadK(Instruction i)
        {
            int a = 0, bx = 0;
            i.ABX(ref a, ref bx);
            a += 1;
            luaState.GetConstVar(bx);
            luaState.Replace(a);
        }

        /// <summary>
        /// 使用iabx模式指令，Ax操作数使用26位bit编码，最大可以表示67108864索引号
        /// </summary>
        /// <param name="i"></param>
        public void LoadKX(Instruction i)
        {
            int a = 0, bx = 0;
            i.ABX(ref a, ref bx);
            a += 1;
            int ax = new Instruction(luaState.Fetch()).AX();
            luaState.GetConstVar(ax);
            luaState.Replace(a);
        }

        /// <summary>
        /// 二元操作符操作指令，b代表操作数1索引，c代表操作数2索引，a代表结果寄存器索引
        /// </summary>
        /// <param name="i"></param>
        /// <param name="opType"></param>
        public void DoubleOperator(Instruction i, TokenType opType)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            //将指定操作数传入栈顶
            luaState.GetRK(b);
            luaState.GetRK(c);
            //运算
            luaState.MathOperation(opType);
            //将结果传送到指定寄存器
            luaState.Replace(a + 1);
        }

        /// <summary>
        /// 求长度指令，b代表变量寄存器索引，a代表结果目标索引
        /// </summary>
        /// <param name="i"></param>
        public void Len(Instruction i)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            luaState.Len(b + 1);
            luaState.Replace(a + 1);
        }

        /// <summary>
        /// 连接指令，将连续n个寄存器的值连接在一起，放入a指定的寄存器，b和c代表起始索引和终结索引
        /// </summary>
        /// <param name="i"></param>
        public void Concat(Instruction i)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            int n = c - b + 1;
            for (int j = b + 1; j <= c + 1; j++)
            {
                luaState.PushValueFromIndex(j);
            }
            luaState.Concat(n);
            luaState.Replace(a + 1);
        }

        /// <summary>
        /// 比较指令，比较两个寄存器或常量表里的值，分别由b和c指定，如果比较结果和a操作数相同，则跳过下一条指令。比较指令不更改寄存器状态
        /// </summary>
        /// <param name="i"></param>
        public void Compare(Instruction i, TokenType opType)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            luaState.GetRK(b + 1);
            luaState.GetRK(c + c);
            var result = luaState.Compare(-2, -1, opType);
            bool aBool = false;
            if (a == 1)
            {
                aBool = true;
            }
            if ((bool)result.OValue == aBool)
            {
                luaState.AddPC();
            }
        }

        /// <summary>
        /// not指令，求某个变量是否为nil，a指定结果寄存器索引，b指定变量寄存器索引
        /// </summary>
        /// <param name="i"></param>
        public void Not(Instruction i)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            luaState.Push(new LuaValue(luaState.IsBool(b + 1), LuaValueType.Bool));
            luaState.Replace(a + 1);
        }

        /// <summary>
        /// Test，判断由b操作数指定的寄存器值转换为bool类型变量后，是否与c操作数指定的bool值一样，若一样，跳过下一条指令。否则什么也不做
        /// </summary>
        /// <param name="i"></param>
        public void Test(Instruction i)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            var result = luaState.IsBool(b + 1);
            bool cBool = false;
            if (c == 1)
            {
                cBool = true;
            }
            if (result == cBool)
            {
                luaState.AddPC();
            }
        }

        /// <summary>
        /// TestSet，判断由b操作数指定的寄存器值转换为bool类型变量后，是否与c操作数指定的bool值一样，若一样，将b寄存器的值赋值到a操作数指定的寄存器，否则跳过下一条指令
        /// </summary>
        /// <param name="i"></param>
        public void TestSet(Instruction i)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            var result = luaState.IsBool(b + 1);
            bool cBool = false;
            if (c == 1)
            {
                cBool = true;
            }
            if (result == cBool)
            {
                luaState.CopyTo(b + 1, a + 1);
            }
            else
            {
                luaState.AddPC();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ForPrep(Instruction i)
        {
            int a = 0, sbx = 0;
            i.ASBX(ref a, ref sbx);
            luaState.PushValueFromIndex(a + 3);
            luaState.PushValueFromIndex(a + 1);
            luaState.MathOperation(TokenType.Minus);
            luaState.Replace(a + 1);
            luaState.AddPC(sbx);
        }

        /// <summary>
        /// 数值for循环指令，
        /// </summary>
        /// <param name="i"></param>
        public void ForLoop(Instruction i)
        {
            int a = 0, sbx = 0;
            i.ASBX(ref a, ref sbx);
            luaState.PushValueFromIndex(a + 3);
            luaState.PushValueFromIndex(a + 1);
            luaState.MathOperation(TokenType.Plus);
            luaState.Replace(a + 1);
            bool isPositionStep = luaState.ToNumber(a + 2) >= 0;
            if (isPositionStep && (bool)luaState.Compare(a, a + 1, TokenType.SmallerEqual).OValue || !isPositionStep && (bool)luaState.Compare(a + 1, a, TokenType.SmallerEqual).OValue)
            {
                luaState.AddPC(sbx);
                luaState.CopyTo(a + 1, a + 4);
            }
        }

        /// <summary>
        /// NEWTABLE指令（iABC模式）创建空表，并将其放入指定寄存器。寄存器索引由操作数A指定，表的初始数组容量和哈希表容量分别由操作数B和C指定。
        /// </summary>
        /// <param name="i"></param>
        public void NewTable(Instruction i)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            a += 1;
            luaState.CreatLuaTable(b, c);
            luaState.Replace(a);
        }

        /// <summary>
        /// GETTABLE指令（iABC模式）根据键从表里取值，并放入目标寄存器中。
        /// 其中表位于寄存器中，索引由操作数B指定；键可能位于寄存器中，也可能在常量表里，索引由操作数C指定；目标寄存器索引则由操作数A指定。
        /// </summary>
        /// <param name="i"></param>
        public void GetTable(Instruction i)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            a += 1;
            b += 1;
            luaState.GetRK(c);
            var key = luaState.Pop();
            luaState.GetTable(key, b);
            luaState.Replace(a);
        }

        /// <summary>
        /// SETTABLE指令（iABC模式）根据键往表里赋值。其中表位于寄存器中，索引由操作数A指定；键和值可能位于寄存器中，也可能在常量表里，索引分别由操作数B和C指定。
        /// </summary>
        /// <param name="i"></param>
        public void SetTable(Instruction i)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            luaState.GetRK(b);
            luaState.GetRK(c);
            var value = luaState.Pop();
            var key = luaState.Pop();
            luaState.SetTable(a + 1, key, value);
        }

        /// <summary>
        /// SetList指令。
        /// </summary>
        /// <param name="i"></param>
        public void SetList(Instruction i)
        {

        }

        /// <summary>
        /// 将一个函数原型实例化为一个closure对象，然后放入指定寄存器内。由操作数A指定,函数原型由bx操作数指定
        /// </summary>
        /// <param name="i"></param>
        public void Closure(Instruction i)
        {
            int a = 0, bx = 0;
            i.ABX(ref a, ref bx);
            a += 1;
            luaState.LoadPrototype(bx);
            luaState.Replace(a);
        }

        /// <summary>
        /// Call指令，操作数A指定被调用函数对象，如果B=1，表示没有参数，如果B>1，表示有B-1个参数，这些参数从寄存器R(A+1)开始。
        /// 函数调用完之后，如果C=1，表示没有返回值，如果C>1，表示需要C-1个返回值，这些返回值会存到寄存器R(A)和它后面。
        /// B=0时，参数从R(A+1)一直到栈顶；C=0时，返回值从R(A)一直到栈顶。
        /// </summary>
        /// <param name="i"></param>
        public void Call(Instruction i)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            a += 1;
            int nArg = PushArgAndFunc(a, b);
            luaState.Call(nArg, c - 1);
            RunLuaClosure();
            var results = luaState.GetReturnValue(c - 1);
            luaState.PopLuaStack();
            if(results != null)
                luaState.PushN(results, results.Length);
        }

        private void RunLuaClosure()
        {
            while(true)
            {
                Instruction pc = new Instruction(luaState.Fetch());
                if (pc.OpCode != 38)
                {
                    pc.Execute(this);
                }
                else
                {
                    Console.WriteLine("代码执行完毕！");
                    return;
                }
            }
        }

        /// <summary>
        /// 将函数对象和参数压入栈顶,返回函数参数个数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private int PushArgAndFunc(int a, int b)
        {
            if (b >= 1)
            {
                for (int i = a; i < a + b; i++)
                {
                    luaState.PushValueFromIndex(i);
                }
                return b - 1;
            }
            // b = 0，代表参数从R(A+1)一直到栈顶
            else
            {
                int stackTop = luaState.GetStackTop();
                for (int i = a; i < a + stackTop; i++)
                {
                    luaState.PushValueFromIndex(i);
                }
                //参数就是从栈顶一直到R（A + 1）
                return luaState.GetStackTop() - luaState.RegsiterCount() - 1;
            }
        }

        private void PopResults(int a, int c)
        {
            //不需要返回值
            if (c == 1)
            {
                return;
            }
            //c > 1，表示需要 c - 1个返回值
            if (c > 1)
            {
                //返回值的寄存器从a开始。
                for (int i = a + c - 2; i >= a; i--)
                {
                    luaState.Replace(i);
                }
            }
            //c = 0,返回所有返回值,从栈顶一直到R（A）
            else
            {
                int stackTop = luaState.GetStackTop();
                for (int i = a + stackTop; i >= a; i--)
                {
                    luaState.Replace(i);
                }
            }
        }

        /// <summary>
        /// return指令，将连续多个寄存器的返回值返回给调用函数，第一个寄存器有操作数A指定，个数由操作数B指定，C操作数没用。
        /// 如果B=1，表示没有返回值，如果B>1，表示有B-1个返因值，这些返回值就存在寄存器R(A)和它后面。
        /// </summary>
        /// <param name="i"></param>
        public void Return(Instruction i)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            a += 1;
            if (b == 1)
            {
                return;
            }
            if (b > 1)
            {
                for (int j = 0; j < a + b - 2; j++)
                {
                    luaState.PushValueFromIndex(j);
                }
                return;
            }
            if (b == 0)
            {

            }
        }

        /// <summary>
        /// 变长参数指令。把传递给当前函数的连续多个参数加载到连续的指定寄存器中。A操作数指定第一个寄存器。B操作数指定数量。C操作数没用
        /// b == 1,表示没有，b > 1，表示 b-1个参数,b = 0表示把从栈顶到a的所有寄存器全部加载
        /// </summary>
        /// <param name="i"></param>
        public void Vararg(Instruction i)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            a += 1;
            //b > 1和b>0统一处理
            if (b != 1)
            {
                luaState.LoadVararg(b - 1);
                PopResults(a, b);
            }
        }

        /// <summary>
        /// 尾调用指令，使用当前函数帧继续调用函数
        /// </summary>
        /// <param name="i"></param>
        public void TailCall(Instruction i)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            a += 1;
            c = 0;
            int nArg = PushArgAndFunc(a, b);
            luaState.Call(nArg, c - 1);
            PopResults(a, c);
        }

        /// <summary>
        /// Self指令，把对象和函数对象拷贝到两个相邻的寄存器中。对象在寄存器中，由操作数B指定，方法名在常量表中，操作数C指定，目标寄存器由操作数A指定。这样比两次Move少一个指令
        /// </summary>
        /// <param name="i"></param>
        public void Self(Instruction i)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            a += 1;
            b += 1;
            luaState.CopyTo(b, a + 1);
            luaState.GetRK(c);
            luaState.GetTable(luaState.GetTopValue(), b);
            luaState.Replace(a);
        }

        /// <summary>
        /// GetUpval指令。A操作数指定目标寄存器，B操作数指定Upval寄存器，C没用
        /// </summary>
        /// <param name="i"></param>
        public void GetUpval(Instruction i)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            luaState.Push(luaState.GetUpval(b));
            luaState.CopyTo(luaState.GetStackTop(), a + 1);
            luaState.Pop();
        }

        /// <summary>
        /// setupval指令，将寄存器的值赋值给upval，a指定寄存器索引，b指定upval索引，c没用
        /// </summary>
        /// <param name="i"></param>
        public void SetUpval(Instruction i)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            luaState.SetUpval(b, luaState.Get(a + 1));
        }

        /// <summary>
        /// gettableup指令，当某个upval是一个表，则通过该指令从该表中获取值，目标寄存器由a指定，upval索引由b指定，键由c指定，可能是常量也可能在寄存器中
        /// </summary>
        /// <param name="i"></param>
        public void GetUpTable(Instruction i)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            luaState.GetRK(c);
            var table = luaState.GetUpval(b);
            luaState.Push((table.OValue as LuaTable)[luaState.Pop()]);
            luaState.Replace(a + 1);
        }

        /// <summary>
        /// settableup指令，当某个upval是一个表，则通过该指令给该表赋值，upval索引由a指定，值由c指定，键由b指定，可能是常量也可能在寄存器中
        /// </summary>
        /// <param name="i"></param>
        public void SetUpTable(Instruction i)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            var table = luaState.GetUpval(a);
            luaState.GetRK(b);
            luaState.GetRK(c);
            var key = luaState.Pop();
            var value = luaState.Pop();
            (table.OValue as LuaTable)[key] = value;
        }

        public void LuaMain(Prototype prototype)
        {
            int regsQuantity = prototype.MaxStackSize;
            luaState = new LuaAPI.LuaState(prototype);
            luaState.RunLuaClosureAction = RunLuaClosure;
            //OpCode returnOp = Instruction.OpCodeTable[38];
            while (true)
            {
                Instruction pc = new Instruction(luaState.Fetch());
                if (pc.OpCode != 38)
                {
                    pc.Execute(this);
                }
                else
                {
                    return;
                }
            }
        }
    }
}
