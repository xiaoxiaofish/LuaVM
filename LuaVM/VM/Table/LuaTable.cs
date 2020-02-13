using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.VM.Table
{
    public class LuaTable
    {
        private Dictionary<LuaValue, LuaValue> luaTable;
        /// <summary>
        /// 当表内数据为连续的正整数，不采用字典，直接使用数组
        /// </summary>
        private List<int> array;

        /// <summary>
        /// 构造函数，当nArr > 0时，表明该表可能当做数组来使用
        /// </summary>
        /// <param name="nArr"></param>
        /// <param name="nRec"></param>
        public LuaTable(int nArr, int nRec)
        {
            if (nArr > 0)
            {
                array = new List<int>(nArr);
            }
            if (nRec > 0)
            {
                luaTable = new Dictionary<LuaValue, LuaValue>();
            }
        }

        public LuaValue this[LuaValue key]
        {
            get
            {
                if(key.Type == LuaValueType.Nil)
                {
                    throw new Exception("table key is nil!");
                }
                if (luaTable != null)
                {
                    LuaValue value;
                    if (luaTable.TryGetValue(key, out value))
                    {
                        return value;
                    }
                    else
                    {
                        return new LuaValue();
                    }
                }
                else
                {
                    luaTable = new Dictionary<LuaValue, LuaValue>();
                    return new LuaValue();
                }
            }

            set
            {
                if(key.Type == LuaValueType.Nil)
                {
                    throw new Exception("table key is nil!");
                }
                if (luaTable != null)
                {
                    LuaValue _value;
                    if (luaTable.TryGetValue(key, out _value))
                    {
                        _value = value;
                    }
                    else
                    {
                        luaTable.Add(key, value);
                    }
                }
                else
                {
                    luaTable = new Dictionary<LuaValue, LuaValue>();
                    luaTable.Add(key, value);
                }
            }
        }

        public int Len()
        {
            if(luaTable != null)
            {
                return luaTable.Count;
            }
            return 0;
        }

        private LuaValue IsInt(LuaValue luaValue)
        {
            if(luaValue.Type == LuaValueType.Number)
            {
                if(luaValue.NValue - (int)luaValue.NValue == 0)
                {
                    return null;
                }
            }
            return null;
        }

    }
}
