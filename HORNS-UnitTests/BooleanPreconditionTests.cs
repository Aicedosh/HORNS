using HORNS;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace HORNS_UnitTests
{
    public class BooleanPreconditionTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Combine_WithEqualValue_ShouldReturnSamePrecondition(bool value)
        {
            var v = new BooleanVariable();
            var p = new BooleanPrecondition(value);
            p.Variable = v;

            var p2 = new BooleanPrecondition(value);
            p2.Variable = v;

            var pre = p.Combine(p2);
            Assert.NotNull(pre);
            Assert.Equal(v.Id, pre.Id);
            Assert.IsType<BooleanPrecondition>(pre);
            Assert.Equal(p.Target, (pre as BooleanPrecondition).Target);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Combine_WithDifferentValue_ShouldReturnNull(bool value)
        {
            var v = new BooleanVariable();
            var p = new BooleanPrecondition(value);
            p.Variable = v;

            var p2 = new BooleanPrecondition(!value);
            p2.Variable = v;

            var pre = p.Combine(p2);
            Assert.Null(pre);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsFulfilled_ShouldBeCorrectlyFulfilled(bool value)
        {
            var v = new BooleanVariable(!value);
            var r = new BooleanResult(value);
            r.Variable = v;
            var p = new BooleanPrecondition(value);
            p.Variable = v;
            Assert.False(p.IsFulfilled());

            p = p.Apply(r) as BooleanPrecondition;
            Assert.True(p.IsFulfilled());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsFulfilledByWorld_ShouldBeCorrectlyFulfilledByVariableValue(bool value)
        {
            var v = new BooleanVariable(!value);
            var p = new BooleanPrecondition(value);
            p.Variable = v;
            Assert.False(p.IsFulfilledByWorld());

            v.Value = value;
            Assert.True(p.IsFulfilledByWorld());
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void IsBetterThan_ShouldReturnCorrectValue(bool value, bool improve)
        {
            var v = new BooleanVariable(!value);

            var p1 = new BooleanPrecondition(value);
            p1.Variable = v;
            var p2 = new BooleanPrecondition(p1);

            var r = new BooleanResult(improve ? value : !value);
            r.Variable = v;

            p2 = p2.Apply(r) as BooleanPrecondition;
            Precondition.ComparisonResult res = improve ? Precondition.ComparisonResult.Better : Precondition.ComparisonResult.EqualWorse;
            Assert.Equal(res, p2.IsBetterThan(p1));

            Assert.Equal(Precondition.ComparisonResult.EqualWorse, p1.IsBetterThan(p1));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanBeReused(bool snapshot)
        {
            BooleanPrecondition pre = new BooleanPrecondition(true);

            var v1 = new BooleanVariable(false);
            var v2 = new BooleanVariable(false);
            var v3 = new BooleanVariable(false);

            var a1 = new BasicAction("1");
            a1.AddResult(v1, new BooleanResult(true));

            var a2 = new BasicAction("2");
            a2.AddPrecondition(v1, pre);
            a2.AddResult(v2, new BooleanResult(true));

            var a3 = new BasicAction("3");
            a3.AddPrecondition(v2, pre);
            a3.AddResult(v3, new BooleanResult(true));

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
