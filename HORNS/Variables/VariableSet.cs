using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    internal class VariableSet
    {
        Dictionary<int, Variable> variables = new Dictionary<int, Variable>();

        private class VariableComparer : IEqualityComparer<Variable>
        {
            public bool Equals(Variable x, Variable y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(Variable var)
            {
                return var.Id.GetHashCode();
            }
        }

        internal void Add(Variable variable)
        {
            variables.Add(variable.Id, variable);
        }

        internal void TryGet(ref Variable variable)
        {
            if (variables.TryGetValue(variable.Id, out Variable var))
            {
                variable = var;
            }
        }
    }
}
