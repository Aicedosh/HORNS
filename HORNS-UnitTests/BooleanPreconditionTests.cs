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
            Variable<bool> v = new Variable<bool>();
            BooleanSolver solver = new BooleanSolver();
            BooleanPrecondition p = new BooleanPrecondition(v, value, solver);

            BooleanPrecondition p2 = new BooleanPrecondition(v, value, solver);

            var pre = p.Combine(p2);
            Assert.NotNull(pre);
            Assert.Equal(v.Id, pre.Id);
            Assert.IsType<BooleanPrecondition>(pre);
            Assert.Equal(p.Value, (pre as BooleanPrecondition).Value);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Combine_WithDifferentValue_ShouldReturnNull(bool value)
        {
            Variable<bool> v = new Variable<bool>();
            BooleanSolver solver = new BooleanSolver();
            BooleanPrecondition p = new BooleanPrecondition(v, value, solver);

            BooleanPrecondition p2 = new BooleanPrecondition(v, !value, solver);

            var pre = p.Combine(p2);
            Assert.Null(pre);
        }
    }
}
