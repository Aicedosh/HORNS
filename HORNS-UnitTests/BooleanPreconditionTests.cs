using HORNS;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace HORNS_UnitTests
{
    public class BooleanPreconditionTests
    {
        [Fact]
        public void Combine_WithEqualValue_True_ShouldReturnSamePrecondition()
        {
            CombineWihEqual(true);
        }

        [Fact]
        public void Combine_WithEqualValue_False_ShouldReturnSamePrecondition()
        {
            CombineWihEqual(false);
        }

        [Fact]
        public void Combine_WithDifferentValue_True_ShouldReturnNull()
        {
            CombineWihDIfferent(true);
        }

        [Fact]
        public void Combine_WithDifferentValue_False_ShouldReturnNull()
        {
            CombineWihDIfferent(false);
        }

        private void CombineWihEqual(bool value)
        {
            Variable<bool> v = new Variable<bool>();
            BooleanSolver solver = new BooleanSolver();
            BooleanPrecondition p = new BooleanPrecondition(v, value, solver);

            BooleanPrecondition p2 = new BooleanPrecondition(v, value, solver);

            var req = p.GetRequirement().Combine(p2.GetRequirement());
            Assert.NotNull(req);
            Assert.Equal(v.Id, req.Id);
            Assert.IsType<BooleanPrecondition.PreconditionRequirement>(req);
            Assert.Equal(p.Value, ((req as BooleanPrecondition.PreconditionRequirement).precondition as BooleanPrecondition).Value);
        }

        private void CombineWihDIfferent(bool value)
        {
            Variable<bool> v = new Variable<bool>();
            BooleanSolver solver = new BooleanSolver();
            BooleanPrecondition p = new BooleanPrecondition(v, value, solver);

            BooleanPrecondition p2 = new BooleanPrecondition(v, !value, solver);

            var req = p.GetRequirement().Combine(p2.GetRequirement());
            Assert.Null(req);
        }
    }
}
