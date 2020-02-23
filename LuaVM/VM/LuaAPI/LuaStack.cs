using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.VM.LuaAPI
{
    public class LuaStack
    {
        List<LuaValue> stack;
        LuaState luaState;
        LuaStack prep;
        Closure closure;
        LuaValue[] varList;
        int pc;
        int top;
        /// <summary>
        /// 初始化创建栈
        /// </summary>
        public void CreatStack(int size, LuaState luaState)
        {
            stack = new List<LuaValue>(size);
            /*for(int i = 0; i < size; i++)
            {
                stack.Add(null);
            }*/
            this.luaState = luaState;
        }

        public LuaStack(int size)
        {
            stack = new List<LuaValue>(size);
            /* for (int i = 0; i < size; i++)
             {
                 stack.Add(null);
             }
             top = 1;*/
            stack.Add(null);
            VarList = new LuaValue[1];
        }

        public void Push(LuaValue luaValue)
        {
            stack.Add(luaValue);
        }

        public LuaValue Pop()
        {
            if (stack.Count > 0)
            {
                LuaValue luaValue = stack.Last();
                stack.RemoveAt(stack.Count - 1);
                return luaValue;
            }
            throw new Exception("栈为空!");

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

        public LuaValue Get(int index)
        {
            try
            {
                if (index < 0)
                    index = AbsIndex(index);
                return stack[index];
            }
            catch (Exception e)
            {
                throw new Exception("无效栈索引！");
            }
        }

        public void Set(int index, LuaValue luaValue)
        {
            try
            {
                if (index < 0)
                    index = AbsIndex(index);
                stack[index] = luaValue;
            }
            catch (Exception e)
            {
                if(index < stack.Capacity)
                {
                    for(int i = stack.Count; i <= index; i++)
                    {
                        stack.Add(null);
                    }
                    stack[index] = luaValue;
                    return;
                }
                throw new Exception("无效栈索引！");
            }
        }

        public bool IsValid(int index)
        {
            return index > 0 && index <= stack.Count;
        }

        public int AbsIndex(int index)
        {
            if (index > 0)
            {
                return index;
            }
            return index + stack.Count;
        }

        public void Reverse(int startIndex, int num)
        {
            if (startIndex < 0)
                startIndex = AbsIndex(startIndex);
            stack.Reverse(startIndex, num);
        }

        public void Remove(int index)
        {
            if (index < 0)
                index = AbsIndex(index);
            stack.RemoveAt(index);
        }

        public int Top
        {
            get
            {
                return stack.Count;
            }
        }

        public LuaStack Prep { get => prep; set => prep = value; }
        public Closure Closure { get => closure; set => closure = value; }
        public LuaValue[] VarList { get => varList; set => varList = value; }
        public int Pc { get => pc; set => pc = value; }

        public void Insert(int index, LuaValue luaValue)
        {
            if (index < 0)
                index = AbsIndex(index);
            stack.Insert(index, luaValue);
        }

    }
}
