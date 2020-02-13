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
        Action<Instruction, LuaVM> action;

        public OpCode(byte testFlag, byte setAflag, OPArgU argBMode, OPArgU argCMode, OPMode opMode, OP name, Action<Instruction, LuaVM> action)
        {
            this.testFlag = testFlag;
            this.setAflag = setAflag;
            this.argBMode = argBMode;
            this.argCMode = argCMode;
            this.opMode = opMode;
            this.name = name;
            this.action = action;
        }

        public byte TestFlag { get => testFlag; }
        public byte SetAflag { get => setAflag; }
        public OPArgU ArgBMode { get => argBMode; }
        public OPArgU ArgCMode { get => argCMode; }
        public OPMode OpMode { get => opMode; }
        public OP Name { get => name; }
        public Action<Instruction, LuaVM> Action { get => action;}
    }

    public struct Instruction
    {
        private static OpCode[] opCodeTable = { new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgN, OPMode.IABC, OP.OP_MOVE,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgN, OPMode.IABX, OP.OP_LOADK,(Instruction i, LuaVM vm) =>{ vm.LoadK(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgN, OPArgU.OPArgN, OPMode.IABX, OP.OP_LOADKX,(Instruction i, LuaVM vm) =>{ vm.LoadKX(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgU, OPMode.IABC, OP.OP_LOADBOOL,(Instruction i, LuaVM vm) =>{ vm.LoadBool(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgN, OPMode.IABC, OP.OP_LOADNIL,(Instruction i, LuaVM vm) =>{ vm.LoadNil(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgN, OPMode.IABC, OP.OP_GETUPVAL,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgK, OPMode.IABC, OP.OP_GETTABUP,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgK, OPMode.IABC, OP.OP_GETTABLE,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 0, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_SETTABUP,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 0, OPArgU.OPArgU, OPArgU.OPArgN, OPMode.IABC, OP.OP_SETUPVAL,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 0, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_SETTABLE,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgU, OPMode.IABC, OP.OP_NEWTABLE,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),

                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgK, OPMode.IABC, OP.OP_SELE,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_ADD,(Instruction i, LuaVM vm) =>{ vm.DoubleOperator(i,Paser.TokenType.Plus);}),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_SUB,(Instruction i, LuaVM vm) =>{ vm.DoubleOperator(i,Paser.TokenType.Minus);}),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_MUL,(Instruction i, LuaVM vm) =>{ vm.DoubleOperator(i,Paser.TokenType.Star);}),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_MOD,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_POW,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_DIV,(Instruction i, LuaVM vm) =>{ vm.DoubleOperator(i,Paser.TokenType.Slash);}),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_IDIV,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_BAND,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_BOR,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_SHL,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_SHR,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgN, OPMode.IABC, OP.OP_UNM,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgN, OPMode.IABC, OP.OP_BNOT,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgN, OPMode.IABC, OP.OP_NOT,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgN, OPMode.IABC, OP.OP_LEN,(Instruction i, LuaVM vm) =>{ vm.Len(i);}),

                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgR, OPMode.IABC, OP.OP_CONCAT,(Instruction i, LuaVM vm) =>{ vm.Concat(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgN, OPMode.IASBX, OP.OP_JMP,(Instruction i, LuaVM vm) =>{ vm.JMP(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_EQ,(Instruction i, LuaVM vm) =>{ vm.DoubleOperator(i,Paser.TokenType.Equal);}),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_LT,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgK, OPArgU.OPArgK, OPMode.IABC, OP.OP_LE,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgN, OPArgU.OPArgU, OPMode.IABC, OP.OP_TEST,(Instruction i, LuaVM vm) =>{ vm.Test(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgU, OPMode.IABC, OP.OP_TESTSET,(Instruction i, LuaVM vm) =>{ vm.TestSet(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgU, OPMode.IABC, OP.OP_CALL,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgU, OPMode.IABC, OP.OP_TAILCALL,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgN, OPMode.IABC, OP.OP_RETURN,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgN, OPMode.IASBX, OP.OP_FORLOOP,(Instruction i, LuaVM vm) =>{ vm.ForLoop(i);}),

                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgN, OPMode.IASBX, OP.OP_FORPREP,(Instruction i, LuaVM vm) =>{ vm.ForPrep(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgN, OPArgU.OPArgU, OPMode.IABC, OP.OP_TFORCALL,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgR, OPArgU.OPArgN, OPMode.IASBX, OP.OP_TFORLOOP,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgU, OPMode.IABC, OP.OP_SETLIST,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgN, OPMode.IABX, OP.OP_CLOSURE,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgN, OPMode.IABC, OP.OP_VARARG,(Instruction i, LuaVM vm) =>{ vm.Move(i);}),
                                                new OpCode(0, 1, OPArgU.OPArgU, OPArgU.OPArgU, OPMode.IAX, OP.OP_EXTRAARG,(Instruction i, LuaVM vm) =>{ vm.Move(i);})};

        readonly static int MaxARGBX = 1 << 18 - 1;// 2^18 - 1 = 262143
        readonly static int MaxARGSBX = (1 << 18 - 1) >> 1; // 262143 / 2 = 131071

        public readonly uint instruction;

        public Instruction(uint i)
        {
            instruction = i;
        }

        public int OpCode
        {
            get
            {
                return (int)(instruction & 0x3f);
            }
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
                return OpCodeTable[this.OpCode].Name;
            }
        }

        public OPMode InstructionMode
        {
            get
            {
                return OpCodeTable[this.OpCode].OpMode;
            }
        }

        public OPArgU OperandBMode
        {
            get
            {
                return OpCodeTable[this.OpCode].ArgBMode;
            }
        }

        public OPArgU OperandCMode
        {
            get
            {
                return OpCodeTable[this.OpCode].ArgCMode;
            }
        }

        public static OpCode[] OpCodeTable { get => opCodeTable;}

        public void Execute(LuaVM vm)
        {
            OpCodeTable[OpCode].Action(this, vm);
        }
    }
}
