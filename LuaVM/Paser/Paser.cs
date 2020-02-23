using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.Paser
{
    public enum GrammarNodeType
    {
        Chunk,
        Block,
        Stat,
        ReturnStat,
        Label,
        FuncName,
        VarList,
        Var,
        NameList,
        ExpList,
        Exp,
        PerFixExp,
        Args,
        FuncBody,
        ParList,
        FieldList,
        Field,
        FieldSep,
        BinOp,
        UnOp,

        IfStat,
        WhileStat,
        ForNumStat,
        ForInStat,
        ElseIfStat,
        ElseStat,
        DoStat,
        RepeatStat,
        BreakStat,
        LocalVarDecStat,
        AssignStat,
        LocalFuncDefStat,
        FunctionCallStat,
        DoubleOperationExp,
        UnaryExp,
        None,
        ConcatExp,
        TableConstructorExp,
        FunctionDefExp,
        ParenExp,
        TableAccessExp,
        FuncCallExp,
        NilExp,
        ConstExp,
        Terminator,
        Nil,
        Number,
        String,
        Boolean,
        Keyword,
    }

    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct TokenValue
    {
        [FieldOffset(0)]
        public string str;

        [FieldOffset(0)]
        public double number;
    }
    public class GrammarNode
    {

        private GrammarNodeType _type;
        protected GrammarNodeType type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                Console.WriteLine(_type);
            }
        }
        protected int line;
        public GrammarNodeType Type { get => type; }
        public GrammarNode()
        {
            Console.WriteLine(type);
        }

        public void Accept(GrammarNodeVisitor visitor)
        {
            visitor.Visitor(this);
        }

    }

    public class ChunckNode : GrammarNode
    {
        public ChunckNode() : base()
        {
            type = GrammarNodeType.Chunk;
        }
        public BlockNode Block;
    }

    public class TerminatorNode : ExpNode
    {
        private Token token;
        private TokenValue value;
        private TokenType termType;
        public TerminatorNode(Token token) : base()
        {
            type = GrammarNodeType.Terminator;
            this.token = token;
            this.termType = token.TokenType;
        }
    }

    public class BlockNode : GrammarNode
    {
        public BlockNode() : base()
        {
            type = GrammarNodeType.Block;
        }

        private List<StatNode> statsNode;
        private ReturnStatNode returnNode;
        public ReturnStatNode ReturnNode { get => returnNode; set => returnNode = value; }
        public List<StatNode> StatsNode { get => statsNode; set => statsNode = value; }
    }

    public class StatNode : GrammarNode
    {
        public StatNode() : base()
        {
            type = GrammarNodeType.Stat;
        }

        public StatNode(GrammarNodeType type) : base()
        {
            this.type = type;
        }
    }

    public class ReturnStatNode : StatNode
    {
        public ReturnStatNode() : base()
        {
            type = GrammarNodeType.ReturnStat;
            isEmpty = false;
        }
        private bool isEmpty;
        private List<ExpNode> expList;
        public List<ExpNode> ExpList { get => expList; set => expList = value; }
        public bool IsEmpty { get => isEmpty; set => isEmpty = value; }
    }

    public class IfStatNode : StatNode
    {
        public IfStatNode() : base()
        {
            type = GrammarNodeType.IfStat;
            elseIfStatNodes = new List<ElseIfStatNode>();
        }

        private List<ElseIfStatNode> elseIfStatNodes;
        private List<ExpNode> exp;
        private BlockNode block;
        private ElseStatNode elseStat;

        public List<ElseIfStatNode> ElseIfStatNodes { get => elseIfStatNodes; set => elseIfStatNodes = value; }
        public List<ExpNode> Exp { get => exp; set => exp = value; }
        public BlockNode Block { get => block; set => block = value; }
        public ElseStatNode ElseStat { get => elseStat; set => elseStat = value; }
    }

    public class ElseIfStatNode : StatNode
    {
        public ElseIfStatNode() : base()
        {
            type = GrammarNodeType.ElseIfStat;
        }
        private List<ExpNode> exp;
        private BlockNode block;

        public List<ExpNode> Exp { get => exp; set => exp = value; }
        public BlockNode Block { get => block; set => block = value; }
    }

    public class ElseStatNode : StatNode
    {
        public ElseStatNode() : base()
        {
            type = GrammarNodeType.ElseStat;
        }

        private BlockNode block;

        public BlockNode Block { get => block; set => block = value; }
    }

    public class WhileStatNode : StatNode
    {
        public WhileStatNode() : base()
        {
            type = GrammarNodeType.WhileStat;
        }

        private ExpNode exp;
        private DoStatNode doStatNode;

        public ExpNode Exp { get => exp; set => exp = value; }
        public DoStatNode DoStatNode { get => doStatNode; set => doStatNode = value; }
    }

    public class DoStatNode : StatNode
    {
        public DoStatNode() : base()
        {
            type = GrammarNodeType.DoStat;
        }
        private BlockNode block;

        public BlockNode Block { get => block; set => block = value; }
    }

    public class ForNumStatNode : StatNode
    {
        public ForNumStatNode() : base()
        {
            type = GrammarNodeType.ForNumStat;
        }

        private ConstExpNode varName;
        private ExpNode initExp;
        private ExpNode limitExp;
        private ExpNode stepExp;
        private DoStatNode doBlock;

        public ConstExpNode VarName { get => varName; set => varName = value; }
        public ExpNode InitExp { get => initExp; set => initExp = value; }
        public ExpNode LimitExp { get => limitExp; set => limitExp = value; }
        public ExpNode StepExp { get => stepExp; set => stepExp = value; }
        public DoStatNode DoBlock { get => doBlock; set => doBlock = value; }
    }

    public class NameListNode : ExpNode
    {
        public NameListNode() : base()
        {
            type = GrammarNodeType.NameList;
            NameList = new List<ConstExpNode>();
        }

        List<ConstExpNode> nameList;

        public List<ConstExpNode> NameList { get => nameList; set => nameList = value; }
    }

    public class LableStatNode : StatNode
    {
        public LableStatNode() : base()
        {
            type = GrammarNodeType.Label;
        }

        private ConstExpNode name;
        public ConstExpNode Name { get => name; set => name = value; }
    }

    public class ForInStatNode : StatNode
    {
        public ForInStatNode() : base()
        {
            type = GrammarNodeType.ForInStat;
        }

        private NameListNode nameList;
        private List<ExpNode> expList;
        private DoStatNode doBlock;

        public NameListNode NameList { get => nameList; set => nameList = value; }
        public List<ExpNode> ExpList { get => expList; set => expList = value; }
        public DoStatNode DoBlock { get => doBlock; set => doBlock = value; }
    }

    public class RepeatStatNode : StatNode
    {
        public RepeatStatNode() : base()
        {
            type = GrammarNodeType.RepeatStat;
        }

        private BlockNode block;
        private ExpNode exp;

        public ExpNode Exp { get => exp; set => exp = value; }
        public BlockNode Block { get => block; set => block = value; }
    }

    public class LocalVarDecStatNode : StatNode
    {
        public LocalVarDecStatNode() : base()
        {
            type = GrammarNodeType.LocalVarDecStat;
            nameList = new List<ConstExpNode>();
            expList = new List<ExpNode>();
        }

        private List<ConstExpNode> nameList;
        private List<ExpNode> expList;

        public List<ConstExpNode> NameList { get => nameList; set => nameList = value; }
        public List<ExpNode> ExpList { get => expList; set => expList = value; }
    }

    public class AssignStatNode : StatNode
    {
        public AssignStatNode() : base()
        {
            type = GrammarNodeType.AssignStat;
            varList = new List<ExpNode>();
            expList = new List<ExpNode>();
        }

        private List<ExpNode> varList;
        private List<ExpNode> expList;

        public List<ExpNode> VarList { get => varList; set => varList = value; }
        public List<ExpNode> ExpList { get => expList; set => expList = value; }
    }

    public class LoacalFuncDefStatNode : StatNode
    {
        public LoacalFuncDefStatNode() : base()
        {
            type = GrammarNodeType.LocalFuncDefStat;
        }
        private ConstExpNode funcName;
        private ExpNode funcExp;

        public ConstExpNode FuncName { get => funcName; set => funcName = value; }
        public ExpNode FuncExp { get => funcExp; set => funcExp = value; }
    }

    public class FuncCallStatNode : StatNode
    {
        public FuncCallStatNode() : base()
        {
            type = GrammarNodeType.FunctionCallStat;
        }

        private ExpNode preExp;
        private ConstExpNode nameExp;
        private List<ExpNode> args;

        public ExpNode PreExp { get => preExp; set => preExp = value; }
        public ConstExpNode NameExp { get => nameExp; set => nameExp = value; }
        public List<ExpNode> Args { get => args; set => args = value; }

    }
    /// <summary>
    /// 重写表达式语法规则
    /// </summary>
    /// exp::=exp12
    /* exp::=exp12
    exp12::=exp11 { or exp11}
    exp11::=exp10 { and exp10}
    exp10::=exp9{('<'|>'|<='|>='|·~='|·==') exp9}
    exp9::=exp8{'|' exp8} exp8::=exp7{'~' exp7} exp7::=exp6{'&' exp6}
    exp6::=exp5{('<<'|1>>1) exp5}
    exp5::=exp4{'..' exp4}
    exp4::=exp3{('+'|-|·*"|/·| //'|'s') exp3}
    exp2::={(' not'|#|-|~)} exp1
    exp1::=exp0{'^'exp2}
    exp0::=nil | false | true | Numeral | Literalstring I...I functiondef I prefixexp I tableconstructor*/
    public class ExpNode : GrammarNode
    {

    }

    public class ConstExpNode : ExpNode
    {
        public ConstExpNode(Token token) : base()
        {
            type = GrammarNodeType.ConstExp;
            switch (token.TokenType)
            {
                case TokenType.String:
                    tokenValue = token.TokenValue;
                    ExpType = ConstExpType.StringExp;
                    break;
                case TokenType.Number:
                    tokenValue = double.Parse(token.TokenValue);
                    ExpType = ConstExpType.NumberExp;
                    break;
                case TokenType.True:
                    ExpType = ConstExpType.TrueExp;
                    break;
                case TokenType.False:
                    ExpType = ConstExpType.FalseExp;
                    break;
                case TokenType.Identifier:
                    name = token.TokenValue;
                    ExpType = ConstExpType.IdentifierExp;
                    break;
                case TokenType.Nil:
                    ExpType = ConstExpType.NilExp;
                    break;
                case TokenType.Vararg:
                    ExpType = ConstExpType.VarargExp;
                    break;
            }
        }

        public enum ConstExpType
        {
            NilExp,
            TrueExp,
            FalseExp,
            VarargExp,
            NumberExp,
            StringExp,
            IdentifierExp,
        }
        public struct Value
        {
            public double number;
            public string str;
        }

        public object tokenValue;
        public string name;
        public ConstExpType ExpType
        {
            get;
        }

        // private TokenValue tokenValue;

        /*   public TokenValue Value
           {
               get
               {
                   return tokenValue;
               }
           }*/


    }

    public class DoubleOperationExpNode : ExpNode
    {
        public DoubleOperationExpNode() : base()
        {
            type = GrammarNodeType.DoubleOperationExp;
        }

        private ExpNode exp1;
        private TokenType opType;
        private ExpNode exp;
        public ExpNode Exp1 { get => exp1; set => exp1 = value; }
        public TokenType OpType { get => opType; set => opType = value; }
        public ExpNode Exp2 { get => exp; set => exp = value; }
    }


    public class UnaryExpNode : ExpNode
    {
        public UnaryExpNode() : base()
        {
            type = GrammarNodeType.UnaryExp;
        }

        private ExpNode exp;
        private TokenType opType;

        public ExpNode Exp { get => exp; set => exp = value; }
        public TokenType OpType { get => opType; set => opType = value; }
    }

    public class ConcatExpNode : ExpNode
    {
        public ConcatExpNode() : base()
        {
            type = GrammarNodeType.ConcatExp;
        }

        private List<ExpNode> expList;
        public List<ExpNode> ExpList { get => expList; set => expList = value; }
    }

    public class TableConstructorExpNode : ExpNode
    {
        public TableConstructorExpNode() : base()
        {
            type = GrammarNodeType.TableConstructorExp;
        }

        private List<ExpNode> keyExpList;
        private List<ExpNode> valExpList;

        public List<ExpNode> KeyExpList { get => keyExpList; set => keyExpList = value; }
        public List<ExpNode> ValExpList { get => valExpList; set => valExpList = value; }
    }

    public class FuncdefExpNode : ExpNode
    {
        public FuncdefExpNode() : base()
        {
            type = GrammarNodeType.FunctionDefExp;
        }

        private List<ConstExpNode> parList;
        private bool isVararg;
        private BlockNode block;
        public List<ConstExpNode> ParList { get => parList; set => parList = value; }
        public bool IsVararg { get => isVararg; set => isVararg = value; }
        public BlockNode Block { get => block; set => block = value; }
    }

    public class ParenExpNode : ExpNode
    {
        public ParenExpNode() : base()
        {
            type = GrammarNodeType.ParenExp;
        }

        private ExpNode exp;

        public ExpNode Exp { get => exp; set => exp = value; }
    }

    public class TableAccessExpNode : ExpNode
    {
        public TableAccessExpNode() : base()
        {
            type = GrammarNodeType.TableAccessExp;
        }

        private ExpNode preExp;
        private ExpNode exp;

        public ExpNode PreExp { get => preExp; set => preExp = value; }
        public ExpNode Exp { get => exp; set => exp = value; }
    }

    public class FuncCallExpNode : ExpNode
    {
        public FuncCallExpNode() : base()
        {
            type = GrammarNodeType.FuncCallExp;
        }

        private ExpNode preExp;
        private ConstExpNode nameExp;
        private List<ExpNode> args;

        public ExpNode PreExp { get => preExp; set => preExp = value; }
        public ConstExpNode NameExp { get => nameExp; set => nameExp = value; }
        public List<ExpNode> Args { get => args; set => args = value; }
    }

    public class GrammarNodeVisitor
    {
        public void Visitor(GrammarNode node)
        {

        }
    }

    public class Paser
    {
        private TokenReader tokenReader;
        private Lexer.Lexer lexer;
        private Token token;
        private ChunckNode chunckNode;

        public ChunckNode ChunckNode { get => chunckNode;}

        public Paser(string codeFilePath, string keyWordFilePath)
        {
            lexer = new Lexer.Lexer(codeFilePath, keyWordFilePath);
            lexer.StartLexer();
            tokenReader = new TokenReader(lexer.TokenList);
        }

        private void ReadToken()
        {
            token = tokenReader.ReadToken();
        }

        public void StartPaser()
        {
            //try
           // {
                chunckNode = ChunckPaser();
          //  }
          //  catch(Exception e)
          //  {
           //     Console.WriteLine(e.Message);
          //  }
          //  Console.WriteLine("语法分析完成!");
        }

        private ChunckNode ChunckPaser()
        {
            ChunckNode chunckNode = new ChunckNode();

            chunckNode.Block = BlockPaser();
            return chunckNode;
        }

        private BlockNode BlockPaser()
        {
            BlockNode blockNode = new BlockNode();
            blockNode.StatsNode = StatsPaser();
            blockNode.ReturnNode = ReturnStatPaser();
            return blockNode;
        }

        private StatNode StatPaser()
        {
            switch (tokenReader.PeekOne().TokenType)
            {
                case TokenType.SemiColon:
                    return EmptyStatPaser();
                case TokenType.Break:
                    return BreakStatPaser();
                case TokenType.DoubleLable:
                    return LablePaser();
                case TokenType.Do:
                    return DoStatPaser();
                case TokenType.If:
                    return IfStatPaser();
                case TokenType.While:
                    return WhileStatPaser();
                case TokenType.Repeat:
                    return RepeatStatPaser();
                case TokenType.Local:
                    return LocalAssignOrFuncDefStatPaser();
                case TokenType.Function:
                    return NoLocalFuncDefStatPaser();
                case TokenType.For:
                    return ForStatPaser();
                default:
                    return AssignOrFuncCallStatPaser();
            }
        }

        private StatNode EmptyStatPaser()
        {
            StatNode grammarNode = new StatNode();
            ReadToken();
            return grammarNode;
        }

        private StatNode BreakStatPaser()
        {
            ReadToken();
            return new StatNode(GrammarNodeType.BreakStat);
        }

        private DoStatNode DoStatPaser()
        {
            ReadToken();
            DoStatNode doStatNode = new DoStatNode();
            doStatNode.Block = BlockPaser();
            if (tokenReader.PeekOne().TokenType == TokenType.SemiColon)
            {
                ReadToken();
            }

            if (tokenReader.PeekOne().TokenType == TokenType.End)
            {
                ReadToken();
                return doStatNode;
            }
            else
            {
                throw new Exception("没有与do匹配的end在" + token.Line + "行");
            }
        }

        private WhileStatNode WhileStatPaser()
        {
            ReadToken();
            WhileStatNode whileStatNode = new WhileStatNode();
            whileStatNode.Exp = ExpPaser();
            if (tokenReader.PeekOne().TokenType == TokenType.Do)
            {
                whileStatNode.DoStatNode = DoStatPaser();
            }
            else
            {
                throw new Exception("没有与while匹配的do在" + token.Line + "行");
            }
            return whileStatNode;
        }

        private IfStatNode IfStatPaser()
        {
            IfStatNode ifStatNode = new IfStatNode();
            ReadToken();
            ifStatNode.Exp = ExpListPaser();
            if (tokenReader.PeekOne().TokenType == TokenType.Then)
            {
                ReadToken();
                ifStatNode.Block = BlockPaser();
                while (tokenReader.PeekOne().TokenType == TokenType.Elseif)
                {
                    ifStatNode.ElseIfStatNodes.Add(ElseIfStatPaser());
                }
                if (tokenReader.PeekOne().TokenType == TokenType.Else)
                {
                    ifStatNode.ElseStat = ElseStatPaser();
                }

                if (tokenReader.PeekOne().TokenType == TokenType.End)
                {
                    ReadToken();
                }
                else
                {
                    throw new Exception("没有与if匹配的end在" + token.Line + "行");
                }
            }
            else
            {
                throw new Exception("没有与if匹配的Then在" + token.Line + "行");
            }
            return ifStatNode;
        }

        private ElseIfStatNode ElseIfStatPaser()
        {
            ElseIfStatNode elseIfStatNode = new ElseIfStatNode();
            ReadToken();
            elseIfStatNode.Exp = ExpListPaser();
            if (tokenReader.PeekOne().TokenType == TokenType.Then)
            {
                ReadToken();
                elseIfStatNode.Block = BlockPaser();
            }
            else
            {
                throw new Exception("没有与elseif匹配的Then在" + token.Line + "行");
            }
            return elseIfStatNode;
        }

        private ElseStatNode ElseStatPaser()
        {
            ElseStatNode elseStatNode = new ElseStatNode();
            ReadToken();
            elseStatNode.Block = BlockPaser();
            return elseStatNode;
        }

        private List<StatNode> StatsPaser()
        {
            List<StatNode> stats = new List<StatNode>();
            while (!tokenReader.IsBlockEnd(tokenReader.PeekOne()))
            {
                GrammarNode stat = StatPaser();
                if (stat.Type != GrammarNodeType.None)
                {
                    stats.Add(stat as StatNode);
                }
            }
            return stats;
        }

        private ReturnStatNode ReturnStatPaser()
        {
            if (tokenReader.PeekOne().TokenType != TokenType.Return)
            {
                ReturnStatNode returnStatNode = new ReturnStatNode();
                returnStatNode.IsEmpty = true;
                return returnStatNode;
            }
            Token token_ = tokenReader.ReadToken();
            switch (token_.TokenType)
            {
                case TokenType.Else:
                case TokenType.Elseif:
                case TokenType.Until:
                case TokenType.Eof:
                case TokenType.End:
                    return null;
                case TokenType.SemiColon:
                    return null;
                default:
                    {
                        ReturnStatNode returnStatNode = new ReturnStatNode();
                        returnStatNode.ExpList = ExpListPaser();
                        if (tokenReader.PeekOne().TokenType == TokenType.SemiColon)
                        {
                            tokenReader.ReadToken();
                        }
                        return returnStatNode;
                    }
            }
        }

        private List<ExpNode> ReturnStatsPaser()
        {
            //如果前瞻一个不是return，直接返回null
            if (tokenReader.PeekOne().TokenType != TokenType.Return)
                return new List<ExpNode>() { new ConstExpNode(new Token(TokenType.Nil,"",token.Line))};
            //是return 解析return表达式
            //读取该return
            Token token_ = tokenReader.ReadToken();
            //前瞻一个token
            Token peekToken = tokenReader.PeekOne();
            //如果是块结束，说明返回语句为空，直接返回null
            if (tokenReader.IsBlockEnd(peekToken))
            {
                return new List<ExpNode>() { new ConstExpNode(new Token(TokenType.Nil, "", token.Line)) };
            }
            //如果是分号同样返回空
            if (peekToken.TokenType == TokenType.SemiColon)
            {
                //跳过这个分号
                tokenReader.ReadToken();
                return new List<ExpNode>() { new ConstExpNode(new Token(TokenType.Nil, "", token.Line)) };
            }
            //都不是，说明是Exp列表，解析返回
            List<ExpNode> expList = ExpListPaser();
            if (tokenReader.PeekOne().TokenType == TokenType.SemiColon)
            {
                tokenReader.ReadToken();
            }
            return expList;
        }

        private StatNode RepeatStatPaser()
        {
            ReadToken();
            RepeatStatNode repeatStatNode = new RepeatStatNode();
            repeatStatNode.Block = BlockPaser();
            ReadToken();
            if (token.TokenType == TokenType.Until)
            {
                repeatStatNode.Exp = ExpPaser();
            }
            else
            {
                throw new Exception("没有与repea匹配的until在" + token.Line + "行");
            }
            return repeatStatNode;
        }

        private List<ExpNode> ExpListPaser()
        {
            List<ExpNode> expList = new List<ExpNode>();
            expList.Add(ExpPaser());
            while (tokenReader.PeekOne().TokenType == TokenType.Comma)
            {
                tokenReader.ReadToken();
                expList.Add(ExpPaser());
            }
            
            return expList;
        }

        private StatNode ForStatPaser()
        {
            ReadToken();
            if(tokenReader.PeekOne().TokenType == TokenType.Identifier)
            {
                ReadToken();
                ConstExpNode identifier = new ConstExpNode(token);
                if (tokenReader.PeekOne().TokenType == TokenType.Assignment)
                {
                    return ForNumStatPaser(identifier);
                }
                else
                {
                    return ForInStatPaser(token);
                }
            }
            else
            {
                throw new Exception("非法字符" + token.TokenValue + "在第" + token.Line + "行。");
            }
        }

        /// <summary>
        /// 普通型for循环
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        private ForNumStatNode ForNumStatPaser(ConstExpNode varName)
        {
            ForNumStatNode forNumStatNode = new ForNumStatNode();
            forNumStatNode.VarName = varName;
            //读取掉=
            ReadToken();
            forNumStatNode.InitExp = ExpPaser();
            //读取掉,
            ReadToken();
            if (token.TokenType == TokenType.Comma)
            {
                forNumStatNode.LimitExp = ExpPaser();
                //如果有冒号，就先读掉
                if (tokenReader.PeekOne().TokenType == TokenType.Comma)
                {
                    ReadToken();
                    forNumStatNode.StepExp = ExpPaser();
                }
                else
                {
                    //没有冒号直接解析
                    forNumStatNode.StepExp = ExpPaser();
                }
                if (tokenReader.PeekOne().TokenType == TokenType.Do)
                {
                    forNumStatNode.DoBlock = DoStatPaser();
                }
                else
                {
                    throw new Exception("没有do与for匹配在第" + token.Line + "行");
                }
            }
            else
            {
                throw new Exception("语法错误，缺少,在" + token.Line + "行");
            }
            return forNumStatNode;
        }

        /// <summary>
        /// foreach型for循环
        /// </summary>
        /// <param name="nameToken"></param>
        /// <returns></returns>
        private ForInStatNode ForInStatPaser(Token nameToken)
        {
            ForInStatNode forInStatNode = new ForInStatNode();
            forInStatNode.NameList = NameListPaser(nameToken);
            if (tokenReader.PeekOne().TokenType == TokenType.In)
            {
                ReadToken();
                forInStatNode.ExpList = ExpListPaser();
                forInStatNode.DoBlock = DoStatPaser();
            }
            else
            {
                throw new Exception("缺少与for匹配的in在第" + token.Line + "行");
            }
            return forInStatNode;
        }

        private NameListNode NameListPaser(Token nameToken)
        {
            NameListNode nameListNode = new NameListNode();
            nameListNode.NameList.Add(new ConstExpNode(token));
            while (tokenReader.PeekOne().TokenType == TokenType.Comma)
            {
                ReadToken();
                if (tokenReader.PeekOne().TokenType == TokenType.Identifier)
                {
                    ReadToken();
                    nameListNode.NameList.Add(new ConstExpNode(token));
                }
                else
                {
                    throw new Exception("非法字符" + token.TokenValue + "在第" + token.Line + "行。");
                }
            }
            return nameListNode;
        }

        private StatNode LocalAssignOrFuncDefStatPaser()
        {
            ReadToken();
            if (tokenReader.PeekOne().TokenType == TokenType.Function)
            {
                return LocalFuncDefStatPaser();
            }
            else if (tokenReader.PeekOne().TokenType == TokenType.Identifier)
            {
                return LocalVarDecStatPaser();
            }
            else
            {
                throw new Exception("非法字符" + token.TokenValue + "在第" + token.Line + "行。");
            }
        }

        private LoacalFuncDefStatNode LocalFuncDefStatPaser()
        {
            ReadToken();//跳过function关键字
            ReadToken();//读取标识符
            LoacalFuncDefStatNode loacalFuncDefStatNode = new LoacalFuncDefStatNode();
            if (token.TokenType == TokenType.Identifier)
            {
                ConstExpNode funcName = new ConstExpNode(token);
                loacalFuncDefStatNode.FuncName = funcName;
                loacalFuncDefStatNode.FuncExp = FuncdefExpPaser();
                return loacalFuncDefStatNode;
            }
            else
            {
                throw new Exception("非法字符" + token.TokenValue + "在第" + token.Line + "行。");
            }
        }

        private LocalVarDecStatNode LocalVarDecStatPaser()
        {
            ReadToken();
            LocalVarDecStatNode localVarDecStatNode = new LocalVarDecStatNode();
            localVarDecStatNode.NameList = NameListPaser(token).NameList;
            if (tokenReader.PeekOne().TokenType == TokenType.Assignment)
            {
                ReadToken();
                localVarDecStatNode.ExpList = ExpListPaser();
            }
            return localVarDecStatNode;
        }

        private StatNode AssignOrFuncCallStatPaser()
        {
            ExpNode preExp = PerfixExpPaser();
            if (preExp is FuncCallExpNode)
            {
                FuncCallStatNode funcCallStatNode = new FuncCallStatNode();
                FuncCallExpNode expNode = (preExp as FuncCallExpNode);
                funcCallStatNode.Args = expNode.Args;
                funcCallStatNode.NameExp = expNode.NameExp;
                funcCallStatNode.PreExp = expNode.PreExp;
                return funcCallStatNode;
            }
            else
            {
                return AssingStatPaser(preExp);
            }
        }

        private List<ExpNode> VarListPaser(ExpNode exp1)
        {
            List<ExpNode> varList = new List<ExpNode>();
            varList.Add(exp1);
            while (tokenReader.PeekOne().TokenType == TokenType.Comma)
            {
                ReadToken();
                ExpNode exp = PerfixExpPaser();
                if (exp is TableAccessExpNode || exp is NameListNode)
                {
                    varList.Add(exp);
                }
                else
                {
                    throw new Exception("非法表达式在第" + token.Line + "行");
                }
            }
            return varList;
        }

        private StatNode AssingStatPaser(ExpNode exp1)
        {
            AssignStatNode assignStat = new AssignStatNode();
            assignStat.VarList = VarListPaser(exp1);
            if (tokenReader.PeekOne().TokenType == TokenType.Assignment)
            {
                ReadToken();
                assignStat.ExpList = ExpListPaser();
            }
            else
            {
                throw new Exception("非法字符" + token.TokenValue + "在第" + token.Line + "行");
            }
            return assignStat;
        }

        private StatNode LablePaser()
        {
            //跳过::符号
            ReadToken();
            LableStatNode lableStatNode = new LableStatNode();
            if (tokenReader.PeekOne().TokenType == TokenType.Identifier)
            {
                ReadToken();
                lableStatNode.Name = new ConstExpNode(token);
            }
            else
            {
                throw new Exception("非法字符" + token.TokenValue + "在第" + token.Line + "行");
            }
            if (tokenReader.PeekOne().TokenType == TokenType.DoubleLable)
            {
                ReadToken();
            }
            else
            {
                throw new Exception("非法字符" + token.TokenValue + "在第" + token.Line + "行");
            }
            return lableStatNode;
        }

        private AssignStatNode NoLocalFuncDefStatPaser()
        {
            ReadToken();
            AssignStatNode assignStatNode = new AssignStatNode();
            bool isSingleLable = false;
            assignStatNode.VarList.Add(FuncNamePaser(ref isSingleLable));
            assignStatNode.ExpList.Add(FuncdefExpPaser());
            if (isSingleLable == true)
            {
                (assignStatNode.ExpList[0] as FuncdefExpNode).ParList.Insert(0, new ConstExpNode(new Token(TokenType.Self, "self", token.Line)));//给函数参数加上self标志
            }
            return assignStatNode;
        }

        private ExpNode FuncNamePaser(ref bool isSingleLable)
        {
            ReadToken();
            ExpNode exp = new ConstExpNode(token);
            ExpNode returnRef = exp;
            while (tokenReader.PeekOne().TokenType == TokenType.Dawn)
            {
                ReadToken();
                ReadToken();
                ExpNode idx = new ConstExpNode(token);
                TableAccessExpNode tableAccessExpNode = new TableAccessExpNode();
                tableAccessExpNode.Exp = idx;
                tableAccessExpNode.PreExp = exp;
                exp = tableAccessExpNode;
            }
            //如果有:a()语法糖
            if (tokenReader.PeekOne().TokenType == TokenType.SingleLable)
            {
                ReadToken();
                ReadToken();
                ExpNode idx = new ConstExpNode(token);
                TableAccessExpNode tableAccessExpNode = new TableAccessExpNode();
                tableAccessExpNode.Exp = idx;
                tableAccessExpNode.PreExp = exp;
                exp = tableAccessExpNode;
                isSingleLable = true;
            }
            return returnRef;
        }

        /// <summary>
        /// exp12 -> exp11 {or exp 11}
        /// </summary>
        /// <returns></returns>
        private ExpNode Exp12Paser()
        {
            ExpNode expNode = Exp11Paser();
            if (tokenReader.PeekOne().TokenType == TokenType.Or)
            {
                ReadToken();
                DoubleOperationExpNode orNode = new DoubleOperationExpNode();
                orNode.Exp1 = expNode;
                orNode.OpType = TokenType.Or;
                orNode.Exp2 = Exp11Paser();
                return orNode;
            }
            return expNode;
        }

        /// <summary>
        /// exp11 -> exp10{and exp10}
        /// </summary>
        /// <returns></returns>
        private ExpNode Exp11Paser()
        {
            ExpNode expNode = Exp10Paser();
            if (tokenReader.PeekOne().TokenType == TokenType.And)
            {
                ReadToken();
                DoubleOperationExpNode andNode = new DoubleOperationExpNode();
                andNode.Exp1 = expNode;
                andNode.OpType = TokenType.And;
                andNode.Exp2 = Exp10Paser();
                return andNode;
            }
            return expNode;
        }

        /// <summary>
        /// exp10 -> exp9{{< > = <= >= ~= ==} exp9}
        /// </summary>
        /// <returns></returns>
        private ExpNode Exp10Paser()
        {
            ExpNode expNode = Exp9Paser();
            if (tokenReader.PeekOne().TokenType >= TokenType.Smaller && tokenReader.PeekOne().TokenType <= TokenType.StarAssignment)
            {
                ReadToken();
                DoubleOperationExpNode node = new DoubleOperationExpNode();
                node.Exp1 = expNode;
                node.OpType = token.TokenType;
                node.Exp2 = Exp9Paser();
                return node;
            }
            return expNode;
        }

        /// <summary>
        /// exp9 -> exp8 {| exp8}
        /// </summary>
        /// <returns></returns>
        private ExpNode Exp9Paser()
        {
            ExpNode expNode = Exp8Paser();
            if (tokenReader.PeekOne().TokenType == TokenType.OrOperation)
            {
                ReadToken();
                DoubleOperationExpNode node = new DoubleOperationExpNode();
                node.Exp1 = expNode;
                node.OpType = token.TokenType;
                node.Exp2 = Exp8Paser();
                return node;
            }
            return expNode;
        }

        /// <summary>
        /// exp 8 -> exp7{~ exp7}
        /// </summary>
        /// <returns></returns>
        private ExpNode Exp8Paser()
        {
            ExpNode expNode = Exp7Paser();
            if (tokenReader.PeekOne().TokenType == TokenType.Error)
            {
                ReadToken();
                DoubleOperationExpNode node = new DoubleOperationExpNode();
                node.Exp1 = expNode;
                node.OpType = token.TokenType;
                node.Exp2 = Exp7Paser();
                return node;
            }
            return expNode;
        }

        /// <summary>
        /// exp 7 -> exp6{~exp6}
        /// </summary>
        /// <returns></returns>
        private ExpNode Exp7Paser()
        {
            ExpNode expNode = Exp6Paser();
            if (tokenReader.PeekOne().TokenType == TokenType.Error)
            {
                ReadToken();
                DoubleOperationExpNode node = new DoubleOperationExpNode();
                node.Exp1 = expNode;
                node.OpType = token.TokenType;
                node.Exp2 = Exp6Paser();
                return node;
            }
            return expNode;
        }

        /// <summary>
        /// exp6 -> exp5{..exp5}
        /// </summary>
        /// <returns></returns>
        private ExpNode Exp6Paser()
        {
            ExpNode expNode = Exp5Paser();

            if (tokenReader.PeekOne().TokenType == TokenType.Connect)
            {
                List<ExpNode> expList = new List<ExpNode>();
                ConcatExpNode node = new ConcatExpNode();
                while (tokenReader.PeekOne().TokenType == TokenType.Connect)
                {
                    ReadToken();
                    expList.Add(Exp5Paser());
                }
                node.ExpList = expList;
                return node;
            }
            return expNode;
        }

        /// <summary>
        /// exp5 -> exp4 {(+ -)exp4}
        /// </summary>
        /// <returns></returns>
        private ExpNode Exp5Paser()
        {
            ExpNode expNode = Exp4Paser();
            switch (tokenReader.PeekOne().TokenType)
            {
                case TokenType.Plus:
                case TokenType.Minus:
                    ReadToken();
                    DoubleOperationExpNode node = new DoubleOperationExpNode();
                    node.Exp1 = expNode;
                    node.OpType = token.TokenType;
                    node.Exp2 = Exp4Paser();
                    return node;
                default:
                    break;
            }
            return expNode;
        }

        /// <summary>
        /// exp4 -> exp3 {+ - exp3}
        /// </summary>
        /// <returns></returns>
        private ExpNode Exp4Paser()
        {
            ExpNode expNode = Exp3Paser();
            switch (tokenReader.PeekOne().TokenType)
            {
                case TokenType.Percent:
                case TokenType.Slash:
                case TokenType.Star:
                    ReadToken();
                    DoubleOperationExpNode node = new DoubleOperationExpNode();
                    node.Exp1 = expNode;
                    node.OpType = token.TokenType;
                    node.Exp2 = Exp4Paser();
                    return node;
                default:
                    break;
            }
            return expNode;
        }

        /// <summary>
        /// exp3 -> {- ~ not # exp2}
        /// </summary>
        /// <returns></returns>
        private ExpNode Exp3Paser()
        {
            switch (tokenReader.PeekOne().TokenType)
            {
                case TokenType.Not:
                case TokenType.Len:
                case TokenType.Minus:
                case TokenType.Error:
                    ReadToken();
                    UnaryExpNode node = new UnaryExpNode();
                    node.OpType = token.TokenType;
                    node.Exp = Exp3Paser();
                    return node;
                default:
                    break;
            }
            return Exp2Paser();
        }

        /// <summary>
        /// exp2 -> exp1 {^ exp3}
        /// </summary>
        /// <returns></returns>
        private ExpNode Exp2Paser()
        {
            ExpNode expNode = Exp1Paser();
            switch (tokenReader.PeekOne().TokenType)
            {
                case TokenType.Not:
                case TokenType.Len:
                case TokenType.Minus:
                case TokenType.Error:
                    ReadToken();
                    UnaryExpNode node = new UnaryExpNode();
                    node.OpType = token.TokenType;
                    node.Exp = Exp3Paser();
                    return node;
                default:
                    break;
            }
            return expNode;
        }

        private ExpNode Exp1Paser()
        {
            switch (tokenReader.PeekOne().TokenType)
            {
                case TokenType.Nil:
                case TokenType.True:
                case TokenType.False:
                case TokenType.Vararg:
                case TokenType.String:
                case TokenType.Number:
                    {
                        ReadToken();
                        return new ConstExpNode(token);
                    }
                case TokenType.Function:
                    {
                        //读掉function
                        ReadToken();
                        return FuncdefExpPaser();
                    }
                case TokenType.LeftBig:
                    return TableConstructorPaser();
                default:
                    return PerfixExpPaser();
            }
        }

        private FuncdefExpNode FuncdefExpPaser()
        {
            FuncdefExpNode funcdefExpNode = new FuncdefExpNode();
            //function已经被读掉
            //读掉(
            ReadToken();
            bool isVararg = false;
            funcdefExpNode.ParList = ParListPaser(ref isVararg);
            funcdefExpNode.IsVararg = isVararg;
            ReadToken();
            funcdefExpNode.Block = BlockPaser();
            if (tokenReader.PeekOne().TokenType == TokenType.End)
            {
                ReadToken();
            }
            else
            {
                throw new Exception("非法字符" + token.TokenValue + "在第" + token.Line + "行");
            }
            return funcdefExpNode;
        }

        private List<ConstExpNode> ParListPaser(ref bool isVararg)
        {
            List<ConstExpNode> ConstExpNodes = new List<ConstExpNode>();
            switch (tokenReader.PeekOne().TokenType)
            {
                //读到)，没有函数参数
                case TokenType.RightParen:
                    ConstExpNodes.Add(new ConstExpNode(new Token(TokenType.Nil, "nil", token.Line)));
                    isVararg = false;
                    return ConstExpNodes;
                //读到...表示可变参数
                case TokenType.Vararg:
                    ConstExpNodes.Add(new ConstExpNode(new Token(TokenType.Nil, "nil", token.Line)));
                    isVararg = true;
                    return ConstExpNodes;
            }
            //都不是表示为变量名
            ReadToken();
            if (token.TokenType == TokenType.Identifier)
            {
                ConstExpNodes.Add(new ConstExpNode(token));
                while (tokenReader.PeekOne().TokenType == TokenType.Comma)
                {
                    //读掉,
                    ReadToken();
                    //读掉变量名
                    ReadToken();
                    if (token.TokenType == TokenType.Identifier)
                    {
                        ConstExpNodes.Add(new ConstExpNode(token));
                    }
                    //如果为...
                    else if (token.TokenType == TokenType.Vararg)
                    {
                        isVararg = true;
                    }
                    //右括号结束解析
                    else if (token.TokenType == TokenType.LeftParen)
                    {
                        return ConstExpNodes;
                    }
                    else
                    {
                        throw new Exception("非法字符" + token.TokenValue + "在第" + token.Line + "行");
                    }
                }
                return ConstExpNodes;
            }
            else
            {
                throw new Exception("非法字符" + token.TokenValue + "在第" + token.Line + "行");
            }
        }

        private TableConstructorExpNode TableConstructorPaser()
        {
            //读掉{
            ReadToken();
            TableConstructorExpNode tableConstructorExpNode = new TableConstructorExpNode();
            FieldListPaser(tableConstructorExpNode.KeyExpList, tableConstructorExpNode.ValExpList);
            if(tableConstructorExpNode.KeyExpList == null)
            {
                tableConstructorExpNode.KeyExpList = new List<ExpNode>();
            }
            if(tableConstructorExpNode.ValExpList == null)
            {
                tableConstructorExpNode.ValExpList = new List<ExpNode>();
            }
            if (tokenReader.PeekOne().TokenType == TokenType.Comma || tokenReader.PeekOne().TokenType == TokenType.SemiColon)
            {
                //读掉最后的,和;
                ReadToken();
                //读掉最后的}
                if (tokenReader.PeekOne().TokenType == TokenType.RightBig)
                {
                    ReadToken();
                }
                else
                {
                    throw new Exception("非法字符" + token.TokenValue + "在第" + token.Line + "行");
                }
            }
            if (tokenReader.PeekOne().TokenType == TokenType.RightBig)
            {
                ReadToken();
            }
            return tableConstructorExpNode;
        }

        private void FieldListPaser(List<ExpNode> keyList, List<ExpNode> valList)
        {
            while (tokenReader.PeekOne().TokenType == TokenType.Comma || tokenReader.PeekOne().TokenType == TokenType.SemiColon || tokenReader.PeekOne().TokenType == TokenType.RightParen)
            {
                ExpNode key = null;
                ExpNode val = null;
                FieldPaser(ref key, ref val);
                if (key != null)
                {
                    keyList.Add(key);
                    valList.Add(val);
                }
                else if (key == null && val != null)
                {
                    keyList.Add(key);
                    valList.Add(val);
                }
                else
                {
                    continue;
                }
            }
        }

        //fiedl -> [exp] = exp | identifier = exp | exp
        private void FieldPaser(ref ExpNode key, ref ExpNode val)
        {
            //假如读到[
            if (tokenReader.PeekOne().TokenType == TokenType.LeftSquare)
            {
                ReadToken();
                key = ExpPaser();

                if (tokenReader.PeekOne().TokenType == TokenType.Assignment)
                {
                    ReadToken();
                    val = ExpPaser();
                    return;
                }
                else
                {
                    throw new Exception("非法字符" + token.TokenValue + "在第" + token.Line + "行");
                }
            }
            else if (tokenReader.PeekOne().TokenType == TokenType.Identifier)
            {
                ReadToken();
                key = ExpPaser();
                if (tokenReader.PeekOne().TokenType == TokenType.Assignment)
                {
                    ReadToken();
                    val = ExpPaser();
                    return;
                }
                //如果读到, ;或者)
                else if (tokenReader.PeekOne().TokenType == TokenType.Comma || tokenReader.PeekOne().TokenType == TokenType.SemiColon || tokenReader.PeekOne().TokenType == TokenType.RightParen)
                {
                    ReadToken();
                    val = key;
                    key = null;
                    return;
                }
                else
                {
                    throw new Exception("非法字符" + token.TokenValue + "在第" + token.Line + "行");
                }
            }
            else if (tokenReader.PeekOne().TokenType == TokenType.Comma || tokenReader.PeekOne().TokenType == TokenType.SemiColon || tokenReader.PeekOne().TokenType == TokenType.RightParen)
            {
                return;
            }
        }

        private ExpNode PerfixExpPaser()
        {
            ExpNode expNode = null;
            if (tokenReader.PeekOne().TokenType == TokenType.Identifier)
            {
                ReadToken();
                expNode = new ConstExpNode(token);
            }
            else if (tokenReader.PeekOne().TokenType == TokenType.LeftParen)
            {
                expNode = ParenExpPaser();
            }
            return _PerfixExpPaser(expNode);
        }

        private ExpNode _PerfixExpPaser(ExpNode expNode)
        {
            while (true)
            {
                switch (tokenReader.PeekOne().TokenType)
                {
                    //a[b]
                    case TokenType.LeftSquare:
                        {
                            ReadToken();
                            ExpNode keyExp = ExpPaser();
                            if (tokenReader.PeekOne().TokenType == TokenType.RightSquare)
                            {
                                TableAccessExpNode tableAccessExpNode = new TableAccessExpNode();
                                tableAccessExpNode.Exp = keyExp;
                                tableAccessExpNode.PreExp = expNode;
                                expNode = tableAccessExpNode;
                                ReadToken();
                            }
                            else
                            {
                                throw new Exception("非法字符" + token.TokenValue + "在第" + token.Line + "行");
                            }
                            break;
                        }
                        //a.b
                    case TokenType.Dawn:
                        {
                            ReadToken();
                            if (tokenReader.PeekOne().TokenType == TokenType.Identifier)
                            {
                                ReadToken();
                                ExpNode keyExp = new ConstExpNode(token);
                                TableAccessExpNode tableAccessExpNode = new TableAccessExpNode();
                                tableAccessExpNode.Exp = keyExp;
                                tableAccessExpNode.PreExp = expNode;
                                expNode = tableAccessExpNode;
                            }
                            else
                            {
                                throw new Exception("非法字符" + token.TokenValue + "在第" + token.Line + "行");
                            }
                            break;
                        }
                        //[:name]
                    case TokenType.SingleLable:
                    case TokenType.LeftParen:
                    case TokenType.LeftBig:
                    case TokenType.String:
                        {
                            expNode = FuncCallExpPaser(expNode);
                            break;
                        }
                    default:
                        return expNode;
                }
            }
        }

        private ExpNode ParenExpPaser()
        {
            ReadToken();
            ExpNode expNode = ExpPaser();
            if (tokenReader.PeekOne().TokenType == TokenType.RightParen)
            {
                ReadToken();
                return expNode;
            }
            else
            {
                throw new Exception("非法字符" + token.TokenValue + "在第" + token.Line + "行");
            }
        }

        private ExpNode VarOrFuncCallExpPaser(ExpNode expNode)
        {
            while (true)
            {
                switch (tokenReader.PeekOne().TokenType)
                {
                    //[exp]
                    case TokenType.LeftSquare:
                        {
                            ReadToken();
                            ExpNode keyNode = ExpPaser();
                            TableAccessExpNode tableExp = new TableAccessExpNode();
                            tableExp.PreExp = expNode;
                            tableExp.Exp = keyNode;
                            expNode = tableExp;
                            ReadToken();
                            break;
                        }
                    //a.exp -> a['exp']
                    case TokenType.Dawn:
                        {
                            ReadToken();
                            if (tokenReader.PeekOne().TokenType == TokenType.Identifier)
                            {
                                TableAccessExpNode tableExp = new TableAccessExpNode();
                                tableExp.PreExp = expNode;
                                tableExp.Exp = new ConstExpNode(token);
                                expNode = tableExp;
                            }
                            else
                            {
                                throw new Exception("非法字符" + token.TokenValue + "在第" + token.Line + "行");
                            }
                            break;
                        }
                    case TokenType.LeftBig:
                    case TokenType.LeftParen:
                    case TokenType.String:
                        expNode = FuncCallExpPaser(expNode);
                        break;
                    default:
                        return expNode;
                }
                return expNode;
            }
        }

        private FuncCallExpNode FuncCallExpPaser(ExpNode perNode)
        {
            FuncCallExpNode funcCallExpNode = new FuncCallExpNode();
            funcCallExpNode.PreExp = perNode;
            funcCallExpNode.NameExp = NamePaser();
            funcCallExpNode.Args = ArgsPaser();
            return funcCallExpNode;
        }

        private ConstExpNode NamePaser()
        {
            if (tokenReader.PeekOne().TokenType == TokenType.SingleLable)
            {
                ReadToken();
                if (tokenReader.PeekOne().TokenType == TokenType.Identifier)
                {
                    ReadToken();
                    return new ConstExpNode(token);
                }
                else
                {
                    throw new Exception("非法字符" + token.TokenValue + "在第" + token.Line + "行");
                }
            }
            else
            {
                return new ConstExpNode(new Token(TokenType.Nil, "", token.Line));
            }
        }

        private List<ExpNode> ArgsPaser()
        {
            List<ExpNode> args = null;
            switch (tokenReader.PeekOne().TokenType)
            {
                case TokenType.LeftParen:
                    ReadToken();
                    if (tokenReader.PeekOne().TokenType != TokenType.RightParen)
                    {
                        args = ExpListPaser();
                    }
                    ReadToken();
                    break;
                case TokenType.LeftBig:
                    args.Add(TableConstructorPaser());
                    break;
                case TokenType.String:
                    ReadToken();
                    args.Add(new ConstExpNode(token));
                    break;
            }
            return args;
        }

        private ExpNode ExpPaser()
        {
            return Exp12Paser();
        }

    }
}
