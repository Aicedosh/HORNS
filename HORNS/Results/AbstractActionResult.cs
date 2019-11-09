using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class ActionResult
    {
        internal Action Action { get; private protected set; }
        internal abstract Variable AbstractVariable { get; }
        public abstract void Apply();
        public abstract double GetCost();
    }
}
