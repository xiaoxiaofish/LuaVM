using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaVM.VM
{
    public class UpValue
    {
        LuaValue value;

        public UpValue(LuaValue value)
        {
            this.value = value;
        }

        public LuaValue Value { get => value; set => this.value = value; }
    }
}
