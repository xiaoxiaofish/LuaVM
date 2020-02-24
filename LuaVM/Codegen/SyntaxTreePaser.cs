using LuaVM.Paser;
using LuaVM.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuaVM.Codegen.Codegen;

namespace LuaVM.Codegen
{
    public class SyntaxTreePaser
    {
        public void CGBlock(FuncInfo funcInfo, BlockNode blockNode)
        {
            foreach (var stat in blockNode.StatsNode)
            {
                CGStat(funcInfo, stat);
            }

            if (blockNode.ReturnNode != null)
            {
                CGReturnStat(funcInfo, blockNode.ReturnNode.ExpList);
            }
        }

        private void CGStat(FuncInfo funcInfo, StatNode node)
        {
            switch (node.Type)
            {
                case GrammarNodeType.FunctionCallStat:
                    CGFuncCallStat(funcInfo, node as FuncCallStatNode);
                    break;
                case GrammarNodeType.BreakStat:
                    int pc = funcInfo.EmitJMP(0, 0);
                    funcInfo.AddBreakJmp(pc);
                    break;
                case GrammarNodeType.DoStat:
                    CGDoStat(funcInfo, node as DoStatNode);
                    break;
                case GrammarNodeType.RepeatStat:
                    CGRepeatStat(funcInfo, node as RepeatStatNode);
                    break;
                case GrammarNodeType.WhileStat:
                    CGWhileStat(funcInfo, node as WhileStatNode);
                    break;
                case GrammarNodeType.ForInStat:
                    CGWhileStat(funcInfo, node as WhileStatNode);
                    break;
                case GrammarNodeType.IfStat:
                    CGIfStat(funcInfo, node as IfStatNode);
                    break;
                case GrammarNodeType.ForNumStat:
                    CGForNumStat(funcInfo, node as ForNumStatNode);
                    break;
                case GrammarNodeType.LocalVarDecStat:
                    CGLocalVarDefStat(funcInfo, node as LocalVarDecStatNode);
                    break;
                case GrammarNodeType.LocalFuncDefStat:
                    CGLocalFuncDefStat(funcInfo, node as LoacalFuncDefStatNode);
                    break;
                case GrammarNodeType.AssignStat:
                    CGAssignStat(funcInfo, node as AssignStatNode);
                    break;
            }
        }

        private void CGDoStat(FuncInfo funcInfo, DoStatNode node)
        {
            funcInfo.EnterScope(false);
            CGBlock(funcInfo, node.Block);
            funcInfo.ExitScope();
        }

        private void CGReturnStat(FuncInfo funcInfo, List<ExpNode> returneExps)
        {
            if (returneExps == null)
            {
                funcInfo.EmitReturn(0, 0);
                return;
            }
            bool muleRet = IsVarargOrFuncCall(returneExps.Last());
            foreach (var exp in returneExps)
            {
                int r = funcInfo.AllocReg();
                if (r == returneExps.Count - 1 && muleRet)
                {
                    CGExp(funcInfo, exp, r, -1);
                }
                else
                {
                    CGExp(funcInfo, exp, r, 1);
                }
            }
            funcInfo.FreeRegs(returneExps.Count);
        }

        private void CGLocalFuncDefStat(FuncInfo funcInfo, LoacalFuncDefStatNode node)
        {
            int a = funcInfo.AddLocalVar(node.FuncName.name);
            CGFuncDefExp(funcInfo, node.FuncExp as FuncdefExpNode, a);
        }

        private void CGFuncCallStat(FuncInfo funcInfo, FuncCallStatNode node)
        {
            var r = funcInfo.AllocReg();
            FuncCallExpNode funcCall = new FuncCallExpNode();
            funcCall.Args = node.Args;
            funcCall.NameExp = node.NameExp;
            funcCall.PreExp = node.PreExp;
            CGFuncCallExp(funcInfo, funcCall, r, 0);
        }

