using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    internal interface IActionCostEvaluator
    {
        Variable GetVariable();
        float GetCost();
    }
}
