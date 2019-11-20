using HORNS;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HORNS_UnitTests
{
    public class IntegerSolverTests
    {
        [Theory]
        [InlineData(5, 7, 1)]
        [InlineData(7, 2, 2)]
        [InlineData(10, 9, 2)]
        [InlineData(-3, -2, 1)]
        public void GetActionsTowards_ShouldReturnMatchingAction(int value, int goalValue, int pickedActionId)
        {
            BasicAction a1 = new BasicAction(1);
            BasicAction a2 = new BasicAction(2);

            Variable<int> v = new Variable<int>(value);
            IntegerSolver s = new IntegerSolver();

            IntegerAddResult r1 = new IntegerAddResult(v, 1);
            IntegerAddResult r2 = new IntegerAddResult(v, -1);

            r1.Action = a1;
            r2.Action = a2;
            s.Register(r1);
            s.Register(r2);

            List<Action> actions = new List<Action>(s.GetActionsTowards(v, goalValue));
            Assert.Single(actions);
            Assert.Equal(pickedActionId, (actions[0] as BasicAction).N);
        }

    }
}
