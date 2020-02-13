using LuaVM.Codegen;
using LuaVM.Paser;
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
        /// 将源寄存器值赋值到目标寄存器，源寄存器由a指定，目标寄存器为b，c无效
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
            if(a != 0)
            {
                throw new Exception("指令错误！");
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
            for(int j = a + 1; j <= a + b; j++)
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
            luaState.Push(new LuaValue(b != 0,LuaValueType.Bool));
            luaState.Replace(a, luaState.GetTopValue());
            if(c != 0)
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
            luaState.GetConstVar(a);
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
            luaState.Replace(a);
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
            for(int j = b + 1; j <= c + 1; j++)
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
        public void Compare(Instruction i,TokenType opType)
        {
            int a = 0, b = 0, c = 0;
            i.ABC(ref a, ref b, ref c);
            luaState.GetRK(b + 1);
            luaState.GetRK(c + c);
            var result = luaState.Compare(-2, -1, opType);
            bool aBool = false;
            if(a == 1)
            {
                aBool = true;
            }
            if((bool)result.OValue == aBool)
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
        /// TestSet，判断由b操作数指定的寄存器值转换为bool类型变量后，是否与c操作数指定的bool值一样，若一样，跳过下一条指令。否则什么也不做
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
            if(result == cBool)
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
            if(isPositionStep && (bool)luaState.Compare(a,a+1,TokenType.SmallerEqual).OValue || !isPositionStep && (bool)luaState.Compare(a + 1, a, TokenType.SmallerEqual).OValue)
            {
                luaState.AddPC(sbx);
                luaState.CopyTo(a + 1, a + 4);
            }
        }

        public void LuaMain(Prototype prototype)
        {
            int regsQuantity = prototype.MaxStackSize;
            luaState = new LuaAPI.LuaState(prototype);
            //OpCode returnOp = Instruction.OpCodeTable[38];
            while (true)
            {
                Instruction pc = new Instruction(luaState.Fetch());
                if(pc.OpCode != 38)
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
