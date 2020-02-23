using LuaVM.Paser;
using LuaVM.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.Codegen
{
    public class Codegen
    {

        public class LocalVarInfo
        {
            string identifier;
            int scopeLv;
            int regIndex;
            bool captured;
            LocalVarInfo prev;

            public LocalVarInfo(string identifier, int scopeLv, int regIndex, bool captured)
            {
                this.identifier = identifier;
                this.scopeLv = scopeLv;
                this.regIndex = regIndex;
                this.captured = captured;
            }

            /// <summary>
            /// 变量名
            /// </summary>
            public string Identifier { get => identifier; set => identifier = value; }
            /// <summary>
            /// 作用域索引
            /// </summary>
            public int ScopeLv { get => scopeLv; set => scopeLv = value; }
            /// <summary>
            /// 寄存器索引
            /// </summary>
            public int RegIndex { get => regIndex; set => regIndex = value; }
            /// <summary>
            /// 是否被闭包捕捉
            /// </summary>
            public bool Captured { get => captured; set => captured = value; }
            public LocalVarInfo Prev { get => prev; set => prev = value; }
        }
        /// <summary>
        /// 函数捕获变量信息
        /// </summary>
        public class UpValInfo
        {
            int localVarRegIndex;
            int upValIndex;
            int index;

            public UpValInfo(int localVarRegIndex, int upValIndex, int index)
            {
                this.localVarRegIndex = localVarRegIndex;
                this.upValIndex = upValIndex;
                this.index = index;
            }

            /// <summary>
            /// 寄存器索引
            /// </summary>
            public int LocalVarRegIndex { get => localVarRegIndex; }
            /// <summary>
            /// 该闭包捕获变量在上层作用域的UpVal表索引
            /// </summary>
            public int UpValIndex { get => upValIndex; }
            /// <summary>
            /// 在函数中出现的顺序
            /// </summary>
            public int Index { get => index; }
        }
        public class FuncInfo
        {
            int usedReg;
            int maxReg;
            int scopeLv;
            int nParam;
            bool isVararg;
            List<FuncInfo> childFunc;
            /// <summary>
            /// 上层函数
            /// </summary>
            FuncInfo parent;
            List<List<int>> breakTable;
            /// <summary>
            /// 局部变量字典
            /// </summary>
            Dictionary<string, LocalVarInfo> VarDic;
            Dictionary<object, int> constDic;
            /// <summary>
            /// 捕获变量字典
            /// </summary>
            Dictionary<string, UpValInfo> upVarDic;
            /// <summary>
            /// 按变量顺序存储变量
            /// </summary>
            LinkedList<LocalVarInfo> localVars;
            public int UsedReg { get => usedReg; set => usedReg = value; }
            public int MaxReg { get => maxReg; }
            public List<Instruction> InstructionList { get => instructionList; set => instructionList = value; }
            public Dictionary<string, LocalVarInfo> VarDic1 { get => VarDic; }
            public Dictionary<object, int> ConstDic { get => constDic; set => constDic = value; }
            public bool IsVararg { get => isVararg; set => isVararg = value; }
            public List<FuncInfo> ChildFunc { get => childFunc; set => childFunc = value; }
            public Dictionary<string, UpValInfo> UpVarDic { get => upVarDic; set => upVarDic = value; }
            public int NParam { get => nParam; set => nParam = value; }

            public FuncInfo()
            {
                childFunc = new List<FuncInfo>();
                breakTable = new List<List<int>>();
                UpVarDic = new Dictionary<string, UpValInfo>();
                localVars = new LinkedList<LocalVarInfo>();
                constDic = new Dictionary<object, int>();
                instructionList = new List<Instruction>();
                VarDic = new Dictionary<string, LocalVarInfo>();
            }
            public int IndexOfConstVar(object val)
            {
                if(constDic.ContainsKey(val))
                {
                    return constDic[val];
                }
                else
                {
                    int index = constDic.Count;
                    constDic.Add(val, index);
                    return index;
                }
            }
            List<Instruction> instructionList;

            /// <summary>
            /// 分配一个寄存器，上限255.索引从0开始
            /// </summary>
            /// <returns></returns>
            public int AllocReg()
            {
                usedReg++;
                if (usedReg >= 255)
                {
                    throw new Exception("寄存器数量达到上限！");
                }
                else if (usedReg > maxReg)
                {
                    maxReg = usedReg;
                }
                return usedReg - 1;
            }

            /// <summary>
            /// 连续分配n个寄存器，返回第一个寄存器索引。上限255.索引从0开始
            /// </summary>
            /// <param name="regNum"></param>
            /// <returns></returns>
            public int AllocRegs(int regNum)
            {
                usedReg += regNum;
                if (usedReg >= 255)
                {
                    throw new Exception("寄存器数量达到上限！");
                }
                else if (usedReg > maxReg)
                {
                    maxReg = usedReg;
                }
                return usedReg - regNum;
            }

            /// <summary>
            /// 回收最近一个分配的寄存器
            /// </summary>
            public void FreeReg()
            {
                usedReg--;
            }

            /// <summary>
            /// 回收最近分配的n个寄存器
            /// </summary>
            public void FreeRegs(int regNum)
            {
                usedReg -= regNum;
            }

            /// <summary>
            /// 添加一个局部变量并且分配寄存器索引
            /// </summary>
            /// <param name="identifier"></param>
            /// <returns></returns>
            public int AddLocalVar(string identifier)
            {
                LocalVarInfo localVar = new LocalVarInfo(identifier, this.scopeLv, AllocReg(), false);
                localVars.AddFirst(localVar);
                //如果已经有同名变量，则在当前作用域覆盖掉同名变量。
                if (VarDic1.ContainsKey(identifier))
                {
                    localVar.Prev = VarDic1[identifier];
                    //先让该变量拿到之前声明变量的指针,然后覆盖之前变量
                    VarDic1[identifier] = localVar;
                    return localVar.RegIndex;
                }
                else
                {
                    VarDic1.Add(identifier, localVar);
                    return localVar.RegIndex;
                }
            }

            /// <summary>
            /// 查询某变量是否已经分配寄存器，如果是则返回寄存器索引，不是则返回-1
            /// </summary>
            /// <param name="identifier"></param>
            /// <returns></returns>
            public int RegIndexOfVar(string identifier)
            {
                if (VarDic1.ContainsKey(identifier))
                {
                    return VarDic1[identifier].RegIndex;
                }
                return -1;
            }

            /// <summary>
            /// 退出作用域
            /// </summary>
            public void ExitScope()
            {
                scopeLv--;
                var last = localVars.Last;
                while (last.Previous != null)
                {
                    if (last.Value.ScopeLv > scopeLv)
                    {
                        RemoveLocalVar(last.Value);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            /// <summary>
            /// 回收当前作用域的所有局部变量
            /// </summary>
            /// <param name="localVar"></param>
            public void RemoveLocalVar(LocalVarInfo localVar)
            {
                FreeReg();
                //若没有同名变量，直接去除
                if (localVar.Prev == null)
                {
                    VarDic1.Remove(localVar.Identifier);
                    localVars.Remove(localVar);
                }
                //若果存在同名变量且作用域相同，则先删除该变量再递归删除同名变量。
                else if (localVar.ScopeLv == localVar.Prev.ScopeLv)
                {
                    RemoveLocalVar(localVar.Prev);
                }
                else
                {
                    //有同名变量且作用域在上层，则直接释放该变量，更新变量索引
                    VarDic1[localVar.Identifier] = localVar.Prev;
                    localVars.Remove(localVar);
                }
            }

            /// <summary>
            /// 进入新作用域
            /// </summary>
            /// <param name="breakable"></param>
            public void EnterScope(bool breakable)
            {
                scopeLv++;
                if (breakable)
                {
                    breakTable.Add(new List<int>());
                }
                else
                {
                    breakTable.Add(null);
                }
            }

            public void AddBreakJmp(int pc)
            {
                for (int i = scopeLv; i >= 0; i--)
                {
                    if (breakTable[i] != null)
                    {
                        breakTable[i].Add(pc);
                        return;
                    }
                }
                throw new Exception("没有与break对应的作用快！");
            }

            /// <summary>
            /// 该变量是否已经绑定upval，是返回索引，否则尝试绑定，失败返回-1
            /// </summary>
            /// <param name="identifier"></param>
            /// <returns></returns>
            public int UpValOfIndex(string identifier)
            {
                if (UpVarDic.ContainsKey(identifier))
                {
                    return UpVarDic[identifier].Index;
                }
                else if (parent != null)
                {
                    //如果上层作用域有该变量，则直接捕获
                    if (parent.VarDic1.ContainsKey(identifier))
                    {
                        var localVar = VarDic1[identifier];
                        int upValIndex = UpVarDic.Count;
                        UpValInfo upValInfo = new UpValInfo(localVar.RegIndex, -1, upValIndex);
                        localVar.Captured = true;
                        return upValIndex;
                    }
                    //如果上层也没有，则递归继续向上层寻找
                    else
                    {
                        int upValIndex = parent.UpValOfIndex(identifier);
                        var localVar = parent.UpVarDic[identifier];
                        int thisValIndex = UpVarDic.Count;
                        if (upValIndex != -1)
                        {
                            UpVarDic[identifier] = new UpValInfo(localVar.LocalVarRegIndex, upValIndex, thisValIndex);
                        }
                        return thisValIndex;
                    }
                }
                else
                {
                    return -1;
                }
            }

            public void EmitABC(int a, int b, int c, OP op)
            {
                Console.WriteLine(op + " a:" + a + " b:" + b + " c:" + c);
                InstructionList.Add(new Instruction((uint)(b << 23 | c << 14 | a << 6 | (int)op)));
            }

            public void EmitABX(int a, int bx, OP op)
            {
                Console.WriteLine(op + " a:" + a + " bx:" + bx);
                InstructionList.Add(new Instruction((uint)(bx << 14 | a << 6 | (int)op)));
            }

            public void EmitASBX(int a, int b, OP op)
            {
                Console.WriteLine(op + " a:" + a + " b:" + b);
                InstructionList.Add(new Instruction((uint)((b + Instruction.MaxARGSBX) << 14 | a << 6 | (int)op)));
            }

            public void EmitAX(int ax, OP op)
            {
                Console.WriteLine(op);
                InstructionList.Add(new Instruction((uint)(ax << 6 | (int)op)));
            }

            public int PC()
            {
                return InstructionList.Count - 1;
            }

            public void FixSBX(int pc, int sbx)
            {
                var inst = InstructionList[pc];
                var i = inst.instruction;
                i = i << 18 >> 18;
                i = i | (uint)((Instruction.MaxARGSBX + sbx) << 14);
                InstructionList[pc] = new Instruction(i);
            }


            public void EmitMove(int a, int b)
            {
                EmitABC(a, b, 0, OP.OP_MOVE);
            }

            public int EmitJMP(int a, int sbx)
            {
                EmitASBX(a, sbx, OP.OP_JMP);
                return PC();
            }

            public void EmitTest(int a, int c)
            {
                EmitABC(a, 0, c, OP.OP_TEST);
            }

            public void EmitTestSet(int a, int b ,int c)
            {
                EmitABC(a, b, c, OP.OP_TESTSET);
            }

            public void EmitLoadK(int a, object val)
            {
                int index = IndexOfConstVar(val);
                if(index < (1 << 18))
                {
                    EmitABX(a, index, OP.OP_LOADK);
                }
                else
                {
                    EmitABX(a, 0, OP.OP_LOADK);
                    EmitAX(index, OP.OP_EXTRAARG);
                }
            }

            public int EmitForPrep(int a, int sbx)
            {
                EmitASBX(a, sbx, OP.OP_FORPREP);
                return PC();
            }

            public int EmitForLoop(int a, int sbx)
            {
                EmitASBX(a, sbx, OP.OP_FORLOOP);
                return PC();
            }

            public void EmitTForCall(int a, int c)
            {
                EmitABC(a, 0, c, OP.OP_TFORCALL);
            }

            public void EmitTForLoop(int a, int sbx)
            {
                EmitASBX(a, sbx, OP.OP_TFORLOOP);
            }

            public void EmitLoadNil(int a, int n)
            {
                if (n == 0)
                    return;
                EmitABC(a, n - 1, 0, OP.OP_LOADNIL);
            }

            public void EmitSetUpval(int a, int b)
            {
                EmitABC(a, b, 0, OP.OP_SETUPVAL);
            }

            public void EmitGetUpval(int a, int b)
            {
                EmitABC(a, b, 0, OP.OP_GETUPVAL);
            }

            public void EmitSetTabUp(int a, int b, int c)
            {
                EmitABC(a, b, c, OP.OP_SETTABUP);
            }

            public void EmitGetTabUp(int a, int b, int c)
            {
                EmitABC(a, b, c, OP.OP_GETTABUP);
            }

            public void EmitTailCall(int a, int nArgs)
            {
                EmitABC(a, nArgs + 1, 0, OP.OP_TAILCALL);
            }

            public void EmitSelf(int a, int b, int c)
            {
                EmitABC(a, b, c, OP.OP_SELF);
            }

            public void EmitReturn(int a, int n)
            {
                EmitABC(a, n + 1, 0, OP.OP_RETURN);
            }

            public void EmitCall(int a, int nArgs, int nRet)
            {
                EmitABC(a, nArgs + 1, nRet + 1, OP.OP_CALL);
            }

            public void EmitGetTable(int a, int b, int c)
            {
                EmitABC(a, b, c, OP.OP_GETTABLE);
            }

            public void EmitSetList(int a, int b, int c)
            {
                EmitABC(a, b, c, OP.OP_SETLIST);
            }

            public void EmitNewTable(int a, int b, int c)
            {
                EmitABC(a, b, c, OP.OP_NEWTABLE);
            }

            public void EmitSetTable(int a, int b, int c)
            {
                EmitABC(a, b, c, OP.OP_SETTABLE);
            }

            public void EmitLoadBool(int a, int b, int c)
            {
                EmitABC(a, b, c, OP.OP_LOADBOOL);
            }

            public void EmitVararg(int a, int n)
            {
                EmitABC(a, n + 1, 0, OP.OP_VARARG);
            }

            public  void EmitClosure(int a, int bx)
            {
                EmitABX(a, bx, OP.OP_CLOSURE);
            }

            public void EmitUnaryOp(int a, int b, TokenType type)
            {
                switch(type)
                {
                    case TokenType.Not:
                        EmitABC(a, b, 0, OP.OP_NOT);
                        break;
                    case TokenType.Minus:
                        EmitABC(a, b, 0, OP.OP_UNM);
                        break;
                    case TokenType.Error:
                        EmitABC(a, b, 0, OP.OP_BNOT);
                        break;
                    case TokenType.Len:
                        EmitABC(a, b, 0, OP.OP_LEN);
                        break;
                }
            }

            public void EmitDoubleOperator(int a, int b, int c, TokenType type)
            {
                switch (type)
                {
                    case TokenType.Plus:
                        EmitABC(a, b, c, OP.OP_ADD);
                        break;
                    case TokenType.Minus:
                        EmitABC(a, b, c, OP.OP_SUB);
                        break;
                    case TokenType.Star:
                        EmitABC(a, b, c, OP.OP_MUL);
                        break;
                    case TokenType.Slash:
                        EmitABC(a, b, c, OP.OP_DIV);
                        break;
                    default:
                        {
                            switch(type)
                            {
                                case TokenType.Equal:
                                    EmitABC(1, b, c, OP.OP_EQ);
                                    break;
                                case TokenType.SmallerEqual:
                                    EmitABC(1, b, c, OP.OP_LE);
                                    break;
                                case TokenType.NotEqual:
                                    EmitABC(0, b, c, OP.OP_EQ);
                                    break;
                                case TokenType.Smaller:
                                    EmitABC(1, b, c, OP.OP_LT);
                                    break;
                                case TokenType.Bigger:
                                    EmitABC(0, b, c, OP.OP_LT);
                                    break;
                                case TokenType.BiggerEqual:
                                    EmitABC(0, b, c, OP.OP_LE);
                                    break;
                            }
                            EmitJMP(0, 1);
                            EmitLoadBool(a, 0, 1);
                            EmitLoadBool(a, 1, 0);
                            break;
                        }
                }
            }
        }

    }
}
