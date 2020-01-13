using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    internal class ConstantCostEvaluator : IActionCostEvaluator
    {
        private float cost;

        public ConstantCostEvaluator(float cost)
        {
            this.cost = cost;
        }

        public float GetCost()
        {
            return cost;
        }

        public Variable GetVariable()
        {
            return null;
        }
    }
}
