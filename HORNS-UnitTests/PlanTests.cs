using Xunit;
using HORNS;

namespace HORNS_UnitTests
{
    public class PlanTests
    {
        [Fact]
        public void Plan_OneAction_NoPreconditions()
        {
            var agent = new Agent();

            var solver = new BooleanSolver();
            var variable = new Variable<bool>() { Value = false };

            var action = new BasicAction();
            action.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(
                new BooleanResult(variable, true), solver);
            action.AddCost(1);
            agent.AddAction(action);

            var need = new BooleanNeed(variable, true, solver);
            agent.AddNeed(need);

            var nextAction = agent.GetNextAction();
            Assert.IsType<BasicAction>(nextAction);

            nextAction.Perform();
            Assert.True(variable.Value);
            Assert.True(need.IsSatisfied());
        }
    }
}
