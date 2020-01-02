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
            Assert.Equal(improve, p2.IsBetterThan(p1));

            Assert.False(p1.IsBetterThan(p1));
        }
    }
}
