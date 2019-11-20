using Xunit;
using HORNS;
using System.Linq;
using System.Collections.Generic;

namespace HORNS_UnitTests
{
    public class PlanTests
    {
        [Fact]
        public void Plan_OneAction_NoPreconditions()
        {
            var agent = new Agent();
            var solver = new BooleanSolver();
            var variable = new Variable<bool>();

            var action = new BasicAction();
            action.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(
                new BooleanResult(variable, true), solver);
            action.AddCost(1);

            var need = new BooleanNeed(variable, true, solver);

            agent.AddAction(action);
            agent.AddNeed(need);

            var actions = Plan(agent);

            Assert.Single(actions);

            actions[0].Perform();
            Assert.True(variable.Value);
            Assert.True(need.IsSatisfied());
        }

        [Fact]
        public void Plan_ThreeStepResult_BooleanVariables()
        {
            var agent = new Agent();

            var s1 = new BooleanSolver();
            var s2 = new BooleanSolver();
            var s3 = new BooleanSolver();
            var v1 = new Variable<bool>();
            var v2 = new Variable<bool>();
            var v3 = new Variable<bool>();

            var need = new BooleanNeed(v3, true, s3);

            var a1 = new BasicAction("1");
            a1.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(v1, true), s1);

            var a2 = new BasicAction("2");
            a2.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanPrecondition(v1, true, s1));
            a2.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(v2, true), s2);

            var a3 = new BasicAction("3");
            a3.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanPrecondition(v2, true, s2));
            a3.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(v3, true), s3);

            agent.AddNeed(need);
            agent.AddActions(a1, a2, a3);

            var actions = Plan(agent);

            Assert.Equal(3, actions.Count);
            Assert.Equal("1", (actions[0] as BasicAction).Tag);
            Assert.Equal("2", (actions[1] as BasicAction).Tag);
            Assert.Equal("3", (actions[2] as BasicAction).Tag);
        }

        [Fact]
        public void Plan_SingleIntPrecondition_RequiredMultipleTimesOnPath()
        {
            const int REQUIRED_NUMBER = 5;
            var agent = new Agent();

            var s1 = new IntegerSolver();
            var s2 = new BooleanSolver();

            var v1 = new Variable<int>();
            var v2 = new Variable<bool>();

            var need = new BooleanNeed(v2, true, s2);

            var a1 = new BasicAction("Last");
            a1.AddPrecondition<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(new IntegerPrecondition(v1, REQUIRED_NUMBER, IntegerPrecondition.Condition.AtLeast, s1));
            a1.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(v2, true), s2);

            var a2 = new BasicAction("Add one");
            a2.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(new IntegerAddResult(v1, 1), s1);

            agent.AddActions(a1, a2);
            agent.AddNeed(need);

            var actions = Plan(agent);

            Assert.Equal(REQUIRED_NUMBER + 1, actions.Count);
            Assert.Equal("Last", (actions[REQUIRED_NUMBER] as BasicAction).Tag);
            for (int i = 0; i < REQUIRED_NUMBER - 1; i++)
            {
                Assert.Equal("Add one", (actions[i] as BasicAction).Tag);
            }
        }

        [Fact]
        public void Plan_TwoActionsBooleanResult_PicksBetterAction()
        {
            var agent = new Agent();
            var solver = new BooleanSolver();
            var variable = new Variable<bool>();

            var a1 = new BasicAction("Worse");
            a1.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(variable, true), solver);
            a1.AddCost(10);

            var a2 = new BasicAction("Better");
            a2.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(variable, true), solver);
            a2.AddCost(5);

            var need = new BooleanNeed(variable, true, solver);

            agent.AddNeed(need);
            agent.AddActions(a1, a2);

            var actions = Plan(agent);

            Assert.Single(actions);
            Assert.Equal("Better", (actions[0] as BasicAction).Tag);
        }

        [Theory]
        [InlineData(1, 5, 4, "Cheap")]
        [InlineData(2, 5, 1, "Expensive")]
        public void Plan_TwoActionsIntegerResult_PickCheaperPath(int cost1, int cost2, int actionCount, string betterTag)
        {
            const int REQUIRED = 4;

            var agent = new Agent();
            var solver = new IntegerSolver();
            var v1 = new Variable<int>();
            var v2 = new Variable<int>(1);

            var a1 = new BasicAction("Cheap");
            a1.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(new IntegerAddResult(v1, 1), solver);
            a1.AddCost(cost1);

            var a2 = new BasicAction("Expensive");
            a2.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(new IntegerAddResult(v1, REQUIRED), solver);
            a2.AddCost(cost2);

            var a3 = new BasicAction("Last");
            a3.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(new IntegerAddResult(v2, -1), solver);
            a3.AddPrecondition<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerPrecondition(v1, REQUIRED, IntegerPrecondition.Condition.AtLeast, solver));

            var need = new LinearIntegerNeed(v2, 0, solver);

            agent.AddNeed(need);
            agent.AddActions(a1, a2, a3);

            var actions = Plan(agent);

            Assert.Equal(actionCount + 1, actions.Count);
            Assert.Equal("Last", (actions[actionCount] as BasicAction).Tag);
            for (int i = 0; i < actionCount - 1; i++)
            {
                Assert.Equal(betterTag, (actions[i] as BasicAction).Tag);
            }
        }

        private class LinearNeed : Need<int>
        {
            public LinearNeed(Variable<int> variable, int desired, VariableSolver<int> solver) : base(variable, desired, solver)
            {
            }

            public override float Evaluate(int value)
            {
                return 25 * value;
            }
        }

        private class BoolNeed : Need<bool>
        {
            public BoolNeed(Variable<bool> variable, bool desired, VariableSolver<bool> solver) : base(variable, desired, solver)
            {
            }

            public override float Evaluate(bool value)
            {
                return value ? 100 : 40;
            }
        }

        [Fact]
        public void Plan_ChoosingDifferentPathAfterConditionsChanged()
        {
            var agent = new Agent();
            var a1 = new BasicAction("1");
            var a2 = new BasicAction("2");
            var a3 = new BasicAction("3");
            var a4 = new BasicAction("4");
            var a5 = new BasicAction("5");

            var v1 = new Variable<int>(1);
            var v2 = new Variable<bool>(false);
            var v3 = new Variable<bool>(false);
            var v4 = new Variable<bool>(false);

            var s1 = new IntegerSolver();
            var s2 = new BooleanSolver();
            var s3 = new BooleanSolver();
            var s4 = new BooleanSolver();

            var n1 = new LinearNeed(v1, 10, s1);
            var n2 = new BoolNeed(v2, true, s2);

            a1.AddCost(2);
            a1.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(v2, true), s2);

            a2.AddCost(3);
            a2.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(v3, true), s3);

            a3.AddCost(1);
            a3.AddCost(v2, v => v ? 100 : 0);
            a3.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(v4, true), s4);

            a4.AddCost(1);
            a4.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanPrecondition(v3, true, s3));
            a4.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(v3, false), s3);
            a4.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(new IntegerAddResult(v1, 1), s1);

            a5.AddCost(1);
            a5.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanPrecondition(v4, true, s4));
            a5.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(new BooleanResult(v4, false), s4);
            a5.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(new IntegerAddResult(v1, 1), s1);

            agent.AddActions(a1, a2, a3, a4, a5);
            agent.AddNeed(n1);
            agent.AddNeed(n2);

            List<Action> actions = Plan(agent);

            Assert.Equal(2, actions.Count);
            Assert.Equal("3", (actions[0] as BasicAction).Tag);
            Assert.Equal("5", (actions[1] as BasicAction).Tag);

            foreach(Action a in actions)
            {
                a.Perform();
            }

            actions = Plan(agent);
            Assert.Single(actions);
            Assert.Equal("1", (actions[0] as BasicAction).Tag);

            foreach (Action a in actions)
            {
                a.Perform();
            }

            actions = Plan(agent);
            Assert.Equal(2, actions.Count);
            Assert.Equal("2", (actions[0] as BasicAction).Tag);
            Assert.Equal("4", (actions[1] as BasicAction).Tag);
        }

        // helper functions
        List<Action> Plan(Agent agent, IEnumerable<Action> idleActions = null)
        {
            if (idleActions == null)
            {
                idleActions = Enumerable.Empty<Action>();
            }
            var planner = new ActionPlanner();
            return planner.Plan(agent, idleActions);
        }
    }
}
