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
        FunctionCall,
        Args,
        FunctionDef,
        FuncBody,
        ParList,
        TableConstructor,
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
        DoubleOperationExp,
        UnaryExp,
        None,

        Terminator,
        Nil,
        Number,
        String,
        Boolean,
        Keyword,
    }

    [StructLayout(LayoutKind.Explicit, Size = 8)]
    struct TokenValue
    {
        [FieldOffset(0)]
        public string str;

        [FieldOffset(0)]
        public double number;
    }
    public class GrammarNode
    {
        protected GrammarNodeType type;
        protected List<GrammarNode> childList;
        protected int line;
        public GrammarNodeType Type { get => type; }
        public GrammarNode()
        {
            childList = new List<GrammarNode>();
            childList.Capacity = 10;
            childList.ForEach(t => t = null);
        }

        public GrammarNode(GrammarNodeType type)
        {
            this.type = type;
            childList = new List<GrammarNode>();
            childList.Capacity = 10;
            childList.ForEach(t => t = null);
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

    public class TerminatorNode : GrammarNode
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
            statsNode = new List<StatNode>();
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
    }

    public class ReturnStatNode : StatNode
    {
        public ReturnStatNode() : base()
        {
            type = GrammarNodeType.ReturnStat;
            ExpList = new List<ExpNode>();
        }

        private List<ExpNode> expList;
        public List<ExpNode> ExpList { get => expList; set => expList = value; }
    }

    public class UnaryExpression : GrammarNode
    {
        public UnaryExpression() : base()
        {
            type = GrammarNodeType.ReturnStat;
        }
    }

    public class IfStatNode : StatNode
    {
        public IfStatNode() : base()
        {
            type = GrammarNodeType.IfStat;
            elseIfStatNodes = new List<ElseIfStatNode>();
        }

        private List<ElseIfStatNode> elseIfStatNodes;
        private ExpNode exp;
        private BlockNode block;
        private ElseStatNode elseStat;

        public List<ElseIfStatNode> ElseIfStatNodes { get => elseIfStatNodes; set => elseIfStatNodes = value; }
        public ExpNode Exp { get => exp; set => exp = value; }
        public BlockNode Block { get => block; set => block = value; }
        public ElseStatNode ElseStat { get => elseStat; set => elseStat = value; }
    }

    public class ElseIfStatNode : StatNode
    {
        public ElseIfStatNode() : base()
        {
            type = GrammarNodeType.ElseIfStat;
        }
        private ExpNode exp;
        private BlockNode block;

        public ExpNode Exp { get => exp; set => exp = value; }
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

        private ExpNode varName;
        private ExpNode initExp;
        private ExpNode limitExp;
        private ExpNode stepExp;
        private DoStatNode doBlock;

        public ExpNode VarName { get => varName; set => varName = value; }
        public ExpNode InitExp { get => initExp; set => initExp = value; }
        public ExpNode LimitExp { get => limitExp; set => limitExp = value; }
        public ExpNode StepExp { get => stepExp; set => stepExp = value; }
        public DoStatNode DoBlock { get => doBlock; set => doBlock = value; }
    }

    public class NameListNode : GrammarNode
    {
        public NameListNode() :base()
        {
            type = GrammarNodeType.NameList;
            NameList = new List<TerminatorNode>();
        }

        List<TerminatorNode> nameList;

        public List<TerminatorNode> NameList { get => nameList; set => nameList = value; }
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



    public class ExpNode : GrammarNode
    {

    }

    public class DoubleOperationExpNode : GrammarNode
    {
     
    }

    public class UnaryExpNode : GrammarNode
    {
       
    }

    public class PrefixExpNode : GrammarNode
    {
       
    }

    //变量语法节点
    public class VariableNode : GrammarNode
    {

        public enum VariableNodeType
        {
            Identifier,//单独标识符类型比如，local a = 0
            Index,//[]类型，比如 a = array[4]
            Prefix,//前缀类型，比如 a.b = 0

        }
        public VariableNode() : base()
        {
            type = GrammarNodeType.Var;
        }
        private VariableNodeType variableNodeType;


        //变量名或者前缀表达式
        public GrammarNode Identifier
        {
            get
            {
                return childList[0];
            }

            set
            {
                childList[0] = value;
            }
        }

        //左方括号
        public GrammarNode LeftSquare
        {
            get
            {
                return childList[1];
            }

            set
            {
                childList[1] = value;
            }
        }

        //索引
        public GrammarNode Index
        {
            get
            {
                return childList[2];
            }

            set
            {
                childList[2] = value;
            }
        }

        //右方括号
        public GrammarNode RightSquare
        {
            get
            {
                return childList[2];
            }

            set
            {
                childList[2] = value;
            }
        }

        public GrammarNode Comma
        {
            get
            {
                return childList[1];
            }

            set
            {
                childList[1] = value;
            }
        }
        /// <summary>
        /// .后的最终变量名
        /// </summary>
        public GrammarNode FinalIdentifier
        {
            get
            {
                return childList[2];
            }

            set
            {
                childList[2] = value;
            }
        }

        public VariableNodeType VariableNodeType1 { get => variableNodeType; set => variableNodeType = value; }
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
            }
            return null;
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
            return new StatNode();
        }

        private DoStatNode DoStatPaser()
        {
            ReadToken();
            DoStatNode doStatNode = new DoStatNode();
            doStatNode.Block = BlockPaser();
            ReadToken();
            if (token.TokenType == TokenType.End)
            {
            }
            else
            {
                throw new Exception("没有与do匹配的end在" + token.Line + "行");
            }
            return doStatNode;
        }

        private WhileStatNode WhileStatPaser()
        {
            ReadToken();
            WhileStatNode whileStatNode = new WhileStatNode();
            whileStatNode.Exp = ExpPaser();
            ReadToken();
            if (token.TokenType == TokenType.Do)
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

            TerminatorNode ifKeyWordNode = new TerminatorNode(token);
            ifStatNode.Exp = ExpPaser();
            ReadToken();
            if (token.TokenType == TokenType.Then)
            {
                ifStatNode.Block = BlockPaser();
                while (tokenReader.PeekOne().TokenType == TokenType.Elseif)
                {
                    ifStatNode.ElseIfStatNodes.Add(ElseIfStatPaser());
                }
                if (tokenReader.PeekOne().TokenType == TokenType.Else)
                {
                    ifStatNode.ElseStat = ElseStatPaser();
                }

                ReadToken();
                if (token.TokenType == TokenType.End)
                {
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
            return null;
        }

        private ElseIfStatNode ElseIfStatPaser()
        {
            ElseIfStatNode elseIfStatNode = new ElseIfStatNode();
            ReadToken();
            elseIfStatNode.Exp = ExpPaser();
            if (token.TokenType == TokenType.Then)
            {
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
            while (tokenReader.IsBlockEnd(tokenReader.PeekOne()))
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
                return null;
            Token token = tokenReader.ReadToken();
            switch (token.TokenType)
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
                return null;
            //是return 解析return表达式
            //读取该return
            Token token = tokenReader.ReadToken();
            //前瞻一个token
            Token peekToken = tokenReader.PeekOne();
            //如果是块结束，说明返回语句为空，直接返回null
            if (tokenReader.IsBlockEnd(peekToken))
            {
                return null;
            }
            //如果是分号同样返回空
            if (peekToken.TokenType == TokenType.SemiColon)
            {
                //跳过这个分号
                tokenReader.ReadToken();
                return null;
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
            if(token.TokenType == TokenType.Until)
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


        /// <summary>
        /// 普通型for循环
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        private ForNumStatNode ForNumStatPaser(ExpNode varName)
        {
            ForNumStatNode forNumStatNode = new ForNumStatNode();
            forNumStatNode.VarName = varName;
            //读取掉=
            ReadToken();
            forNumStatNode.InitExp = ExpPaser();
            //读取掉,
            ReadToken();
            if(token.TokenType == TokenType.Comma)
            {
                forNumStatNode.LimitExp = ExpPaser();
                //如果有冒号，就先读掉
                if(tokenReader.PeekOne().TokenType == TokenType.Comma)
                {
                    ReadToken();
                    forNumStatNode.StepExp = ExpPaser();
                }
                else
                {
                    //没有冒号直接解析
                    forNumStatNode.StepExp = ExpPaser();
                }
                if(tokenReader.PeekOne().TokenType == TokenType.Do)
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
            if(tokenReader.PeekOne().TokenType == TokenType.In)
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
            nameListNode.NameList.Add(new TerminatorNode(nameToken));
            while(tokenReader.PeekOne().TokenType == TokenType.Comma)
            {
                ReadToken();
                nameListNode.NameList.Add(new TerminatorNode(token));
            }
            return nameListNode;
        }

        private StatNode LablePaser()
        {
            return null;
        }

        private GrammarNode FuncNamePaser()
        {
            return null;
        }

        public GrammarNode PrefixExpPaser()
        {

            return null;
        }

        public GrammarNode VariablePaser()
        {
            return null;
        }

        public ExpNode ExpPaser()
        {

            return null;
        }

        public GrammarNode DoubleOperationExpPaser(ExpNode leftNode)
        {
            return null;
        }
    }
}