        private void CGWhileStat(FuncInfo funcInfo, WhileStatNode node)
        {
            int beforePc = funcInfo.PC();
            int r = funcInfo.AllocReg();
            CGExp(funcInfo, node.Exp, r, 1);
            funcInfo.FreeReg();
            funcInfo.EmitTest(r, 0);
            int jumPC = funcInfo.EmitJMP(0, 0);
            funcInfo.EnterScope(true);
            CGBlock(funcInfo, node.DoStatNode.Block);
            funcInfo.EmitJMP(0, beforePc - funcInfo.PC());
            funcInfo.ExitScope();
            funcInfo.FixSBX(jumPC, funcInfo.PC() - jumPC);
        }

        private void CGRepeatStat(FuncInfo funcInfo, RepeatStatNode node)
        {
            funcInfo.EnterScope(true);
            int blockPC = funcInfo.PC();
            CGBlock(funcInfo, node.Block);

            int r = funcInfo.AllocReg();
            CGExp(funcInfo, node.Exp, r, 1);
            funcInfo.FreeReg();

            funcInfo.EmitTest(r, 0);
            funcInfo.EmitJMP(0, blockPC - funcInfo.PC() - 1);
            funcInfo.ExitScope();
        }

        private void CGIfStat(FuncInfo funcInfo, IfStatNode node)
        {
            var jmpToEndsPC = new int[node.Exp.Count];
            int jmpToNextExpPc = -1;
            int i = 0;
            foreach (var exp in node.Exp)
            {
                if (jmpToNextExpPc > 0)
                {
                    funcInfo.FixSBX(jmpToNextExpPc, funcInfo.PC() - jmpToNextExpPc);

                }
                int r = funcInfo.AllocReg();
                CGExp(funcInfo, exp, r, 1);
                funcInfo.FreeReg();
                funcInfo.EmitTest(r, 0);
                jmpToNextExpPc = funcInfo.EmitJMP(0, 0);
                funcInfo.EnterScope(false);
                CGBlock(funcInfo, node.Block);
                funcInfo.ExitScope();
                if (i < node.Exp.Count - 1)
                {
                    jmpToEndsPC[i] = funcInfo.EmitJMP(0, 0);
                }
                else
                {
                    jmpToEndsPC[i] = jmpToNextExpPc;
                }
                i++;
            }

            foreach (int j in jmpToEndsPC)
            {
                funcInfo.FixSBX(j, funcInfo.PC() - j);
            }
        }

        private void CGForNumStat(FuncInfo funcInfo, ForNumStatNode node)
        {
            funcInfo.EnterScope(true);
            LocalVarDecStatNode varDecStatNode = new LocalVarDecStatNode();
            varDecStatNode.ExpList = new List<ExpNode>() { node.InitExp, node.LimitExp, node.StepExp };
            if (node.StepExp == null)
            {
                varDecStatNode.ExpList[2] = new ConstExpNode(new Token(TokenType.Number, "1", -1));
            }
            varDecStatNode.NameList = new List<ConstExpNode>() {new ConstExpNode(new Token(TokenType.Identifier,"for_index",-1)),
                                                                new ConstExpNode(new Token(TokenType.Identifier,"for_limit",-1)),
                                                                new ConstExpNode(new Token(TokenType.Identifier,"for_step",-1))};
            CGLocalVarDefStat(funcInfo, varDecStatNode);
            funcInfo.AddLocalVar(node.VarName.name);
            int a = funcInfo.UsedReg - 4;
            int forPrepPC = funcInfo.EmitForPrep(a, 0);
            CGBlock(funcInfo, node.DoBlock.Block);
            int forLoopPC = funcInfo.EmitForLoop(a, 0);
            funcInfo.FixSBX(forPrepPC, forLoopPC - forPrepPC - 1);
            funcInfo.FixSBX(forLoopPC, forPrepPC - forLoopPC - 1);
            funcInfo.ExitScope();
        }

