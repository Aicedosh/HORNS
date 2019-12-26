using HORNS;
using Xunit;

namespace HORNS_UnitTests
{
    public class IntegerConsumePreconditionTests
    {
        [Fact]
        public void Combine_ShouldReturnCorrectValue()
        {
            var v = new IntegerConsumeVariable();

            var p1 = new IntegerConsumePrecondition(3);
            p1.Variable = v;

            var p2 = new IntegerConsumePrecondition(5);
            p2.Variable = v;

            var p3 = p1.Combine(p2) as IntegerConsumePrecondition;
            Assert.NotNull(p3);
            Assert.Equal(v.Id, p3.Variable.Id);
            Assert.Equal(8, p3.Target);

            var p4 = p2.Combine(p1) as IntegerConsumePrecondition;
            Assert.NotNull(p4);
            Assert.Equal(v.Id, p4.Variable.Id);
            Assert.Equal(8, p4.Target);
        }

        [Fact]
        public void IsFulfilled_ShouldBeCorrectlyFulfilled()
        {
            var v = new IntegerConsumeVariable();
            var r = new IntegerAddResult(1);
            r.Variable = v;
            var p = new IntegerConsumePrecondition(2);
            p.Variable = v;
            Assert.False(p.IsFulfilled());

            p = p.Apply(r) as IntegerConsumePrecondition;
            Assert.False(p.IsFulfilled());

            p = p.Apply(r) as IntegerConsumePrecondition;
            Assert.True(p.IsFulfilled());
        }

        [Fact]
        public void IsFulfilledByWorld_ShouldBeCorrectlyFulfilledByVariableValue()
        {
            var v = new IntegerConsumeVariable();
            var p = new IntegerConsumePrecondition(3);
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
            var v = new IntegerConsumeVariable();
            var r = new IntegerAddResult(1);
            r.Variable = v;
            var p = new IntegerConsumePrecondition(3);
            p.Variable = v;
            Assert.False(p.IsFulfilledByWorld());

            p = p.Apply(r) as IntegerConsumePrecondition;
            Assert.False(p.IsFulfilled());

            v.Value = 2;
            Assert.True(p.IsFulfilledByWorld());
        }
    }
}
