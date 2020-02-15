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
        const int minStackSize = 20;
        const int maxStackSize = 10000000;
        const int registtyIndex = -maxStackSize - 1000;
        const int GRegistryIndex = 2;
        LuaValue GRegistryKey = new LuaValue(GRegistryIndex);
        LuaTable registerTable;
        LuaStack stack;
        LuaOperator LuaOperator;
        Dictionary<int, UpValue> openUpValDic;
        public int Pc { get => stack.Pc; set => stack.Pc = value; }
        public LuaState(Prototype prototype)
        {
            stack = new LuaStack(minStackSize);
            //stack.CreatStack(minStackSize);
            stack.Closure = new Closure(prototype);
            Pc = 0;
            //this.prototype = prototype;
            LuaOperator = new LuaOperator();
            registerTable = new LuaTable(0, 0);
            //openUpValDic = new Dictionary<int, UpValue>();
        }

        private void InitState()
        {
            registerTable[GRegistryKey] = new LuaValue(new LuaTable(0, 0), LuaValueType.Table);
        }

        public void AddPC()
        {
            stack.Pc++;
        }

        public void AddPC(int step)
        {
            stack.Pc += step;
        }

        /// <summary>
        /// 读取一条指令
        /// </summary>
        /// <returns></returns>
        public uint Fetch()
        {
            return stack.Closure.Prototype.Code[stack.Pc++];
        }

        /// <summary>
        /// 从常量表中读取指定常量推入栈顶
        /// </summary>
        /// <param name="index"></param>
        public void GetConstVar(int index)
        {
            stack.Push(stack.Closure.Prototype.ConstVars[index]);
        }

        /// <summary>
        /// 根据参数，当参数 > 0xff,将 rk & 0xff索引处常量推入栈顶，否则将栈rk处变量推入栈顶
        /// </summary>
        /// <returns></returns>
        public void GetRK(int rk)
        {
            if (rk > 0xff)
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
            while (num > 0)
            {
                stack.Pop();
                num--;
            }
        }

        public LuaValue Pop()
        {
            return stack.Pop();
        }

        public LuaValue[] PopN(int num)
        {
            LuaValue[] values = new LuaValue[num];
            int index = num - 1;
            for (int i = index; i > 0; i++)
            {
                values[index] = Pop();
            }
            return values;
        }

        public void PushN(LuaValue[] values, int n)
        {
            if (n < 0)
            {
                n = values.Length;
            }
            int len = values.Length;
            for (int i = 0; i < n; i++)
            {
                if (i < len)
                {
                    Push(values[i]);
                }
                else
                {
                    Push(new LuaValue());
                }
            }
        }

        public void CopyTo(int sourceIndex, int targetIndex)
        {
            LuaValue luaValue = stack.Get(sourceIndex);
            stack.Set(targetIndex, luaValue);
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
            LuaValue luaValue = stack.Pop();
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
            if (step > 0)
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
            if (index < 0)
            {
                throw new Exception("栈索引错误！");
            }
            int num = stack.Top - index;
            if (num > 0)
            {
                while (num > 0)
                {
                    stack.Pop();
                    num--;
                }
            }
            else
            {
                while (num > 0)
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
            switch (TypeOfIndex(index))
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
            if (Get(index).OValue == null)
            {
                return Get(index).NValue;
            }
            throw new Exception("转换错误");
        }

        public string ToString(int index)
        {
            switch (TypeOfIndex(index))
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

        /// <summary>
        /// 从指定寄存器器里取出表，将表中key对应值推到栈顶，并返回值的类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public LuaValueType GetTable(LuaValue key, int index)
        {
            var table = Get(index);
            if (table.Type == LuaValueType.Table)
            {
                stack.Push((table.OValue as LuaTable)[key]);
                return (table.OValue as LuaTable)[key].Type;
            }
            else
            {
                throw new Exception("this is not table");
            }
        }

        /// <summary>
        /// 将键值对写入指定寄存器的表内
        /// </summary>
        /// <param name="index"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetTable(int index, LuaValue key, LuaValue value)
        {
            var table = Get(index);
            if (table.Type == LuaValueType.Table)
            {
                (table.OValue as LuaTable)[key] = value;
            }
            else
            {
                throw new Exception("this is not table");
            }
        }

        public void PushGTable()
        {
            Push(registerTable[GRegistryKey]);
        }

        public LuaValueType GetGTable(string key)
        {
            var val = (registerTable[GRegistryKey].OValue as LuaTable)[new LuaValue(key, LuaValueType.String)];
            Push(val);
            return val.Type;
        }

        public void SetGTable(LuaValue value, string key)
        {
            var t = (registerTable[GRegistryKey].OValue as LuaTable);
            t[new LuaValue(key, LuaValueType.String)] = value;
        }

        public void RegiterCSharpFunc(Closure CSharpFunc, string key)
        {
            SetGTable(new LuaValue(CSharpFunc, LuaValueType.CSharpFunc), key);
        }


        /// <summary>
        /// 用单向链表做函数调用栈。添加一个新的函数调用栈。
        /// </summary>
        /// <param name="stack"></param>
        public void PushLuaStack(LuaStack stack)
        {
            stack.Prep = this.stack;
            this.stack = stack;
        }

        /// <summary>
        /// 弹出最底层的调用栈
        /// </summary>
        public void PopLuaStack()
        {
            this.stack = this.stack.Prep;
        }

        /// <summary>
        /// 加载二进制代码文件，并初始化main函数，构造为函数对象压入栈顶。
        /// </summary>
        /// <param name="chunck"></param>
        /// <returns></returns>
        public int Load(byte[] chunck)
        {
            BinaryChunk binaryChunk = new BinaryChunk(chunck);
            Prototype prototype = binaryChunk.Undump();
            Closure c = new Closure(prototype);
            if (prototype.UpValues.Length > 0)
            {
                var env = registerTable[GRegistryKey];
                c.UpValues[0] = new UpValue(env);
            }
            Push(new LuaValue(c, LuaValueType.Function));
            return 0;

        }

        /// <summary>
        /// 调用函数。nArg为参数个数，nResult为返回值个数
        /// </summary>
        /// <param name="nArg">参数个数</param>
        /// <param name="nResult">返回值个数</param>
        public void Call(int nArg, int nResult)
        {
            var c = Get(-(nArg + 1));
            if (c.Type == LuaValueType.Function)
            {
                CallLuaClosure(nArg, nResult, c.OValue as Closure);
            }
            else if (c.Type == LuaValueType.CSharpFunc)
            {
                CallCSharpClosure(nArg, nResult, c.OValue as Closure);
            }
            else
            {
                throw new Exception("this is not a function");
            }
        }

        /// <summary>
        /// 调用Lua函数
        /// </summary>
        /// <param name="nArg">参数数量</param>
        /// <param name="nResult">返回值数量</param>
        /// <param name="c"></param>
        private void CallLuaClosure(int nArg, int nResult, Closure c)
        {
            int nReg = c.Prototype.MaxStackSize;
            int nParams = c.Prototype.ParamsNum;
            bool isVararg = c.Prototype.IsVararg;
            LuaStack newStack = new LuaStack(nReg + 20);
            newStack.Closure = c;
            var args = PopN(nArg);
            //把函数对象POP掉
            Pop();
            newStack.PushN(args, nParams);
            //让指针指向预分配的栈顶
            if (newStack.Top < nReg)
            {
                int i = nReg - newStack.Top;
                while (i > 0)
                {
                    newStack.Push(new LuaValue());
                }
            }
            //如果函数时可变参数函数且参数个数比固定参数多，则把参数保存在栈的varlist里
            if (nArg > nParams && isVararg)
            {
                newStack.VarList = args;
            }
            PushLuaStack(newStack);
        }

        private void CallCSharpClosure(int nArg, int nResult, Closure c)
        {
            LuaStack newStack = new LuaStack(nArg + 20);
            newStack.Closure = c;
            var args = PopN(nArg);
            newStack.PushN(args, nArg);
            int nInfactResult = c.CSharpFunc(this);
            PopLuaStack();
            if (nInfactResult != 0)
            {
                var results = newStack.PopN(nInfactResult);
                stack.PushN(results, nResult);
            }
        }

        public LuaValue[] GetReturnValue(int nResult)
        {
            if (nResult != 0)
            {
                return stack.PopN(nResult);
            }
            else
                return null;
        }

        /// <summary>
        /// 当前函数已经使用了的寄存器数量
        /// </summary>
        /// <returns></returns>
        public int RegsiterCount()
        {
            return stack.Closure.Prototype.MaxStackSize;
        }

        /// <summary>
        /// 将变长参数推入栈顶
        /// </summary>
        /// <param name="num"></param>
        public void LoadVararg(int num)
        {
            if (num < 0)
            {
                num = stack.VarList.Length;
            }
            PushN(stack.VarList, num);
        }

        /// <summary>
        /// 把当前lua函数的指定子函数实例化为closure类型，并且加载闭包推入栈顶
        /// </summary>
        /// <param name="index"></param>
        public void LoadPrototype(int index)
        {
            var proto = stack.Closure.Prototype.ChildProtos[index];
            Closure c = new Closure(proto);
            int len = proto.UpValues.Length;
            for (int i = 0; i < len; i++)
            {
                int upValIndex = proto.UpValues[i].Index;
                //该变量是当前栈的，直接捕获。
                if (proto.UpValues[i].Instack == 1)
                {
                    if (openUpValDic == null)
                    {
                        openUpValDic = new Dictionary<int, UpValue>();
                    }
                    UpValue upValue = null;
                    //该upval处于open状态，直接捕获
                    if (openUpValDic.TryGetValue(upValIndex, out upValue))
                    {
                        c.UpValues[i] = upValue;
                    }
                    //否则先保存起来
                    else
                    {
                        c.UpValues[i] = new UpValue(Get(upValIndex));
                        openUpValDic.Add(upValIndex, c.UpValues[i]);
                    }
                }
                //如果upval属于再外层的变量，直接从当前的闭包中捕获
                else
                {
                    c.UpValues[i] = stack.Closure.UpValues[upValIndex];
                }
            }
            Push(new LuaValue(c, LuaValueType.Function));
        }

        /// <summary>
        /// 将C#函数压入栈
        /// </summary>
        /// <param name="func"></param>
        public void PushCSharpFunc(Func<LuaState, int> func)
        {
            Closure c = new Closure(func, 0);
            Push(new LuaValue(c, LuaValueType.CSharpFunc));
        }

        /// <summary>
        /// 将C#闭包压入栈
        /// </summary>
        /// <param name="func"></param>
        /// <param name="nUpval"></param>
        public void PushCSharpClosure(Func<LuaState, int> func, int nUpval)
        {
            Closure c = new Closure(func, nUpval);
            for (int i = nUpval; i > 0; i++)
            {
                c.UpValues[i - 1] = new UpValue(Pop());
            }
            Push(new LuaValue(c, LuaValueType.CSharpFunc));
        }

        public bool IsCSharpFunc(int index)
        {
            return Get(index).Type == LuaValueType.CSharpFunc;
        }

        public Func<LuaState, int> ToCSharpFunc(int index)
        {
            var func = Get(index);
            if (func.Type == LuaValueType.CSharpFunc)
            {
                return func.OValue as Func<LuaState, int>;
            }
            return null;
        }

    }
}
