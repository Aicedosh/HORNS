using HORNS;
using Xunit;

namespace HORNS_UnitTests
{
    // TODO: tests for combining partially fulfilled precs
    public class IntegerConsumePreconditionTests
    {
        // TODO: commenting this out till we figure out what to do with precs
        //[Fact]
        //public void Combine_ShouldReturnCorrectValue()
        //{
        //    var v = new IntegerConsumeVariable();

        //    var p1 = new IntegerConsumePrecondition(3);
        //    p1.Variable = v;

        //    var p2 = new IntegerConsumePrecondition(5);
        //    p2.Variable = v;

        //    var p3 = p1.Combine(p2) as IntegerConsumePrecondition;
        //    Assert.NotNull(p3);
        //    Assert.Equal(v.Id, p3.Variable.Id);
        //    Assert.Equal(8, p3.Target);

        //    var p4 = p2.Combine(p1) as IntegerConsumePrecondition;
        //    Assert.NotNull(p4);
        //    Assert.Equal(v.Id, p4.Variable.Id);
        //    Assert.Equal(8, p4.Target);
        //}

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

        [Theory]
        [InlineData(1, true)]
        [InlineData(-1, false)]
        public void IsBetterThan_ShouldReturnCorrectValue(int term, bool improve)
        {
            var v = new IntegerConsumeVariable();

            var p1 = new IntegerConsumePrecondition(5);
            p1.Variable = v;
            p1.State = 2;
            var p2 = new IntegerConsumePrecondition(p1);

            var r = new IntegerAddResult(term);
            r.Variable = v;

            p2 = p2.Apply(r) as IntegerConsumePrecondition;

            Precondition.ComparisonResult res = improve ? Precondition.ComparisonResult.Better : Precondition.ComparisonResult.EqualWorse;
            Precondition.ComparisonResult resInv = !improve ? Precondition.ComparisonResult.Better : Precondition.ComparisonResult.EqualWorse;
            Assert.Equal(res, p2.IsBetterThan(p1));
            Assert.Equal(resInv, p1.IsBetterThan(p2));

            Assert.Equal(Precondition.ComparisonResult.EqualWorse, p1.IsBetterThan(p1));
        }
    }
}
