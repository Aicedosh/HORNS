using HORNS;
using Xunit;

namespace HORNS_UnitTests
{
    public class IntegerPreconditionTests
    {
        [Fact]
        public void Combine_ShouldReturnCorrectValue()
        {
            var v = new IntegerVariable();

            var p1 = new IntegerPrecondition(3, false);
            p1.Variable = v;

            var p2 = new IntegerPrecondition(5, true);
            p2.Variable = v;

            var p3 = new IntegerPrecondition(5, false);
            p3.Variable = v;

            var p4 = p1.Combine(p2) as IntegerPrecondition;
            Assert.NotNull(p4);
            Assert.Equal(v.Id, p4.Variable.Id);
            Assert.Equal(3, p4.Target);

            var p5 = p2.Combine(p1) as IntegerPrecondition;
            Assert.NotNull(p5);
            Assert.Equal(v.Id, p5.Variable.Id);
            Assert.Equal(5, p5.Target);

            var p6 = p1.Combine(p3) as IntegerPrecondition;
            Assert.NotNull(p6);
            Assert.Equal(v.Id, p6.Variable.Id);
            Assert.Equal(5, p6.Target);

            var p7 = p3.Combine(p1) as IntegerPrecondition;
            Assert.NotNull(p7);
            Assert.Equal(v.Id, p7.Variable.Id);
            Assert.Equal(5, p7.Target);
        }

        [Fact]
        public void IsFulfilled_ShouldBeCorrectlyFulfilled()
        {
            var v = new IntegerVariable();
            var r = new IntegerAddResult(1);
            r.Variable = v;
            var p = new IntegerPrecondition(2, false);
            p.Variable = v;
            Assert.False(p.IsFulfilled());

            p = p.Apply(r) as IntegerPrecondition;
            Assert.False(p.IsFulfilled());

            p = p.Apply(r) as IntegerPrecondition;
            Assert.True(p.IsFulfilled());
        }

        [Fact]
        public void IsFulfilledByWorld_ShouldBeCorrectlyFulfilledByVariableValue()
        {
            var v = new IntegerVariable();
            var p = new IntegerPrecondition(3, false);
            p.Variable = v;
            Assert.False(p.IsFulfilledByWorld());

            v.Value = 3;
            Assert.True(p.IsFulfilledByWorld());

            v.Value = 2;
            Assert.False(p.IsFulfilledByWorld());

            v.Value = 4;
            Assert.True(p.IsFulfilledByWorld());
        }

        [Fact]
        public void IsFulfilledByWorld_PartiallyFulfilled_ShouldBeCorrectlyFulfilledByVariableValue()
        {
            var v = new IntegerVariable();
            var r = new IntegerAddResult(1);
            r.Variable = v;
            var p = new IntegerPrecondition(3, false);
            p.Variable = v;
            Assert.False(p.IsFulfilledByWorld());

            p = p.Apply(r) as IntegerPrecondition;
            Assert.False(p.IsFulfilled());

            v.Value = 2;
            Assert.True(p.IsFulfilledByWorld());
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(-1, false)]
        public void IsBetterThan_ShouldReturnCorrectValue(int term, bool improve)
        {
            var v = new IntegerVariable();

            var p1 = new IntegerPrecondition(5, false);
            p1.Variable = v;
            p1.State = 2;
            var p2 = new IntegerPrecondition(p1);

            var r = new IntegerAddResult(term);
            r.Variable = v;

            p2 = p2.Apply(r) as IntegerPrecondition;

            Precondition.ComparisonResult res = improve ? Precondition.ComparisonResult.Better : Precondition.ComparisonResult.EqualWorse;
            Precondition.ComparisonResult resInv = !improve ? Precondition.ComparisonResult.Better : Precondition.ComparisonResult.EqualWorse;
            Assert.Equal(res, p2.IsBetterThan(p1));
            Assert.Equal(resInv, p1.IsBetterThan(p2));

            Assert.Equal(Precondition.ComparisonResult.EqualWorse, p1.IsBetterThan(p1));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanBeReused(bool snapshot)
        {
            IntegerPrecondition pre = new IntegerPrecondition(1, false);

            var v1 = new IntegerVariable(0);
            var v2 = new IntegerVariable(0);
            var v3 = new IntegerVariable(0);

            var a1 = new BasicAction("1");
            a1.AddResult(v1, new IntegerAddResult(1));

            var a2 = new BasicAction("2");
            a2.AddPrecondition(v1, pre);
            a2.AddResult(v2, new IntegerAddResult(1));

            var a3 = new BasicAction("3");
            a3.AddPrecondition(v2, pre);
            a3.AddResult(v3, new IntegerAddResult(1));

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
