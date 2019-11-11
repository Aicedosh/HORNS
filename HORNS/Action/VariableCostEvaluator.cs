using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    internal class VariableCostEvaluator<T> : IActionCostEvaluator
    {
        private Variable<T> variable;
        private Func<T, float> evaluationFunction;

        public VariableCostEvaluator(Variable<T> variable, Func<T, float> evaluationFunction)
        {
            this.variable = variable;
            this.evaluationFunction = evaluationFunction;
        }

        public float GetCost()
        {
            return evaluationFunction(variable.Value);
        }

        public Variable GetVariable()
        {
            return variable;
        }
    }
}
