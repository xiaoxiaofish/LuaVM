using LuaVM.Paser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.VM.LuaAPI
{
    public class LuaOperator
    {
        private Dictionary<TokenType, Func<LuaValue, LuaValue,LuaValue>> operatorDic;
        public LuaOperator()
        {
            operatorDic = new Dictionary<TokenType, Func<LuaValue, LuaValue, LuaValue>>();
            operatorDic.Add(TokenType.Plus, (LuaValue value1, LuaValue value2) => { return value1 + value2; });
            operatorDic.Add(TokenType.Minus, (LuaValue value1, LuaValue value2) => { return value1 - value2; });
            operatorDic.Add(TokenType.Star, (LuaValue value1, LuaValue value2) => { return value1 * value2; });
            operatorDic.Add(TokenType.Slash, (LuaValue value1, LuaValue value2) => { return value1 / value2; });
            operatorDic.Add(TokenType.Equal, (LuaValue value1, LuaValue value2) => { return value1 == value2; });
            operatorDic.Add(TokenType.NotEqual, (LuaValue value1, LuaValue value2) => { return value1 != value2;});
            operatorDic.Add(TokenType.BiggerEqual, (LuaValue value1, LuaValue value2) => { return value1 >= value2; });
            operatorDic.Add(TokenType.SmallerEqual, (LuaValue value1, LuaValue value2) => { return value1 <= value2; });
            operatorDic.Add(TokenType.Smaller, (LuaValue value1, LuaValue value2) => { return value1 < value2; });
            operatorDic.Add(TokenType.Bigger, (LuaValue value1, LuaValue value2) => { return value1 > value2; });
        }
        public void MathOperation(LuaState luaState,TokenType opType)
        {
            LuaValue luaValue1 = luaState.Pop();
            LuaValue luaValue2 = luaState.Pop();
            LuaValue result = operatorDic[opType](luaValue1, luaValue2);
            luaState.Push(result);
        }
        public bool Compare(LuaState luaState,int index1, int index2, TokenType opType)
        {
            LuaValue luaValue1 = luaState.Get(index1);
            LuaValue luaValue2 = luaState.Get(index2);
            return (bool)operatorDic[opType](luaValue1, luaValue2).OValue;
        }

        public void Len(LuaState luaState,int index)
        {
            var value = luaState.Get(index);
            if(value.Type == LuaValueType.String)
            {
                luaState.Push(new LuaValue((double)((value.OValue as string).Length)));
            }
            else
            {
                throw new Exception("无法计算该变量长度！");
            }
        }

        public void Concat(LuaState luaState, int number)
        {
            if(number == 0)
            {
                luaState.Push(new LuaValue("", LuaValueType.String));
            }
            else if(number >= 2)
            {
                while(number > 1)
                {
                    string str1 = null;
                    string str2 = null;
                    if(luaState.Get(luaState.AbsIndex(-1)).ToString(ref str1) && luaState.Get(luaState.AbsIndex(-2)).ToString(ref str2))
                    {
                        luaState.Pop(2);
                        luaState.Push(new LuaValue(str1 + str2, LuaValueType.String));
                        --number;
                        continue;
                    }
                }
            }
            else
            {
                throw new Exception("该类型的变量无法进行拼接");
            }
        }

    }
}
