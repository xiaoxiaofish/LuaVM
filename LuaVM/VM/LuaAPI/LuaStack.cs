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
        /// <summary>
        /// 初始化创建栈
        /// </summary>
        public  void CreatStack(int size)
        {
            stack = new List<LuaValue>();
        }

        public void Push(LuaValue luaValue)
        {
            stack.Add(luaValue);
        }

        public LuaValue Pop()
        {
            if(stack.Count > 0)
            {
                LuaValue luaValue = stack.Last();
                stack.RemoveAt(stack.Count - 1);
                return luaValue;
            }
            throw new Exception("栈为空!");

        }

        public LuaValue Get(int index)
        {
            try
            {
                return stack[index - 1];
            }
            catch(Exception e)
            {
                throw new Exception("无效栈索引！");
            }
        }

        public void Set(int index, LuaValue luaValue)
        {
            try
            {
                stack[index] = luaValue;
            }
            catch (Exception e)
            {
                throw new Exception("无效栈索引！");
            }
        }

        public bool IsValid(int index)
        {
            return index > 0 && index <= stack.Count;
        }

        public int AbsIndex(int index)
        {
            if(index > 0)
            {
                return index;
            }
            return index + stack.Count + 1;
        }

        public void Reverse(int startIndex, int num)
        {
            stack.Reverse(startIndex, num);
        }

        public void Remove(int index)
        {
            stack.RemoveAt(index);
        }

        public int Top
        {
            get
            {
                return stack.Count;
            }
        }

        public void Insert(int index,LuaValue luaValue)
        {
            stack.Insert(index, luaValue);
        }

    }
}
