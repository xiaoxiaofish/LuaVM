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
        Func<LuaState,int> _CSharpFunc;
        UpValue[] upValues;
        public Closure(Prototype prototype)
        {
            this.prototype = prototype;
            int nUpval = prototype.UpValues.Length;
            if(nUpval > 0)
            {
                UpValues = new UpValue[nUpval];
                
            }
        }

        public Closure(Func<LuaState,int> CSharpFunc, int nUpval)
        {
            _CSharpFunc = CSharpFunc;
            if (nUpval > 0)
            {
                UpValues = new UpValue[nUpval];
            }
        }

        public Prototype Prototype { get => prototype; set => prototype = value; }
        public Func<LuaState, int> CSharpFunc { get => _CSharpFunc; set => _CSharpFunc = value; }
        public UpValue[] UpValues { get => upValues; set => upValues = value; }
    }
}
