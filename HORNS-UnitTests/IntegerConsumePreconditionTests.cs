using HORNS;
using Xunit;

namespace HORNS_UnitTests
{
    public class IntegerConsumePreconditionTests
    {
        [Theory]
        [InlineData(3, 5, 8)]
        public void Combine_ShouldReturnCorrectValue(int value1, int value2, int expected)
        {
            var v = new IntegerConsumeVariable();

            var p1 = new IntegerConsumePrecondition(value1);
            p1.SetSolver(v.Solver);
            p1.Variable = v;

            var p2 = new IntegerConsumePrecondition(value2);
            p2.SetSolver(v.Solver);
            p2.Variable = v;

            var p3 = p1.Combine(p2) as IntegerConsumePrecondition;
            Assert.NotNull(p3);
            Assert.Equal(v.Id, p3.Variable.Id);
            Assert.Equal(expected, p3.Value);

            var p4 = p2.Combine(p1) as IntegerConsumePrecondition;
            Assert.NotNull(p4);
            Assert.Equal(v.Id, p4.Variable.Id);
            Assert.Equal(expected, p4.Value);
        }
    }
}
