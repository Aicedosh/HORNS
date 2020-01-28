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
        public void GetActionsTowards_ShouldReturnMatchingAction(int value, int goalValue, string pickedActionTag)
        {
            var v = new IntegerVariable(value);

            var a1 = new BasicAction("increase");
            a1.AddResult(v, new IntegerAddResult(1));
            var a2 = new BasicAction("decrease");
            a2.AddResult(v, new IntegerAddResult(-1));
            var a3 = new BasicAction("no effect");
            var ax = new BasicAction("not added to agent");
            ax.AddResult(v, new IntegerAddResult(1));

            Agent agent = new Agent();
            agent.AddActions(a1, a2, a3);

            List<Action> actions = new List<Action>(v.Solver.GetActionsTowards(v, goalValue, agent));
            Assert.Single(actions);
            Assert.Equal(pickedActionTag, (actions[0] as BasicAction).Tag);
        }

        [Fact]
        public void GetActionsSatisfying_ShouldReturnMatchingActions()
        {
            var v = new IntegerVariable(5);

            var a1 = new BasicAction("increase");
            a1.AddResult(v, new IntegerAddResult(1));
            var a2 = new BasicAction("decrease");
            a2.AddResult(v, new IntegerAddResult(-1));
            var a3 = new BasicAction("no effect");
            var ax = new BasicAction("not added to agent");
            ax.AddResult(v, new IntegerAddResult(1));

            Agent agent = new Agent();
            agent.AddActions(a1, a2, a3);

            List<Action> actions = new List<Action>(v.Solver.GetActionsSatisfying(new IntegerPrecondition(10, false), agent));
            Assert.Single(actions);
            Assert.Equal("increase", (actions[0] as BasicAction).Tag);
        }
    }
}
