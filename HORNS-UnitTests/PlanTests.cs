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
            var variable = new BoolVariable();

            var action = new BasicAction();
            action.AddResult(variable, new BooleanResult(true));
            action.AddCost(1);

            var need = new BooleanNeed(variable, true);

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
            
            var v1 = new BoolVariable();
            var v2 = new BoolVariable();
            var v3 = new BoolVariable();

            var need = new BooleanNeed(v3, true);

            var a1 = new BasicAction("1");
            a1.AddResult(v1, new BooleanResult(true));

            var a2 = new BasicAction("2");
            a2.AddPrecondition(v1, new BooleanPrecondition(true));
            a2.AddResult(v2, new BooleanResult(true));

            var a3 = new BasicAction("3");
            a3.AddPrecondition(v2, new BooleanPrecondition(true));
            a3.AddResult(v3, new BooleanResult(true));

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

            var v1 = new IntVariable();
            var v2 = new BoolVariable();

            var need = new BooleanNeed(v2, true);

            var a1 = new BasicAction("Last");
            a1.AddPrecondition(v1, new IntegerPrecondition(REQUIRED_NUMBER, IntegerPrecondition.Condition.AtLeast));
            a1.AddResult(v2, new BooleanResult(true));

            var a2 = new BasicAction("Add one");
            a2.AddResult(v1, new IntegerAddResult(1));

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
            var variable = new BoolVariable();

            var a1 = new BasicAction("Worse");
            a1.AddResult(variable, new BooleanResult(true));
            a1.AddCost(10);

            var a2 = new BasicAction("Better");
            a2.AddResult(variable, new BooleanResult(true));
            a2.AddCost(5);

            var need = new BooleanNeed(variable, true);

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
            var v1 = new IntVariable();
            var v2 = new IntVariable(1);

            var a1 = new BasicAction("Cheap");
            a1.AddResult(v1, new IntegerAddResult(1));
            a1.AddCost(cost1);

            var a2 = new BasicAction("Expensive");
            a2.AddResult(v1, new IntegerAddResult(REQUIRED));
            a2.AddCost(cost2);

            var a3 = new BasicAction("Last");
            a3.AddResult(v2, new IntegerAddResult(-1));
            a3.AddPrecondition(v1, new IntegerPrecondition(REQUIRED, IntegerPrecondition.Condition.AtLeast));

            var need = new LinearIntegerNeed(v2, 0);

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

        [Fact]
        public void Plan_PathMakesNeedWorse_DiscardPath()
        {
            var agent = new Agent();
            var v1 = new IntVariable(3);
            var v2 = new IntVariable();
            var need = new LinearIntegerNeed(v1, 10);

            var a1 = new BasicAction("Fulfills need");
            a1.AddPrecondition(v2, new IntegerPrecondition(1, IntegerPrecondition.Condition.AtLeast));
            a1.AddResult(v1, new IntegerAddResult(1));

            var a2 = new BasicAction("Makes need worse");
            a2.AddResult(v1, new IntegerAddResult(-3));
            a2.AddResult(v2, new IntegerAddResult(1));

            agent.AddNeed(need);
            agent.AddActions(a1, a2);

            var actions = Plan(agent);
            Assert.Empty(actions);
        }

        private class LinearNeed : Need<int>
        {
            public LinearNeed(Variable<int> variable, int desired) : base(variable, desired)
            {
            }

            public override float Evaluate(int value)
            {
                return 25 * value;
            }
        }

        private class BoolNeed : Need<bool>
        {
            public BoolNeed(Variable<bool> variable, bool desired) : base(variable, desired)
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

            var v1 = new IntVariable(1);
            var v2 = new BoolVariable(false);
            var v3 = new BoolVariable(false);
            var v4 = new BoolVariable(false);

            var n1 = new LinearNeed(v1, 10);
            var n2 = new BoolNeed(v2, true);

            a1.AddCost(2);
            a1.AddResult(v2, new BooleanResult(true));

            a2.AddCost(3);
            a2.AddResult(v3, new BooleanResult(true));

            a3.AddCost(1);
            a3.AddCost(v2, v => v ? 100 : 0);
            a3.AddResult(v4, new BooleanResult(true));

            a4.AddCost(1);
            a4.AddPrecondition(v3, new BooleanPrecondition(true));
            a4.AddResult(v3, new BooleanResult(false));
            a4.AddResult(v1, new IntegerAddResult(1));

            a5.AddCost(1);
            a5.AddPrecondition(v4, new BooleanPrecondition(true));
            a5.AddResult(v4, new BooleanResult(false));
            a5.AddResult(v1, new IntegerAddResult(1));

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

        [Fact]
        public void Plan_AgentShouldOnlyPlanUsingHisOwnActions()
        {
            var agent1 = new Agent();
            var agent2 = new Agent();

            var v1 = new BoolVariable(false);

            Action a1 = new BasicAction();
            a1.AddResult(v1, new BooleanResult(true));

            agent2.AddAction(a1);

            BoolNeed b1 = new BoolNeed(v1, true);
            agent1.AddNeed(b1);

            var actions = Plan(agent1);
            Assert.Empty(actions);
        }

        [Theory]
        [InlineData(1, 2, "2")]
        [InlineData(2, 1, "1")]
        public void Plan_TwoActionsWithSameBaseCost_PreferOneThatChangesNeedMore(int change1, int change2, string expectedTag)
        {
            Agent agent = new Agent();

            var v = new IntVariable(0);
            var n = new LinearNeed(v, 10);
            agent.AddNeed(n);

            var a1 = new BasicAction("1");
            var a2 = new BasicAction("2");

            a1.AddResult(v, new IntegerAddResult(change1));
            a2.AddResult(v, new IntegerAddResult(change2));

            agent.AddActions(a1, a2);

            var actions = Plan(agent);
            Assert.Single(actions);
            Assert.Equal(expectedTag, (actions[0] as BasicAction).Tag);
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
