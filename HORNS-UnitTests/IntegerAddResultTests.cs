using HORNS;
using Xunit;

namespace HORNS_UnitTests
{
    public class IntegerAddResultTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanBeReused(bool snapshot)
        {
            IntegerAddResult res = new IntegerAddResult(1);

            var v1 = new IntegerVariable(0);
            var v2 = new IntegerVariable(0);
            var v3 = new IntegerVariable(0);

            var a1 = new BasicAction("1");
            a1.AddResult(v1, res);

            var a2 = new BasicAction("2");
            a2.AddPrecondition(v1, new IntegerPrecondition(1, false));
            a2.AddResult(v2, res);

            var a3 = new BasicAction("3");
            a3.AddPrecondition(v2, new IntegerPrecondition(1, false));
            a3.AddResult(v3, res);

            Need<int> n = new Need<int>(v3, 1, v => v);
            Agent a = new Agent();
            a.AddNeed(n);
            a.AddActions(a1, a2, a3);

            var planner = new ActionPlanner();
            (var actions, var need) = planner.Plan(a, snapshot);

            Assert.Equal(3, actions.Count);
            Assert.Equal("1", (actions[0] as BasicAction).Tag);
            Assert.Equal("2", (actions[1] as BasicAction).Tag);
            Assert.Equal("3", (actions[2] as BasicAction).Tag);
        }
    }
}
