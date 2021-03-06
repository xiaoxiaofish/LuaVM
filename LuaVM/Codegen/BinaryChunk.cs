﻿using LuaVM.VM;
using LuaVM.VM.LuaAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.Codegen
{
    public class Prototype
    {
        public struct UpValue
        {
            int instack;
            int index;

            public UpValue(int instack, int index)
            {
                this.instack = instack;
                this.index = index;
            }

            public int Instack { get => instack;}
            public int Index { get => index;}
        }
        string source;
        int lineDefine;
        int lastLineDefine;
        int paramsNum;
        bool isVararg;
        int maxStackSize;
        uint[] code;
        UpValue[] upValues;
        LuaValue[] constVars;
        Prototype[] childProtos;
        public Prototype(string source, int lineDefine, int lastLineDefine, int paramsNum, bool isVararg, int maxStackSize, uint[] code, UpValue[] upValues, Prototype[] childProtos,LuaValue[] constVar)
        {
            this.source = source;
            this.lineDefine = lineDefine;
            this.lastLineDefine = lastLineDefine;
            this.paramsNum = paramsNum;
            this.isVararg = isVararg;
            this.maxStackSize = maxStackSize;
            this.code = code;
            this.upValues = upValues;
            this.childProtos = childProtos;
            constVars = constVar;
        }
        public string Source { get => source; }
        public int LineDefine { get => lineDefine; }
        public int LastLineDefine { get => lastLineDefine; }
        public int ParamsNum { get => paramsNum; }
        public bool IsVararg { get => isVararg; }
        /// <summary>
        /// 最少需要的寄存器数量
        /// </summary>
        public int MaxStackSize { get => maxStackSize; }
        public uint[] Code { get => code; }
        public UpValue[] UpValues { get => upValues; }
        public LuaValue[] ConstVars { get => constVars; }
        public Prototype[] ChildProtos { get => childProtos;}
    }
    public class BinaryChunk
    {
        class BinaryReader
        {
            byte[] datas;
            int index;

            public BinaryReader(byte[] datas)
            {
                this.datas = datas;
                index = 0;
            }

            public byte ReadByte()
            {
                return datas[index++];
            }

            public byte[] ReadBytes(int number)
            {
                byte[] bytes = new byte[number];
                for (int i = 0; i < number; i++)
                {
                    bytes[i] = datas[i + index];
                }
                index += number;
                return bytes;
            }

            public int ReadInt()
            {
                int i = BitConverter.ToInt32(datas, index);
                index += 4;
                return i;
            }

            public uint ReadUint()
            {
                uint i = BitConverter.ToUInt32(datas, index);
                index += 4;
                return i;
            }

            public double ReadDouble()
            {
                double d = BitConverter.ToDouble(datas, index);
                index += 8;
                return d;
            }

            public string ReadString()
            {
                var size = ReadByte();
                if (size == 0)
                {
                    return "";
                }
                if (size == 0xff)
                {
                    int stringSize = ReadInt();
                    return BitConverter.ToString(ReadBytes(stringSize));
                }
                throw new Exception("错误的代码指令！无法执行！");
            }

            public uint[] ReadCode()
            {
                int size = ReadInt();
                uint[] codes = new uint[size];
                for (int i = 0; i < size; i++)
                {
                    codes[i] = ReadUint();
                }
                return codes;
            }

            public Prototype.UpValue[] ReadUpValues()
            {
                int size = ReadInt();
                Prototype.UpValue[] upValues = new Prototype.UpValue[size];
                for (int i = 0; i < size; i ++)
                {
                    upValues[i] = new Prototype.UpValue(ReadInt(), ReadInt());
                }
                return upValues;
            }

            /// <summary>
            /// 读取函数原型
            /// </summary>
            /// <param name="parentSource"></param>
            /// <returns></returns>
            public Prototype ReadPrototype(string parentSource)
            {
                string source = ReadString();
                if (source.Equals(""))
                {
                    source = parentSource;
                }
                return new Prototype(source, ReadInt(), ReadInt(), ReadInt(), BitConverter.ToBoolean(datas, index++), ReadInt(), ReadCode(), ReadUpValues(),ReadPrototypes(parentSource),null);
            }

            public LuaValue ReadConstLuaValue()
            {
                switch(ReadByte())
                {
                    //读取到bool类型变量
                    case 0:
                        return new LuaValue(BitConverter.ToBoolean(datas, index++),LuaValueType.Bool);
                    //读取到nil变量
                    case 1:
                        return new LuaValue();
                    //读取到number变量
                    case 2:
                        return new LuaValue(ReadDouble());
                    //读取到string
                    case 3:
                        return new LuaValue(ReadString(), LuaValueType.String);
                    default:
                        throw new Exception("读取常量失败，执行代码错误！");
                }
            }

            public LuaValue[] ReadConstLuaVars()
            {
                int size = ReadInt();
                LuaValue[] vars = new LuaValue[size];
                for (int i = 0; i < size; i ++)
                {
                    vars[i] = ReadConstLuaValue();
                }
                return vars;
            }


            /// <summary>
            /// 读取所有子函数原型
            /// </summary>
            public Prototype[] ReadPrototypes(string parentSource)
            {
                int size = ReadInt();
                Prototype[] prototypes = new Prototype[size];
                for(int i = 0; i < size; i++)
                {
                    prototypes[i] = ReadPrototype(parentSource);
                }
                return prototypes;
            }

        }

        BinaryReader reader;
        public BinaryChunk(byte[] chunck)
        {
            reader = new BinaryReader(chunck);
        }

        public Prototype Undump()
        {
            if(CheckHeader())
            {
                return reader.ReadPrototype("");
            }
            throw new Exception("无法识别的Lua二进制文件！");
        }

        private bool CheckHeader()
        {
            return true;
        }

    }
}
