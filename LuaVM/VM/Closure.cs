using LuaVM.Codegen;
using LuaVM.VM.LuaAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.VM
{
    public class Closure
    {
        Prototype prototype;
        Func<LuaState, int> _CSharpFunc;
        public Closure(Prototype prototype)
        {
            this.prototype = prototype;
        }

        public Closure(Func<LuaState, int> CSharpFunc)
        {
            _CSharpFunc = CSharpFunc;
        }

        public Prototype Prototype { get => prototype; set => prototype = value; }
        public Func<LuaState, int> CSharpFunc { get => _CSharpFunc; set => _CSharpFunc = value; }
    }
}
