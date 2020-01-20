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
            List<uint> instructions;
            /// <summary>
            /// 上层函数
            /// </summary>
            FuncInfo parent;
            List<List<int>> breakTable;
            /// <summary>
            /// 局部变量字典
            /// </summary>
            Dictionary<string, LocalVarInfo> VarDic;
            /// <summary>
            /// 捕获变量字典
            /// </summary>
            Dictionary<string, UpValInfo> UpVarDic;
            /// <summary>
            /// 按变量顺序存储变量
            /// </summary>
            LinkedList<LocalVarInfo> localVars;
            public int UsedReg { get => usedReg;}
            public int MaxReg { get => maxReg;}

           /// <summary>
           /// 分配一个寄存器，上限255.索引从0开始
           /// </summary>
           /// <returns></returns>
            public int AllocReg()
            {
                usedReg++;
                if(usedReg >= 255)
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
                localVars.AddLast(localVar);
                //如果已经有同名变量，则在当前作用域覆盖掉同名变量。
                if (VarDic.ContainsKey(identifier))
                {
                    //先让该变量拿到之前声明变量的指针,然后覆盖之前变量
                    localVar.Prev = VarDic[identifier].Prev;
                    VarDic[identifier] = localVar;
                    return localVar.RegIndex;
                }
                else
                {
                    VarDic.Add(identifier, localVar);
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
                if(VarDic.ContainsKey(identifier))
                {
                    return VarDic[identifier].RegIndex;
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
                while(last.Previous != null)
                {
                    if(last.Value.ScopeLv > scopeLv)
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
                if(localVar.Prev == null)
                {
                    VarDic[localVar.Identifier] = null;
                }
                //若果存在同名变量且作用域相同，则先删除该变量再递归删除同名变量。
                else if(localVar.ScopeLv == localVar.Prev.ScopeLv)
                {
                    VarDic[localVar.Identifier] = localVar.Prev;
                    RemoveLocalVar(localVar.Prev);
                }
                else
                {
                    //有同名变量且作用域在上层，则直接释放该变量，更新变量索引
                    VarDic[localVar.Identifier] = localVar.Prev;
                }
            }

            /// <summary>
            /// 进入新作用域
            /// </summary>
            /// <param name="breakable"></param>
            public void EnterScope(bool breakable)
            {
                scopeLv++;
                if(breakable)
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
                for(int i = breakTable.Count - 1; i >= 0; i--)
                {
                    if(breakTable[i] != null )
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
                if(UpVarDic.ContainsKey(identifier))
                {
                    return UpVarDic[identifier].Index;
                }
                else if(parent != null)
                {
                    //如果上层作用域有该变量，则直接捕获
                    if(parent.VarDic.ContainsKey(identifier))
                    {
                        var localVar = VarDic[identifier];
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

            public void EmitABC(int a , int b, int c ,int d)
            {
                //instructions.Add(((int)opCode.ArgBMode << 23 | (int)opCode.ArgCMode << 14 | (int)opCode.))
            }
        }
       
    }
}
