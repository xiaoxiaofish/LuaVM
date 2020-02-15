using LuaVM.Codegen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.VM.LuaAndCSharp
{
    public class LuaAndCSharpClosure
    {
        Func<LuaToCSharpState, int> _CSharpFunc;
        Prototype prototype;

        public LuaAndCSharpClosure(Func<LuaToCSharpState, int> cSharpFunc)
        {
            _CSharpFunc = cSharpFunc;
        }

        public LuaAndCSharpClosure(Prototype prototype)
        {
            this.prototype = prototype;
        }

        public Func<LuaToCSharpState, int> CSharpFunc { get => _CSharpFunc; }
        public Prototype LuaClosure { get => prototype;}

        public bool IsLuaClosure
        {
            get
            {
                return prototype != null;
            }
        }

        public bool IsCSharpClosure
        {
            get
            {
                return prototype == null;
            }
        }
    }
}
