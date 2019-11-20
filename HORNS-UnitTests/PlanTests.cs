using Xunit;
using HORNS;
using System.Linq;

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

        [Fact]
        public void Plan_ThreeStepResult_BooleanVariables()
        {
            var agent = new Agent();

            var solver1 = new BooleanSolver();
            var solver2 = new BooleanSolver();
            var solver3 = new BooleanSolver();
            var v1 = new Variable<bool>() { Value = false };
            var v2 = new Variable<bool>() { Value = false };
            var v3 = new Variable<bool>() { Value = false };

            var need = new BooleanNeed(v3, true, solver3);

            var a1 = new BasicAction(1);
            a1.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(v1, true), solver1);

            var a2 = new BasicAction(2);
            a2.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanPrecondition(v1, true, solver1));
            a2.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(v2, true), solver2);

            var a3 = new BasicAction(3);
            a3.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanPrecondition(v2, true, solver2));
            a3.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(v3, true), solver3);

            agent.AddNeed(need);
            agent.AddAction(a1);
            agent.AddAction(a2);
            agent.AddAction(a3);

            ActionPlanner planner = new ActionPlanner();
            var actions = planner.Plan(agent, Enumerable.Empty<HORNS.Action>());

            Assert.Equal(3, actions.Count);
            Assert.Equal(1, (actions[0] as BasicAction).N);
            Assert.Equal(2, (actions[1] as BasicAction).N);
            Assert.Equal(3, (actions[2] as BasicAction).N);
        }

        [Fact]
        public void Plan_SingleIntPrecondition_RequiredMultipleTimesOnPath()
        {
            const int REQUIRED_NUMBER = 5;
            var agent = new Agent();

            var s1 = new IntegerSolver();
            var s2 = new BooleanSolver();

            var v1 = new Variable<int>() { Value = 0 };
            var v2 = new Variable<bool>() { Value = false };

            var need = new BooleanNeed(v2, true, s2);

            var a1 = new BasicAction(1);
            a1.AddPrecondition<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(new IntegerPrecondition(v1, REQUIRED_NUMBER, IntegerPrecondition.Condition.AtLeast, s1));
            a1.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(v2, true), s2);

            var a2 = new BasicAction(2);
            a2.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(new IntegerAddResult(v1, 1), s1);

            agent.AddAction(a1);
            agent.AddAction(a2);
            agent.AddNeed(need);

            ActionPlanner planner = new ActionPlanner();
            var actions = planner.Plan(agent, Enumerable.Empty<HORNS.Action>());

            Assert.Equal(REQUIRED_NUMBER + 1, actions.Count);
            Assert.Equal(1, (actions[REQUIRED_NUMBER] as BasicAction).N);
            for (int i = 0; i < REQUIRED_NUMBER - 1; i++)
            {
                Assert.Equal(2, (actions[i] as BasicAction).N);
            }
        }
    }
}
