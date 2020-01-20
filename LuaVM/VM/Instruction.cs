using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.VM
{
    public enum OP
    {
        OP_MOVE,
        OP_LOADK,
        OP_LOADKX,
        OP_LOADBOOL,
        OP_LOADNIL,
        OP_GETUPVAL,
        OP_GETTABUP,
        OP_GETTABLE,
        OP_SETTABUP,
        OP_SETUPVAL,
        OP_SETTABLE,
        OP_NEWTABLE,
        OP_SELE,
        OP_ADD,
        OP_SUB,
        OP_MUL,
        OP_MOD,
        OP_POW,
        OP_DIV,
        OP_IDIV,
        OP_BAND,
        OP_BOR,
        OP_BXOR,
        OP_SHL,
        OP_SHR,
        OP_UNM,
        OP_BNOT,
        OP_NOT,
        OP_LEN,
        OP_CONCAT,
        OP_JMP,
        OP_EQ,
        OP_LT,
        OP_LE,
        OP_TEST,
        OP_TESTSET,
        OP_CALL,
        OP_TAILCALL,
        OP_RETURN,
        OP_FORLOOP,
        OP_FORPREP,
        OP_TFORCALL,
        OP_TFORLOOP,
        OP_SETLIST,
        OP_CLOSURE,
        OP_VARARG,
        OP_EXTRAARG,
    }
    public enum OPArgU
    {
        OPArgN,
        OPArgU,
        OPArgR,
        OPArgK,
    }
    public enum OPMode
    {
        IABC,
        IABX,
        IASBX,
        IAX,
    }
    public struct OpCode
    {
        byte testFlag;
        byte setAflag;
        OPArgU argBMode;
        OPArgU argCMode;
        OPMode opMode;
        OP name;

        public OpCode(byte testFlag, byte setAflag, OPArgU argBMode, OPArgU argCMode, OPMode opMode, OP name)
        {
            this.testFlag = testFlag;
            this.setAflag = setAflag;
            this.argBMode = argBMode;
            this.argCMode = argCMode;
            this.opMode = opMode;
            this.name = name;
        }

        public byte TestFlag { get => testFlag; }
        public byte SetAflag { get => setAflag; }
        public OPArgU ArgBMode { get => argBMode; }
        public OPArgU ArgCMode { get => argCMode; }
        public OPMode OpMode { get => opMode; }
        public OP Name { get => name; }
    }

    public class Instruction
    {
        private static OpCode[] opCodeTable = { new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgN, OPMode.IABC, OP.OP_MOVE),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgN, OPMode.IABX, OP.OP_LOADK),
                                                new OpCode(0, 1, OPArgU.OPArgN, OPArgU.OPArgN, OPMode.IABX, OP.OP_LOADKX),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgU, OPMode.IABC, OP.OP_LOADBOOL),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgN, OPMode.IABC, OP.OP_LOADNIL),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgN, OPMode.IABC, OP.OP_GETUPVAL),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgK, OPMode.IABC, OP.OP_GETTABUP),
                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgK, OPMode.IABC, OP.OP_GETTABLE),
                                                new OpCode(0, 0, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_SETTABUP),
                                                new OpCode(0, 0, OPArgU.OPArgU, OPArgU.OPArgN, OPMode.IABC, OP.OP_SETUPVAL),
                                                new OpCode(0, 0, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_SETTABLE),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgU, OPMode.IABC, OP.OP_NEWTABLE),

                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgK, OPMode.IABC, OP.OP_SELE),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_ADD),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_SUB),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_MUL),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_MOD),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_POW),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_DIV),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_IDIV),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_BAND),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_BOR),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_SHL),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_SHR),
                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgN, OPMode.IABC, OP.OP_UNM),
                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgN, OPMode.IABC, OP.OP_BNOT),
                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgN, OPMode.IABC, OP.OP_NOT),
                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgN, OPMode.IABC, OP.OP_LEN),

                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgR, OPMode.IABC, OP.OP_CONCAT),
                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgN, OPMode.IASBX, OP.OP_JMP),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_EQ),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_LT),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_LE),
                                                new OpCode(0, 1, OPArgU.OPArgN, OPArgU.OPArgU, OPMode.IABC, OP.OP_TEST),
                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgU, OPMode.IABC, OP.OP_TESTSET),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgU, OPMode.IABC, OP.OP_CALL),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgU, OPMode.IABC, OP.OP_TAILCALL),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgN, OPMode.IABC, OP.OP_RETURN),
                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgN, OPMode.IASBX, OP.OP_FORLOOP),

                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgN, OPMode.IASBX, OP.OP_FORPREP),
                                                new OpCode(0, 1, OPArgU.OPArgN, OPArgU.OPArgU, OPMode.IABC, OP.OP_TFORCALL),
                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgN, OPMode.IASBX, OP.OP_TFORLOOP),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgU, OPMode.IABC, OP.OP_SETLIST),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgN, OPMode.IABX, OP.OP_CLOSURE),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgN, OPMode.IABC, OP.OP_VARARG),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgU, OPMode.IAX, OP.OP_EXTRAARG)};

        readonly int MaxARGBX = 1 << 18 - 1;// 2^18 - 1 = 262143
        readonly int MaxARGSBX = (1 << 18 - 1) >> 1; // 262143 / 2 = 131071

        uint instruction;

        public int OpCode()
        {
            return (int)(instruction & 0x3f);
        }

        public void ABC(ref int a, ref int b, ref int c)
        {
            a = (int)(instruction >> 6 & 0xff);
            b = (int)(instruction >> 14 & 0x1ff);
            c = (int)(instruction >> 23 & 0x1ff);
        }

        public void ABX(ref int a, ref int bx)
        {
            a = (int)(instruction >> 6 & 0xff);
            bx = (int)(instruction >> 14 & 0x1ff);
        }
        public void ASBX(ref int a, ref int bx)
        {
            ABX(ref a, ref bx);
            bx -= MaxARGSBX;
        }

        public int AX()
        {
            return (int)(instruction >> 6);
        }

        public OP InstructionName
        {
            get
            {
                return opCodeTable[this.OpCode()].Name;
            }
        }

        public OPMode InstructionMode
        {
            get
            {
                return opCodeTable[this.OpCode()].OpMode;
            }
        }

        public OPArgU OperandBMode
        {
            get
            {
                return opCodeTable[this.OpCode()].ArgBMode;
            }
        }

        public OPArgU OperandCMode
        {
            get
            {
                return opCodeTable[this.OpCode()].ArgCMode;
            }
        }
    }
}
