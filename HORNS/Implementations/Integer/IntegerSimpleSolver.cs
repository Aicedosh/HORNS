using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public class IntegerSimpleSolver : VariableSolver<int, IntegerAddResult, IntegerSimplePrecondition>
    {
        protected override IEnumerable<Action> GetActionsSatisfying(IntegerSimplePrecondition precondition)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<Action> GetActionsTowards(Variable<int> variable, int desiredValue)
        {
            throw new NotImplementedException();
        }

        protected internal override void Register(IntegerAddResult result)
        {
            throw new NotImplementedException();
        }
    }
}