        private void CGForInStat(FuncInfo funcInfo, ForInStatNode node)
        {
            funcInfo.EnterScope(true);
            LocalVarDecStatNode varDecStatNode = new LocalVarDecStatNode();
            varDecStatNode.ExpList = node.ExpList;
            varDecStatNode.NameList = new List<ConstExpNode>() {new ConstExpNode(new Token(TokenType.Identifier,"for_generator",-1)),
                                                                new ConstExpNode(new Token(TokenType.Identifier,"for_state",-1)),
                                                                new ConstExpNode(new Token(TokenType.Identifier,"for_control",-1))};

            foreach (var name in node.NameList.NameList)
            {
                funcInfo.AddLocalVar(name.name);
            }

            int jmpToTFCPC = funcInfo.EmitJMP(0, 0);
            CGBlock(funcInfo, node.DoBlock.Block);
            funcInfo.FixSBX(jmpToTFCPC, funcInfo.PC() - jmpToTFCPC);
            var geneartor = funcInfo.VarDic1["for_generator"];
            funcInfo.EmitTForCall(geneartor.RegIndex, node.NameList.NameList.Count);
            funcInfo.EmitTForLoop(geneartor.RegIndex + 2, jmpToTFCPC - funcInfo.PC() - 1);
            funcInfo.ExitScope();
        }

        private void CGLocalVarDefStat(FuncInfo funcInfo, LocalVarDecStatNode node)
        {
            int nExps = node.ExpList.Count;
            int nNames = node.NameList.Count;
            int oldReg = funcInfo.UsedReg;
            if (nExps == nNames)
            {
                foreach (var exp in node.ExpList)
                {
                    int a = funcInfo.AllocReg();
                    CGExp(funcInfo, exp, a, 1);
                }
            }
            else if (nExps > nNames)
            {
                int i = 0;
                foreach (var exp in node.ExpList)
                {
                    int a = funcInfo.AllocReg();
                    if (i == nExps - 1 && IsVarargOrFuncCall(exp))
                    {
                        CGExp(funcInfo, exp, a, 0);
                    }
                    else
                    {
                        CGExp(funcInfo, exp, a, 1);
                    }
                    i++;
                }
            }
            else
            {
                bool multRet = false;
                int i = 0;
                foreach (var exp in node.ExpList)
                {
                    int a = funcInfo.AllocReg();
                    if (i == nExps - 1 && IsVarargOrFuncCall(exp))
                    {
                        multRet = true;
                        int n = nNames - nExps - 1;
                        CGExp(funcInfo, exp, a, n);
                        funcInfo.AllocRegs(n - 1);
                    }
                    else
                    {
                        CGExp(funcInfo, exp, a, 1);
                    }
                    i++;
                }
                if (!multRet)
                {
                    int n = nNames - nExps;
                    int a = funcInfo.AllocRegs(n);
                    funcInfo.EmitLoadNil(a, n);
                }
            }

            funcInfo.UsedReg = oldReg;
            foreach (var nameNode in node.NameList)
            {
                funcInfo.AddLocalVar(nameNode.name);
            }
        }

