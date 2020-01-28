using HORNS;
using Xunit;

namespace HORNS_UnitTests
{
    public class BooleanResultTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanBeReused(bool snapshot)
        {
            BooleanResult res = new BooleanResult(true);

            var v1 = new BooleanVariable(false);
            var v2 = new BooleanVariable(false);
            var v3 = new BooleanVariable(false);

            var a1 = new BasicAction("1");
            a1.AddResult(v1, res);

            var a2 = new BasicAction("2");
            a2.AddPrecondition(v1, new BooleanPrecondition(true));
            a2.AddResult(v2, res);

            var a3 = new BasicAction("3");
            a3.AddPrecondition(v2, new BooleanPrecondition(true));
            a3.AddResult(v3, res);

            Need<bool> n = new Need<bool>(v3, true, v => v ? 100 : 0);
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
