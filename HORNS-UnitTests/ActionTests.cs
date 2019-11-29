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
            var variable = new BoolVariable(false);
            var action = new BasicAction();
            action.AddResult(variable, new BooleanResult(true));

            Assert.False(variable.Value);
            action.Perform();
            Assert.True(variable.Value);
        }
    }
}