        private void CGAssignStat(FuncInfo funcInfo, AssignStatNode node)
        {
            int nExps = node.ExpList.Count;
            int nVars = node.VarList.Count;
            int oldRegs = funcInfo.UsedReg;
            var tRegs = new int[nVars];
            var kRegs = new int[nVars];
            var vRegs = new int[nVars];

            int i = 0;
            foreach (var exp in node.VarList)
            {
                TableAccessExpNode taExp = null;
                ConstExpNode constExp = null;
                if ((taExp = exp as TableAccessExpNode) != null)
                {
                    tRegs[i] = funcInfo.AllocReg();
                    CGExp(funcInfo, taExp.PreExp, tRegs[i], 1);

                    kRegs[i] = funcInfo.AllocReg();
                    CGExp(funcInfo, taExp.Exp, kRegs[i], 1);
                }
                i++;
            }
            for (int j = 0; j < nVars; j++)
            {
                vRegs[j] = funcInfo.UsedReg + j;
            }
            if (nExps >= nVars)
            {
                i = 0;
                foreach (var exp in node.ExpList)
                {
                    int a = funcInfo.AllocReg();
                    if (i > nVars && i == nExps - 1 && IsVarargOrFuncCall(exp))
                    {
                        CGExp(funcInfo, exp, a, 0);
                    }
                    else
                    {
                        CGExp(funcInfo, exp, a, 1);
                    }
                    i++;
                }
            }
            else
            {
                bool multRet = false;
                i = 0;
                foreach (var exp in node.ExpList)
                {
                    int a = funcInfo.AllocReg();
                    if (i == nExps - 1 && IsVarargOrFuncCall(exp))
                    {
                        multRet = true;
                        int n = nVars - nExps + 1;
                        CGExp(funcInfo, exp, a, n);
                        funcInfo.AllocRegs(n - 1);
                    }
                    else
                    {
                        CGExp(funcInfo, exp, a, 1);
                    }
                    i++;
                }
                if (!multRet)
                {
                    int n = nVars - nExps;
                    int a = funcInfo.AllocRegs(n);
                    funcInfo.EmitLoadNil(a, n);
                }
            }

            i = 0;
            foreach (var exp in node.VarList)
            {
                ConstExpNode varNode = null;
                int r = 0;
                if ((varNode = exp as ConstExpNode) != null)
                {
                    var name = varNode.name;
                    LocalVarInfo info = null;
                    if (funcInfo.VarDic1.TryGetValue(name, out info))
                    {
                        funcInfo.EmitMove(info.RegIndex, vRegs[i]);
                    }
                    else if (funcInfo.UpValOfIndex(name) >= 0)
                    {
                        int b = funcInfo.UpValOfIndex(name);
                        funcInfo.EmitSetUpval(vRegs[i], b);
                    }
                    else if (funcInfo.ConstDic.TryGetValue(varNode.name, out r))
                    {
                        funcInfo.EmitLoadK(vRegs[i], varNode.name);
                    }
                    else //全局变量
                    {
                        int a = funcInfo.UpValOfIndex("_ENV");
                        if (kRegs[i] <= 0)
                        {
                            int b = 0x100 + funcInfo.IndexOfConstVar(name);
                            funcInfo.EmitSetTabUp(a, b, vRegs[i]);
                        }
                        else
                        {
                            funcInfo.EmitSetTabUp(a, kRegs[i], vRegs[i]);
                        }

                    }
                }
                else
                {
                    funcInfo.EmitSetTable(tRegs[i], kRegs[i], vRegs[i]);
                }
                i++;
            }
            funcInfo.UsedReg = oldRegs;
        }

        private bool IsVarargOrFuncCall(ExpNode node)
        {
            switch (node.Type)
            {
                case GrammarNodeType.FuncCallExp:
                    return true;
            }
            ConstExpNode cNode = null;
            if (node is ConstExpNode)
            {
                cNode = node as ConstExpNode;
                if (cNode.ExpType == ConstExpNode.ConstExpType.VarargExp)
                    return true;
            }
            return false;

        }

        private void CGExp(FuncInfo funcInfo, ExpNode node, int r, int n)
        {
            switch (node.Type)
            {
                case GrammarNodeType.ConstExp:
                    var constNode = node as ConstExpNode;
                    switch (constNode.ExpType)
                    {
                        case ConstExpNode.ConstExpType.FalseExp:
                            funcInfo.EmitLoadBool(r, 0, 0);
                            break;
                        case ConstExpNode.ConstExpType.IdentifierExp:
                            CGNameExp(funcInfo, constNode, r);
                            break;
                        case ConstExpNode.ConstExpType.NilExp:
                            funcInfo.EmitLoadNil(r, n);
                            break;
                        case ConstExpNode.ConstExpType.NumberExp:
                            funcInfo.EmitLoadK(r, constNode.tokenValue);
                            break;
                        case ConstExpNode.ConstExpType.StringExp:
                            funcInfo.EmitLoadK(r, constNode.tokenValue);
                            break;
                        case ConstExpNode.ConstExpType.TrueExp:
                            funcInfo.EmitLoadBool(r, 1, 0);
                            break;
                        case ConstExpNode.ConstExpType.VarargExp:
                            CGVarargExp(funcInfo, constNode, r, n);
                            break;
                    }
                    break;
                case GrammarNodeType.ParenExp:
                    CGExp(funcInfo, (node as ParenExpNode).Exp, r, 1);
                    break;
                case GrammarNodeType.TableAccessExp:
                    CGTableAccessExp(funcInfo, node as TableAccessExpNode, r);
                    break;
                case GrammarNodeType.TableConstructorExp:
                    CGTableConstructorExp(funcInfo, node as TableConstructorExpNode, r);
                    break;
                case GrammarNodeType.UnaryExp:
                    CGUnaryExp(funcInfo, node as UnaryExpNode, r);
                    break;
                case GrammarNodeType.FuncCallExp:
                    CGFuncCallExp(funcInfo, node as FuncCallExpNode, r, n);
                    break;
                case GrammarNodeType.ConcatExp:
                    CGConcatExp(funcInfo, node as ConcatExpNode, r);
                    break;
                case GrammarNodeType.FunctionDefExp:
                    CGFuncDefExp(funcInfo, node as FuncdefExpNode, r);
                    break;
                case GrammarNodeType.DoubleOperationExp:
                    CGBinOpExp(funcInfo, node as DoubleOperationExpNode, r);
                    break;
            }
        }

