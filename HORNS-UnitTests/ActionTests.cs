using Xunit;
using HORNS;

namespace HORNS_UnitTests
{
    public class ActionTests
    {
        [Fact]
        public void Perform_ExecuteActionResult()
        {
            var action = new ChangeStateAction();

            Assert.False(action.State.Value);
            action.Perform();
            Assert.True(action.State.Value);
        }

        [Fact]
        public void Perform_ApplyResults()
        {
            var solver = new BooleanSolver();
            var variable = new Variable<bool>() { Value = false };
            var action = new BasicAction();
            action.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(variable, true), solver);

            Assert.False(variable.Value);
            action.Perform();
            Assert.True(variable.Value);
        }
    }
}
