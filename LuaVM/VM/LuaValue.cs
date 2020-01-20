using LuaVM.VM.LuaAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.VM
{

    public enum LuaValueType
    {
        Nil,
        Bool,
        Number,
        String,
        Thread,
        Table,
        Function,
        Userdata,
        None,
    }

    public class LuaValue
    {

        LuaValueType type;
        object oValue;
        double nValue;
        /// <summary>
        /// 默认构造函数，创建Nil变量
        /// </summary>
        public LuaValue()
        {
            type = LuaValueType.Nil;
        }

        /// <summary>
        /// 创建任意类型变量，且不指定值，一般用于创建None型变量
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="flags"></param>
        public LuaValue(LuaValueType flag,int flags)
        {
            type = flag;
        }

        /// <summary>
        /// 创建number型变量
        /// </summary>
        /// <param name="number"></param>
        public LuaValue(double number)
        {
            this.type = LuaValueType.Number;
            nValue = number;
        }

        /// <summary>
        /// 创建bool或引用型变量
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        public LuaValue(object obj,LuaValueType type)
        {
            this.type = type;
            oValue = obj;
        }

        public LuaValueType Type { get => type; }
        public object OValue { get => oValue; set => oValue = value; }
        public double NValue { get => nValue; set => nValue = value; }
        public bool ToDouble(ref double number)
        {
            try
            {
                number = double.Parse((string)oValue);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool ToString(ref string str)
        {
            if (this.type == LuaValueType.Number)
            {
                str = nValue.ToString();
                return true;
            }
            else if(type == LuaValueType.String)
            {
                str = oValue as string;
                return true;
            }
            return false;
        }


        public static LuaValue operator +(LuaValue value1, LuaValue value2)
        {
            switch (value1.type)
            {
                case LuaValueType.Number:
                    {
                        if (value2.type == LuaValueType.Number)
                        {
                            return new LuaValue(value1.nValue + value2.nValue);
                        }
                        else if (value2.type == LuaValueType.String)
                        {
                            double number = 0;
                            if (value2.ToDouble(ref number))
                            {
                                return new LuaValue(value1.nValue + number);
                            }
                        }
                        throw new Exception("这两个类型的变量无法进行计算");
                    }
                case LuaValueType.String:
                    {
                        double number1 = 0;
                        if (value1.ToDouble(ref number1))
                        {
                            switch (value2.type)
                            {
                                case LuaValueType.Number:
                                    return new LuaValue(number1 + value2.nValue);
                                case LuaValueType.String:
                                    {
                                        double number2 = 0;
                                        if (value2.ToDouble(ref number2))
                                        {
                                            return new LuaValue(number1 + number2);
                                        }
                                        throw new Exception("这两个类型的变量无法进行计算");
                                    }
                                default:
                                    throw new Exception("这两个类型的变量无法进行计算");
                            }
                        }
                        throw new Exception("这两个类型的变量无法进行计算");
                    }
                default:
                    throw new Exception("这两个类型的变量无法进行计算");
            }
        }

        public static LuaValue operator -(LuaValue value1, LuaValue value2)
        {
            switch (value1.type)
            {
                case LuaValueType.Number:
                    {
                        if (value2.type == LuaValueType.Number)
                        {
                            return new LuaValue(value1.nValue - value2.nValue);
                        }
                        else if (value2.type == LuaValueType.String)
                        {
                            double number = 0;
                            if (value2.ToDouble(ref number))
                            {
                                return new LuaValue(value1.nValue - number);
                            }
                        }
                        throw new Exception("这两个类型的变量无法进行计算");
                    }
                case LuaValueType.String:
                    {
                        double number1 = 0;
                        if (value1.ToDouble(ref number1))
                        {
                            switch (value2.type)
                            {
                                case LuaValueType.Number:
                                    return new LuaValue(number1 - value2.nValue);
                                case LuaValueType.String:
                                    {
                                        double number2 = 0;
                                        if (value2.ToDouble(ref number2))
                                        {
                                            return new LuaValue(number1 - number2);
                                        }
                                        throw new Exception("这两个类型的变量无法进行计算");
                                    }
                                default:
                                    throw new Exception("这两个类型的变量无法进行计算");
                            }
                        }
                        throw new Exception("这两个类型的变量无法进行计算");
                    }
                default:
                    throw new Exception("这两个类型的变量无法进行计算");
            }
        }

        public static LuaValue operator *(LuaValue value1, LuaValue value2)
        {
            switch (value1.type)
            {
                case LuaValueType.Number:
                    {
                        if (value2.type == LuaValueType.Number)
                        {
                            return new LuaValue(value1.nValue * value2.nValue);
                        }
                        else if (value2.type == LuaValueType.String)
                        {
                            double number = 0;
                            if (value2.ToDouble(ref number))
                            {
                                return new LuaValue(value1.nValue * number);
                            }
                        }
                        throw new Exception("这两个类型的变量无法进行计算");
                    }
                case LuaValueType.String:
                    {
                        double number1 = 0;
                        if (value1.ToDouble(ref number1))
                        {
                            switch (value2.type)
                            {
                                case LuaValueType.Number:
                                    return new LuaValue(number1 * value2.nValue);
                                case LuaValueType.String:
                                    {
                                        double number2 = 0;
                                        if (value2.ToDouble(ref number2))
                                        {
                                            return new LuaValue(number1 * number2);
                                        }
                                        throw new Exception("这两个类型的变量无法进行计算");
                                    }
                                default:
                                    throw new Exception("这两个类型的变量无法进行计算");
                            }
                        }
                        throw new Exception("这两个类型的变量无法进行计算");
                    }
                default:
                    throw new Exception("这两个类型的变量无法进行计算");
            }
        }

        public static LuaValue operator /(LuaValue value1, LuaValue value2)
        {
            switch (value1.type)
            {
                case LuaValueType.Number:
                    {
                        if (value2.type == LuaValueType.Number)
                        {
                            return new LuaValue(value1.nValue / value2.nValue);
                        }
                        else if (value2.type == LuaValueType.String)
                        {
                            double number = 0;
                            if (value2.ToDouble(ref number))
                            {
                                return new LuaValue(value1.nValue / number);
                            }
                        }
                        throw new Exception("这两个类型的变量无法进行计算");
                    }
                case LuaValueType.String:
                    {
                        double number1 = 0;
                        if (value1.ToDouble(ref number1))
                        {
                            switch (value2.type)
                            {
                                case LuaValueType.Number:
                                    return new LuaValue(number1 / value2.nValue);
                                case LuaValueType.String:
                                    {
                                        double number2 = 0;
                                        if (value2.ToDouble(ref number2))
                                        {
                                            return new LuaValue(number1 / number2);
                                        }
                                        throw new Exception("这两个类型的变量无法进行计算");
                                    }
                                default:
                                    throw new Exception("这两个类型的变量无法进行计算");
                            }
                        }
                        throw new Exception("这两个类型的变量无法进行计算");
                    }
                default:
                    throw new Exception("这两个类型的变量无法进行计算");
            }
        }

        public static LuaValue operator %(LuaValue value1, LuaValue value2)
        {
            switch (value1.type)
            {
                case LuaValueType.Number:
                    {
                        if (value2.type == LuaValueType.Number)
                        {
                            return new LuaValue(value1.nValue % value2.nValue);
                        }
                        else if (value2.type == LuaValueType.String)
                        {
                            double number = 0;
                            if (value2.ToDouble(ref number))
                            {
                                return new LuaValue(value1.nValue % number);
                            }
                        }
                        throw new Exception("这两个类型的变量无法进行计算");
                    }
                case LuaValueType.String:
                    {
                        double number1 = 0;
                        if (value1.ToDouble(ref number1))
                        {
                            switch (value2.type)
                            {
                                case LuaValueType.Number:
                                    return new LuaValue(number1 % value2.nValue);
                                case LuaValueType.String:
                                    {
                                        double number2 = 0;
                                        if (value2.ToDouble(ref number2))
                                        {
                                            return new LuaValue(number1 % number2);
                                        }
                                        throw new Exception("这两个类型的变量无法进行计算");
                                    }
                                default:
                                    throw new Exception("这两个类型的变量无法进行计算");
                            }
                        }
                        throw new Exception("这两个类型的变量无法进行计算");
                    }
                default:
                    throw new Exception("这两个类型的变量无法进行计算");
            }
        }

        public static LuaValue operator ==(LuaValue value1, LuaValue value2)
        {
            switch (value1.type)
            {
                case LuaValueType.Number:
                    {
                        if (value2.type == LuaValueType.Number)
                        {
                            return new LuaValue(value1.nValue == value2.nValue, LuaValueType.Bool);
                        }
                        throw new Exception("这两个类型的变量无法比较");
                    }
                default:
                    if (value2.type == value1.type)
                    {
                        return new LuaValue(value1.oValue.Equals(value2.oValue),LuaValueType.Bool);
                    }
                    throw new Exception("这两个类型的变量无法比较");
            }
        }

        public static LuaValue operator !=(LuaValue value1, LuaValue value2)
        {
            return new LuaValue(!(value1.nValue == value2.nValue), LuaValueType.Bool);
        }

        public static LuaValue operator >(LuaValue value1, LuaValue value2)
        {
            switch (value1.type)
            {
                case LuaValueType.Number:
                    {
                        if (value2.type == LuaValueType.Number)
                        {
                            return new LuaValue(value1.nValue > value2.nValue, LuaValueType.Bool);
                        }
                        throw new Exception("这两个类型的变量无法比较");
                    }
                case LuaValueType.String:
                    {
                        if (value2.type == LuaValueType.String)
                        {
                            return new LuaValue((value1.oValue as string).CompareTo(value2.oValue) == 1, LuaValueType.Bool);
                        }
                        throw new Exception("这两个类型的变量无法比较");
                    }
                default:
                    throw new Exception("这两个类型的变量无法比较");
            }
        }

        public static LuaValue operator <(LuaValue value1, LuaValue value2)
        {
            switch (value1.type)
            {
                case LuaValueType.Number:
                    {
                        if (value2.type == LuaValueType.Number)
                        {
                            return new LuaValue(value1.nValue < value2.nValue, LuaValueType.Bool);
                        }
                        throw new Exception("这两个类型的变量无法比较");
                    }
                case LuaValueType.String:
                    {
                        if (value2.type == LuaValueType.String)
                        {
                            return new LuaValue((value1.oValue as string).CompareTo(value2.oValue) == -1, LuaValueType.Bool);
                        }
                        throw new Exception("这两个类型的变量无法比较");
                    }
                default:
                    throw new Exception("这两个类型的变量无法比较");
            }
        }

        public static LuaValue operator >=(LuaValue value1, LuaValue value2)
        {
            switch (value1.type)
            {
                case LuaValueType.Number:
                    {
                        if (value2.type == LuaValueType.Number)
                        {
                            return new LuaValue(value1.nValue >= value2.nValue, LuaValueType.Bool);
                        }
                        return new LuaValue(false, LuaValueType.Bool);
                    }
                case LuaValueType.String:
                    {
                        if (value2.type == LuaValueType.String)
                        {
                            return new LuaValue((value1.oValue as string).CompareTo(value2.oValue) == 0 || (value1.oValue as string).CompareTo(value2.oValue) == 1, LuaValueType.Bool);
                        }
                        throw new Exception("这两个类型的变量无法比较");
                    }
                default:
                    throw new Exception("这两个类型的变量无法比较");
            }
        }

        public static LuaValue operator <=(LuaValue value1, LuaValue value2)
        {
            switch (value1.type)
            {
                case LuaValueType.Number:
                    {
                        if (value2.type == LuaValueType.Number)
                        {
                            return new LuaValue(value1.nValue <= value2.nValue, LuaValueType.Bool);
                        }
                        throw new Exception("这两个类型的变量无法比较");
                    }
                case LuaValueType.String:
                    {
                        if (value2.type == LuaValueType.String)
                        {
                            return new LuaValue((value1.oValue as string).CompareTo(value2.oValue) == 0 || (value1.oValue as string).CompareTo(value2.oValue) == -1, LuaValueType.Bool);
                        }
                        throw new Exception("这两个类型的变量无法比较");
                    }
                default:
                    throw new Exception("这两个类型的变量无法比较");
            }
        }

    }

}
