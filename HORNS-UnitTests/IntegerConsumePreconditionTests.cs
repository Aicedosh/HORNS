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
            p1.SetSolver(v.Solver);
            p1.Variable = v;

            var p2 = new IntegerConsumePrecondition(5);
            p2.SetSolver(v.Solver);
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
    }
}
