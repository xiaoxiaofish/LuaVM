using LuaVM.Paser;
using LuaVM.VM.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.VM.LuaAPI
{
    public class LuaOperator
    {
        private Dictionary<TokenType, Func<LuaValue, LuaValue, LuaValue>> operatorDic;
        public LuaOperator()
        {
            operatorDic = new Dictionary<TokenType, Func<LuaValue, LuaValue, LuaValue>>();
            operatorDic.Add(TokenType.Plus, (LuaValue value1, LuaValue value2) => { return value1 + value2; });
            operatorDic.Add(TokenType.Minus, (LuaValue value1, LuaValue value2) => { return value1 - value2; });
            operatorDic.Add(TokenType.Star, (LuaValue value1, LuaValue value2) => { return value1 * value2; });
            operatorDic.Add(TokenType.Slash, (LuaValue value1, LuaValue value2) => { return value1 / value2; });
            operatorDic.Add(TokenType.Equal, (LuaValue value1, LuaValue value2) => { return value1 == value2; });
            operatorDic.Add(TokenType.NotEqual, (LuaValue value1, LuaValue value2) => { return value1 != value2; });
            operatorDic.Add(TokenType.BiggerEqual, (LuaValue value1, LuaValue value2) => { return value1 >= value2; });
            operatorDic.Add(TokenType.SmallerEqual, (LuaValue value1, LuaValue value2) => { return value1 <= value2; });
            operatorDic.Add(TokenType.Smaller, (LuaValue value1, LuaValue value2) => { return value1 < value2; });
            operatorDic.Add(TokenType.Bigger, (LuaValue value1, LuaValue value2) => { return value1 > value2; });
        }
        public void MathOperation(LuaState luaState, TokenType opType)
        {
            LuaValue luaValue1 = luaState.Pop();
            LuaValue luaValue2 = luaState.Pop();
            luaState.Push(operatorDic[opType](luaValue1, luaValue2));
            try
            {
                luaState.Push(operatorDic[opType](luaValue1, luaValue2));
            }
            catch (Exception e)
            {
                LuaValue result = null;
                try
                {
                    if (luaState.CallMetaFunc(luaValue1, luaValue2, GetMetaFuncName(opType), out result))
                    {
                        luaState.Push(result);
                    }
                    else
                    {
                        throw new Exception("没有找到对应的元方法进行计算！");
                    }
                }
                catch (Exception ee)
                {
                    throw e;
                }
            }
        }

        public string GetMetaFuncName(TokenType op)
        {
            switch (op)
            {
                case TokenType.Plus:
                    return "__add";
                case TokenType.Minus:
                    return "__sub";
                case TokenType.Star:
                    return "__mul";
                case TokenType.Slash:
                    return "__div";
                case TokenType.Len:
                    return "__len";
                case TokenType.Equal:
                    return "__eq";
                case TokenType.Smaller:
                    return "__lt";
                case TokenType.SmallerEqual:
                    return "__le";
                default:
                    throw new Exception("");
            }
        }
        public LuaValue Compare(LuaState luaState, int index1, int index2, TokenType opType)
        {
            LuaValue luaValue1 = luaState.Get(index1);
            LuaValue luaValue2 = luaState.Get(index2);
            try
            {
                return operatorDic[opType](luaValue1, luaValue2);
            }
            catch (Exception e)
            {
                switch (opType)
                {
                    case TokenType.Equal:
                        {
                            if ((bool)(luaValue1 != luaValue2).OValue)
                            {
                                LuaValue result = null;
                                try
                                {
                                    if (luaState.CallMetaFunc(luaValue1, luaValue2, GetMetaFuncName(opType), out result))
                                    {
                                        return result;
                                    }
                                    else
                                    {
                                        throw new Exception("没有找到对应的元方法进行比较！");
                                    }
                                }
                                catch (Exception ee)
                                {
                                    throw e;
                                }
                            }
                            else
                            {
                                return luaValue1 == luaValue2;
                            }
                        }
                    default:
                        {
                            LuaValue result = null;
                            try
                            {
                                if (luaState.CallMetaFunc(luaValue1, luaValue2, GetMetaFuncName(opType), out result))
                                {
                                    return result;
                                }
                                else
                                {
                                    throw new Exception("没有找到对应的元方法进行比较！");
                                }
                            }
                            catch (Exception ee)
                            {
                                throw e;
                            }
                        }

                }
            }
        }

        public void Len(LuaState luaState, int index)
        {
            var value = luaState.Get(index);
            if (value.Type == LuaValueType.String)
            {
                luaState.Push(new LuaValue((double)((value.OValue as string).Length)));
            }
            else if (value.Type == LuaValueType.Table)
            {
                luaState.Push(new LuaValue((double)((value.OValue as Table.LuaTable).Len())));
            }
            else
            {
                LuaValue result = null;
                try
                {
                    if (luaState.CallMetaFunc(value, value, GetMetaFuncName(TokenType.Len), out result))
                    {
                        luaState.Push(result);
                    }
                }
                catch
                {
                    throw new Exception("无法计算该变量长度！");
                }
            }
        }

        public void Concat(LuaState luaState, int number)
        {
            if (number == 0)
            {
                luaState.Push(new LuaValue("", LuaValueType.String));
            }
            else if (number >= 2)
            {
                while (number > 1)
                {
                    string str1 = null;
                    string str2 = null;
                    if (luaState.Get(luaState.AbsIndex(-1)).ToString(ref str1) && luaState.Get(luaState.AbsIndex(-2)).ToString(ref str2))
                    {
                        luaState.Pop(2);
                        luaState.Push(new LuaValue(str1 + str2, LuaValueType.String));
                        --number;
                        continue;
                    }
                    else
                    {
                        LuaValue result = null;
                        try
                        {
                            var b = luaState.Pop();
                            var a = luaState.Pop();
                            if (luaState.CallMetaFunc(a, b, GetMetaFuncName(TokenType.Len), out result))
                            {
                                luaState.Push(result);
                            }
                        }
                        catch
                        {
                            throw new Exception("该类型的变量无法进行拼接");
                        }
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