        private void CGVarargExp(FuncInfo funcInfo, ConstExpNode node, int a, int n)
        {
            if (!funcInfo.IsVararg)
            {
                throw new Exception("this function is not use ... beacuse it is not vararg function");
            }
            else
            {
                funcInfo.EmitVararg(a, n);
            }
        }

        public void CGFuncDefExp(FuncInfo funcInfo, FuncdefExpNode node, int a)
        {
            FuncInfo newFuncInfo = new FuncInfo();
            newFuncInfo.NParam = node.ParList.Count;
            funcInfo.ChildFunc.Add(newFuncInfo);
            foreach (var param in node.ParList)
            {
                newFuncInfo.AddLocalVar(param.name);
            }

            CGBlock(newFuncInfo, node.Block);
            newFuncInfo.ExitScope();
            newFuncInfo.EmitReturn(0, 0);
            int bx = funcInfo.ChildFunc.Count - 1;
            funcInfo.EmitClosure(a, bx);
        }

        private void CGTableConstructorExp(FuncInfo funcInfo, TableConstructorExpNode node, int a)
        {
            int nArray = 0;

            foreach (var key in node.KeyExpList)
            {
                var nilNode = key as ConstExpNode;
                if (nilNode != null && nilNode.ExpType == ConstExpNode.ConstExpType.NilExp)
                {
                    nArray++;
                }

            }
            int nExps = node.KeyExpList.Count;
            funcInfo.EmitNewTable(a, nArray, nExps - nArray);

            int i = 0;
            foreach (var key in node.KeyExpList)
            {
                var nilNode = key as ConstExpNode;
                var valNode = node.ValExpList[i];
                if (nilNode.ExpType == ConstExpNode.ConstExpType.NilExp)
                {

                }
                int b = funcInfo.AllocReg();
                CGExp(funcInfo, key, b, 1);
                int c = funcInfo.AllocReg();
                CGExp(funcInfo, valNode, c, 1);
                funcInfo.FreeRegs(2);
                funcInfo.EmitSetTable(a, b, c);
                i++;
            }
        }

        private void CGUnaryExp(FuncInfo funcInfo, UnaryExpNode node, int a)
        {
            int b = funcInfo.AllocReg();
            CGExp(funcInfo, node.Exp, b, 1);
            funcInfo.EmitUnaryOp(a, b, node.OpType);
            funcInfo.FreeReg();
        }

        private void CGConcatExp(FuncInfo funcInfo, ConcatExpNode node, int a)
        {
            foreach (var exp in node.ExpList)
            {
                int r = funcInfo.AllocReg();
                CGExp(funcInfo, exp, r, 1);
            }
            int c = funcInfo.UsedReg - 1;
            int b = node.ExpList.Count - 1;
            funcInfo.FreeRegs(c - b + 1);
            funcInfo.EmitABC(a, b, c, OP.OP_CONCAT);
        }

