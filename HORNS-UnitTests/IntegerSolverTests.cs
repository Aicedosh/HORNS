using HORNS;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HORNS_UnitTests
{
    public class IntegerSolverTests
    {
        [Theory]
        [InlineData(5, 7, "increase")]
        [InlineData(7, 2, "decrease")]
        [InlineData(10, 9, "decrease")]
        [InlineData(-3, -2, "increase")]
        public void GetActionsTowards_ShouldReturnMatchingAction(int value, int goalValue, string pickedActionTag)
        {
            BasicAction a1 = new BasicAction("increase");
            BasicAction a2 = new BasicAction("decrease");

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
            Assert.Equal(pickedActionTag, (actions[0] as BasicAction).Tag);
        }

    }
}
