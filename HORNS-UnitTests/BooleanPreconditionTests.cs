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
            var v = new BoolVariable();
            var p = new BooleanPrecondition(value);
            p.SetSolver(v.Solver);
            p.Variable = v;

            var p2 = new BooleanPrecondition(value);
            p2.SetSolver(v.Solver);
            p2.Variable = v;

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
            var v = new BoolVariable();
            var p = new BooleanPrecondition(value);
            p.SetSolver(v.Solver);
            p.Variable = v;

            var p2 = new BooleanPrecondition(!value);
            p2.SetSolver(v.Solver);
            p2.Variable = v;

            var pre = p.Combine(p2);
            Assert.Null(pre);
        }
    }
}
