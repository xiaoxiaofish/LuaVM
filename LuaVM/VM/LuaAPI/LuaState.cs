using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuaVM.Codegen;
using LuaVM.Paser;
using LuaVM.VM.Table;
namespace LuaVM.VM.LuaAPI
{
    public class LuaState
    {
        LuaStack stack;
        int pc;
        Prototype prototype;
        LuaOperator LuaOperator;
        public int Pc { get => pc; set => pc = value; }
        public LuaState(Prototype prototype)
        {
            stack = new LuaStack();
            stack.CreatStack(0);
            Pc = 0;
            this.prototype = prototype;
            LuaOperator = new LuaOperator();
        }

        public void AddPC()
        {
            pc++;
        }

        public void AddPC(int step)
        {
            pc += step;
        }

        /// <summary>
        /// 读取一条指令
        /// </summary>
        /// <returns></returns>
        public uint Fetch()
        {
            return prototype.Code[pc++];
        }

        /// <summary>
        /// 从常量表中读取指定常量推入栈顶
        /// </summary>
        /// <param name="index"></param>
        public void GetConstVar(int index)
        {
            stack.Push(prototype.ConstVars[index]);
        }

        /// <summary>
        /// 根据参数，当参数 > 0xff,将 rk & 0xff索引处常量推入栈顶，否则将栈rk处变量推入栈顶
        /// </summary>
        /// <returns></returns>
        public void GetRK(int rk)
        {
            if(rk > 0xff)
            {
                GetConstVar(rk & 0xff);
            }
            else
            {
                PushValueFromIndex(rk + 1);
            }
        }

        /// <summary>
        /// 获得栈顶索引
        /// </summary>
        /// <returns></returns>
        public int GetStackTop()
        {
            return stack.Top;
        }

        /// <summary>
        /// 获得栈顶元素
        /// </summary>
        /// <returns></returns>
        public LuaValue GetTopValue()
        {
            return stack.Get(stack.Top - 1);
        }

        public int AbsIndex(int index)
        {
            return stack.AbsIndex(index);
        }

        public LuaValue Get(int index)
        {
            return stack.Get(index);
        }

        public void Pop(int num)
        {
            while(num > 0)
            {
                stack.Pop();
                num--;
            }
        }

        public LuaValue Pop()
        {
            return stack.Pop();

        }

        public void CopyTo(int sourceIndex, int targetIndex)
        {
            LuaValue luaValue = stack.Get(sourceIndex);
            stack.Set(targetIndex,luaValue);
        }

        public void Push(LuaValue luaValue)
        {
            stack.Push(luaValue);
        }

        /// <summary>
        /// 将指定索引处的值推到栈顶
        /// </summary>
        /// <param name="index"></param>
        public void PushValueFromIndex(int index)
        {
            Push(Get(index));
        }

        /// <summary>
        /// 弹出栈顶元素，然后用指定值覆盖目标索引位置
        /// </summary>
        public void Replace(int index, LuaValue luaValue)
        {
            stack.Pop();
            stack.Set(index, luaValue);
        }

        /// <summary>
        /// 弹出栈顶元素，然后覆盖目标索引位置
        /// </summary>
        public void Replace(int index)
        {
            LuaValue luaValue =  stack.Pop();
            stack.Set(index, luaValue);
        }

        public void Remove(int index)
        {
            stack.Remove(index);
        }

        public void Insert(int index, LuaValue luaValue)
        {
            stack.Insert(index, luaValue);
        }

        /// <summary>
        /// 将源索引到栈顶区间的所有元素向栈顶方向旋转step个位置，若step < 0,则向栈底方向
        /// </summary>
        /// <param name="sourceIndex"></param>
        /// <param name="step"></param>
        public void Rotate(int sourceIndex, int step)
        {
            int t = stack.Top;
            int p = stack.AbsIndex(sourceIndex) - 1;
            int m = 0;
            if(step > 0)
            {
                m = t - step;
            }
            else
            {
                m = p - step - 1;
            }
            stack.Reverse(p, m);
            stack.Reverse(m + 1, t);
            stack.Reverse(p, t);
        }

        /// <summary>
        /// 设置栈底，若超出当前栈底，则填充nil，若为超出，则删除多余元素
        /// </summary>
        /// <param name="index"></param>
        public void SetTop(int index)
        {
            if(index < 0)
            {
                throw new Exception("栈索引错误！");
            }
            int num = stack.Top - index;
            if(num > 0)
            {
                while(num > 0)
                {
                    stack.Pop();
                    num--;
                }
            }
            else
            {
                while(num > 0)
                {
                    stack.Push(new LuaValue());
                    num--;
                }
            }
        }

        public LuaValueType TypeOfIndex(int index)
        {
            return stack.Get(index).Type;
        }

        public bool IsBool(int index)
        {
            return TypeOfIndex(index) == LuaValueType.Bool;
        }

        public bool IsNumber(int index)
        {
            return TypeOfIndex(index) == LuaValueType.Number;
        }

        public bool IsString(int index)
        {
            return TypeOfIndex(index) == LuaValueType.String;
        }

        public bool IsNil(int index)
        {
            return TypeOfIndex(index) == LuaValueType.Nil;
        }

        public bool IsNilOrNone(int index)
        {
            return TypeOfIndex(index) == LuaValueType.Nil || TypeOfIndex(index) == LuaValueType.None;
        }

        public bool IsNone(int index)
        {
            return TypeOfIndex(index) == LuaValueType.None;
        }

        public bool ToBool(int index)
        {
            switch(TypeOfIndex(index))
            {
                case LuaValueType.Nil:
                    return false;
                case LuaValueType.Bool:
                    return (bool)Get(index).OValue;
                default:
                    return true;
            }
        }

        public double ToNumber(int index)
        {
            try
            {
                return ToNumberX(index);
            }
            catch
            {
                return 0;
            }
        }

        public double ToNumberX(int index)
        {
            if(Get(index).OValue == null)
            {
                return Get(index).NValue;
            }
            throw new Exception("转换错误");
        }

        public string ToString(int index)
        {
            switch(TypeOfIndex(index))
            {
                case LuaValueType.String:
                    return (string)Get(index).OValue;
                case LuaValueType.Number:
                    return Get(index).NValue.ToString();
                default:
                    throw new Exception("该类型无法转换为string");
            }
        }

        public void MathOperation(TokenType opType)
        {
            LuaOperator.MathOperation(this, opType);
        }

        public void Concat(int number)
        {
            LuaOperator.Concat(this, number);
        }

        public void Len(int index)
        {
            LuaOperator.Len(this, index);
        }

        public LuaValue Compare(int index1, int index2, TokenType opType)
        {
            return LuaOperator.Compare(this, index1, index2, opType);
        }

        /// <summary>
        /// 创建一个预估计了大小的LuaTable推入栈顶
        /// </summary>
        public void CreatLuaTable(int nArry, int nRec)
        {
            stack.Push(new LuaValue(new LuaTable(nArry, nRec), LuaValueType.Table));
        }

        /// <summary>
        /// 创建一个空table推入栈顶
        /// </summary>
        public void NewTable()
        {
            CreatLuaTable(0, 0);
        }


    }
}