        private void CGBinOpExp(FuncInfo funcInfo, DoubleOperationExpNode node, int a)
        {
            switch (node.OpType)
            {
                case TokenType.And:
                case TokenType.Or:
                    {
                        int b = funcInfo.AllocReg();
                        CGExp(funcInfo, node.Exp1, b, 1);
                        funcInfo.FreeReg();
                        if (node.OpType == TokenType.And)
                        {
                            funcInfo.EmitTestSet(a, b, 0);
                        }
                        else
                        {
                            funcInfo.EmitTestSet(a, b, 1);
                        }
                        int jmpPC = funcInfo.EmitJMP(0, 0);
                        int c = funcInfo.AllocReg();
                        CGExp(funcInfo, node.Exp2, c, 1);
                        funcInfo.FreeReg();
                        funcInfo.EmitMove(a, c);
                        funcInfo.FixSBX(jmpPC, funcInfo.PC() - jmpPC);
                        break;
                    }
                default:
                    {
                        int b = funcInfo.AllocReg();
                        CGExp(funcInfo, node.Exp1, b, 1);
                        int c = funcInfo.AllocReg();
                        CGExp(funcInfo, node.Exp2, c, 1);
                        funcInfo.EmitDoubleOperator(a, b, c, node.OpType);
                        funcInfo.FreeRegs(2);
                        break;
                    }
            }
        }

        private void CGNameExp(FuncInfo funcInfo, ConstExpNode node, int a)
        {
            LocalVarInfo varInfo = null;
            int r = 0;
            if (funcInfo.VarDic1.TryGetValue(node.name, out varInfo))
            {
                funcInfo.EmitMove(a, varInfo.RegIndex);
            }
            else if (funcInfo.UpValOfIndex(node.name) != -1)
            {
                r = funcInfo.UpValOfIndex(node.name);
                funcInfo.EmitGetUpval(a, r);
            }
            //else if (funcInfo.ConstDic.TryGetValue(node.name, out r))
            //   {
            //       funcInfo.EmitLoadK(a, node.name);
            //   }
            else
            {
                var exp = new TableAccessExpNode();
                exp.PreExp = new ConstExpNode(new Token(TokenType.Identifier, "_ENV", -1));
                exp.Exp = new ConstExpNode(new Token(TokenType.String, node.name, -1));
                CGTableAccessExp(funcInfo, exp, a);
            }
        }

        private void CGTableAccessExp(FuncInfo funcInfo, TableAccessExpNode node, int a)
        {
            int b = funcInfo.AllocReg();
            CGExp(funcInfo, node.PreExp, b, 1);
            int c = funcInfo.AllocReg();
            CGExp(funcInfo, node.Exp, c, 1);
            funcInfo.EmitGetTable(a, b, c);
            funcInfo.FreeRegs(2);
        }

        private void CGFuncCallExp(FuncInfo funcInfo, FuncCallExpNode node, int a, int n)
        {
            int nArgs = PrepFuncCall(funcInfo, node, a);
            funcInfo.EmitCall(a, nArgs, n);
        }

        private int PrepFuncCall(FuncInfo funcInfo, FuncCallExpNode node, int a)
        {
            int nArgs = node.Args.Count;
            bool lastVarIsVarargOrFuncCall = false;
            CGExp(funcInfo, node.PreExp, a, 1);
            if (node.NameExp.ExpType != ConstExpNode.ConstExpType.NilExp)
            {
                int c = 0x100 + funcInfo.IndexOfConstVar(node.NameExp.name);
                funcInfo.EmitSelf(a, a, c);
            }
            int i = 0;
            foreach (var arg in node.Args)
            {
                int r = funcInfo.AllocReg();
                if (i == nArgs - 1 && IsVarargOrFuncCall(arg))
                {
                    lastVarIsVarargOrFuncCall = true;
                    CGExp(funcInfo, arg, r, -1);
                }
                else
                {
                    CGExp(funcInfo, arg, r, 1);

                }
                i++;
            }
            funcInfo.FreeRegs(nArgs);
            if (node.NameExp.ExpType != ConstExpNode.ConstExpType.NilExp)
            {
                nArgs++;
            }
            if (lastVarIsVarargOrFuncCall)
            {
                nArgs--;
            }
            return nArgs;
        }


    }
}
