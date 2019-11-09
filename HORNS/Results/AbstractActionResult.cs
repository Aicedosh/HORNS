using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public abstract class ActionResult
    {
        internal Action action { get; set; }
        public abstract void Apply();
        public abstract double GetCost();
        internal abstract Variable GetVariable();
    }
}
