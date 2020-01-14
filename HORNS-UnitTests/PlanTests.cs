using Xunit;
using HORNS;
using System.Linq;
using System.Collections.Generic;

namespace HORNS_UnitTests
{
    public class PlanTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Plan_CarpenterTest(bool snapshot)
        {
            var agent = new Agent();
            var hasWood = new BooleanVariable(false);
            var isShopOpen = new BooleanVariable(true);
            var hasCrate = new BooleanVariable(false);
            var workshopHasWood = new BooleanVariable(false);
            var workshopHasCrate = new BooleanVariable(false);
            var woodCount = new IntegerVariable(1);
            var money = new IntegerVariable(0);

            var buyWood = new BasicAction("BuyWood");
            buyWood.AddPrecondition(isShopOpen, new BooleanPrecondition(true));
            buyWood.AddPrecondition(hasWood, new BooleanPrecondition(false));
            buyWood.AddPrecondition(woodCount, new IntegerPrecondition(1));
            buyWood.AddResult(woodCount, new IntegerAddResult(-1));
            buyWood.AddResult(hasWood, new BooleanResult(true));

            var carryWood = new BasicAction("CarryWood");
            carryWood.AddPrecondition(hasWood, new BooleanPrecondition(true));
            carryWood.AddPrecondition(workshopHasWood, new BooleanPrecondition(false));
            carryWood.AddResult(hasWood, new BooleanResult(false));
            carryWood.AddResult(workshopHasWood, new BooleanResult(true));

            var create = new BasicAction("Create");
            create.AddPrecondition(workshopHasWood, new BooleanPrecondition(true));
            create.AddPrecondition(workshopHasCrate, new BooleanPrecondition(false));
            create.AddResult(workshopHasWood, new BooleanResult(false));
            create.AddResult(workshopHasCrate, new BooleanResult(true));

            var pickupCrate = new BasicAction("PickupCrate");
            pickupCrate.AddPrecondition(workshopHasCrate, new BooleanPrecondition(true));
            pickupCrate.AddPrecondition(hasCrate, new BooleanPrecondition(false));
            pickupCrate.AddResult(workshopHasCrate, new BooleanResult(false));
            pickupCrate.AddResult(hasCrate, new BooleanResult(true));

            var sellCrate = new BasicAction("SellCrate");
            sellCrate.AddPrecondition(hasCrate, new BooleanPrecondition(true));
            sellCrate.AddPrecondition(isShopOpen, new BooleanPrecondition(true));
            sellCrate.AddResult(hasCrate, new BooleanResult(false));
            sellCrate.AddResult(money, new IntegerAddResult(5));

            var need = new Need<int>(money, 100, v => v);
            agent.AddActions(buyWood, carryWood, create, pickupCrate, sellCrate);
            agent.AddNeed(need);

            var (actions, curNeed) = Plan(agent, snapshot);
            Assert.NotNull(actions);
            Assert.Equal(5, actions.Count);
            Assert.Equal("BuyWood", (actions[0] as BasicAction).Tag);
            Assert.Equal("CarryWood", (actions[1] as BasicAction).Tag);
            Assert.Equal("Create", (actions[2] as BasicAction).Tag);
            Assert.Equal("PickupCrate", (actions[3] as BasicAction).Tag);
            Assert.Equal("SellCrate", (actions[4] as BasicAction).Tag);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Plan_OneAction_NoPreconditions(bool snapshot)
        {
            var agent = new Agent();
            var variable = new BooleanVariable();

            var action = new BasicAction();
            action.AddResult(variable, new BooleanResult(true));
            action.AddCost(1);

            var need = new BooleanNeed(variable, true);

            agent.AddAction(action);
            agent.AddNeed(need);

            var (actions, curNeed) = Plan(agent, snapshot);

            Assert.Equal(need, curNeed);
            Assert.Single(actions);

            actions[0].Perform();
            Assert.True(variable.Value);
            Assert.True(need.IsSatisfied());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Plan_ThreeStepResult_BooleanVariables(bool snapshot)
        {
            var agent = new Agent();
            
            var v1 = new BooleanVariable();
            var v2 = new BooleanVariable();
            var v3 = new BooleanVariable();

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

            var (actions, curNeed) = Plan(agent, snapshot);

            Assert.Equal(3, actions.Count);
            Assert.Equal("1", (actions[0] as BasicAction).Tag);
            Assert.Equal("2", (actions[1] as BasicAction).Tag);
            Assert.Equal("3", (actions[2] as BasicAction).Tag);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Plan_SingleIntPrecondition_RequiredMultipleTimesOnPath(bool snapshot)
        {
            const int REQUIRED_NUMBER = 5;
            var agent = new Agent();

            var v1 = new IntegerVariable();
            var v2 = new BooleanVariable();

            var need = new BooleanNeed(v2, true);

            var a1 = new BasicAction("Last");
            a1.AddPrecondition(v1, new IntegerPrecondition(REQUIRED_NUMBER));
            a1.AddResult(v2, new BooleanResult(true));

            var a2 = new BasicAction("Add one");
            a2.AddResult(v1, new IntegerAddResult(1));

            agent.AddActions(a1, a2);
            agent.AddNeed(need);

            var (actions, curNeed) = Plan(agent, snapshot);

            Assert.Equal(REQUIRED_NUMBER + 1, actions.Count);
            Assert.Equal("Last", (actions[REQUIRED_NUMBER] as BasicAction).Tag);
            for (int i = 0; i < REQUIRED_NUMBER - 1; i++)
            {
                Assert.Equal("Add one", (actions[i] as BasicAction).Tag);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Plan_TwoActionsBooleanResult_PicksBetterAction(bool snapshot)
        {
            var agent = new Agent();
            var variable = new BooleanVariable();

            var a1 = new BasicAction("Worse");
            a1.AddResult(variable, new BooleanResult(true));
            a1.AddCost(10);

            var a2 = new BasicAction("Better");
            a2.AddResult(variable, new BooleanResult(true));
            a2.AddCost(5);

            var need = new BooleanNeed(variable, true);

            agent.AddNeed(need);
            agent.AddActions(a1, a2);

            var (actions, curNeed) = Plan(agent, snapshot);

            Assert.Single(actions);
            Assert.Equal("Better", (actions[0] as BasicAction).Tag);
        }

        [Theory]
        [InlineData(1, 5, 4, "Cheap", true)]
        [InlineData(2, 5, 1, "Expensive", true)]
        [InlineData(1, 5, 4, "Cheap", false)]
        [InlineData(2, 5, 1, "Expensive", false)]
        public void Plan_TwoActionsIntegerResult_PickCheaperPath(int cost1, int cost2, int actionCount, string betterTag, bool snapshot)
        {
            const int REQUIRED = 4;

            var agent = new Agent();
            var v1 = new IntegerVariable();
            var v2 = new IntegerVariable(1);

            var a1 = new BasicAction("Cheap");
            a1.AddResult(v1, new IntegerAddResult(1));
            a1.AddCost(cost1);

            var a2 = new BasicAction("Expensive");
            a2.AddResult(v1, new IntegerAddResult(REQUIRED));
            a2.AddCost(cost2);

            var a3 = new BasicAction("Last");
            a3.AddResult(v2, new IntegerAddResult(-1));
            a3.AddPrecondition(v1, new IntegerPrecondition(REQUIRED));

            var need = new LinearIntegerNeed(v2, 0);

            agent.AddNeed(need);
            agent.AddActions(a1, a2, a3);

            var (actions, curNeed) = Plan(agent, snapshot);

            Assert.Equal(actionCount + 1, actions.Count);
            Assert.Equal("Last", (actions[actionCount] as BasicAction).Tag);
            for (int i = 0; i < actionCount - 1; i++)
            {
                Assert.Equal(betterTag, (actions[i] as BasicAction).Tag);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Plan_PathMakesNeedWorse_DiscardPath(bool snapshot)
        {
            var agent = new Agent();
            var v1 = new IntegerVariable(3);
            var v2 = new IntegerVariable();
            var need = new LinearIntegerNeed(v1, 10);

            var a1 = new BasicAction("Fulfills need");
            a1.AddPrecondition(v2, new IntegerPrecondition(1));
            a1.AddResult(v1, new IntegerAddResult(1));

            var a2 = new BasicAction("Makes need worse");
            a2.AddResult(v1, new IntegerAddResult(-3));
            a2.AddResult(v2, new IntegerAddResult(1));

            agent.AddNeed(need);
            agent.AddActions(a1, a2);

            var (actions, curNeed) = Plan(agent, snapshot);
            Assert.Empty(actions);
        }

        private class LogNeed : Need<int>
        {
            public LogNeed(Variable<int> variable, int desired) : base(variable, desired, v=>(float)(8*System.Math.Log10(v+1)))
            {
            }
        }

        private class BoolNeed : Need<bool>
        {
            public BoolNeed(Variable<bool> variable, bool desired) : base(variable, desired, v=>v ? 1 : 0)
            {
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Plan_ChoosingDifferentPathAfterConditionsChanged(bool snapshot)
        {
            var agent = new Agent();
            var a1 = new BasicAction("1");
            var a2 = new BasicAction("2");
            var a3 = new BasicAction("3");
            var a4 = new BasicAction("4");
            var a5 = new BasicAction("5");

            var v1 = new IntegerVariable(0);
            var v2 = new BooleanVariable(false);
            var v3 = new BooleanVariable(false);
            var v4 = new BooleanVariable(false);

            var n1 = new LogNeed(v1, 10);
            var n2 = new BoolNeed(v2, true);

            a1.AddResult(v2, new BooleanResult(true));

            a2.AddCost(3);
            a2.AddResult(v3, new BooleanResult(true));

            a3.AddCost(1);
            a3.AddCost(v2, v => v ? 99 : 0);
            a3.AddResult(v4, new BooleanResult(true));

            a4.AddPrecondition(v3, new BooleanPrecondition(true));
            a4.AddResult(v3, new BooleanResult(false));
            a4.AddResult(v1, new IntegerAddResult(1));

            a5.AddPrecondition(v4, new BooleanPrecondition(true));
            a5.AddResult(v4, new BooleanResult(false));
            a5.AddResult(v1, new IntegerAddResult(1));

            agent.AddActions(a1, a2, a3, a4, a5);
            agent.AddNeed(n1);
            agent.AddNeed(n2);

            var (actions, curNeed) = Plan(agent, snapshot);

            Assert.Equal(n1, curNeed);
            Assert.Equal(2, actions.Count);
            Assert.Equal("3", (actions[0] as BasicAction).Tag);
            Assert.Equal("5", (actions[1] as BasicAction).Tag);

            foreach(Action a in actions)
            {
                a.Perform();
            }

            (actions, curNeed) = Plan(agent, snapshot);
            Assert.Equal(n2, curNeed);
            Assert.Single(actions);
            Assert.Equal("1", (actions[0] as BasicAction).Tag);

            foreach (Action a in actions)
            {
                a.Perform();
            }

            (actions, curNeed) = Plan(agent, snapshot);
            Assert.Equal(n1, curNeed);
            Assert.Equal(2, actions.Count);
            Assert.Equal("2", (actions[0] as BasicAction).Tag);
            Assert.Equal("4", (actions[1] as BasicAction).Tag);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Plan_AgentShouldOnlyPlanUsingHisOwnActions(bool snapshot)
        {
            var agent1 = new Agent();
            var agent2 = new Agent();

            var v1 = new BooleanVariable(false);

            Action a1 = new BasicAction();
            a1.AddResult(v1, new BooleanResult(true));

            agent2.AddAction(a1);

            BoolNeed b1 = new BoolNeed(v1, true);
            agent1.AddNeed(b1);

            var (actions, curNeed) = Plan(agent1, snapshot);
            Assert.Empty(actions);
        }

        [Theory]
        [InlineData(1, 2, "2", true)]
        [InlineData(2, 1, "1", true)]
        [InlineData(1, 2, "2", false)]
        [InlineData(2, 1, "1", false)]
        public void Plan_TwoActionsWithSameBaseCost_PreferOneThatChangesNeedMore(int change1, int change2, string expectedTag, bool snapshot)
        {
            Agent agent = new Agent();

            var v = new IntegerVariable(0);
            var n = new LogNeed(v, 10);
            agent.AddNeed(n);

            var a1 = new BasicAction("1");
            var a2 = new BasicAction("2");

            a1.AddResult(v, new IntegerAddResult(change1));
            a2.AddResult(v, new IntegerAddResult(change2));

            agent.AddActions(a1, a2);

            var (actions, curNeed) = Plan(agent, snapshot);
            Assert.Single(actions);
            Assert.Equal(expectedTag, (actions[0] as BasicAction).Tag);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Plan_IdleWithUnfulfilledPreconditions_DiscardIdle(bool snapshot)
        {
            Agent agent = new Agent();
            var v = new IntegerVariable(0);

            var a = new BasicAction("Unfulfilled");
            a.AddPrecondition(v, new IntegerPrecondition(1));
            agent.AddIdleAction(a);

            var (actions, curNeed) = Plan(agent, snapshot);
            Assert.Empty(actions);
        }

        [Theory]
        [InlineData(1, 5, true)]
        [InlineData(1, 5, false)]
        [InlineData(5, 1, true)]
        [InlineData(5, 1, false)]
        public void Plan_IdlesWithUnfulfilledAndFulfilledPreconditions_ChooseFulfilled(int cost1, int cost2, bool snapshot)
        {
            Agent agent = new Agent();
            var v1 = new IntegerVariable(0);
            var v2 = new IntegerVariable(5);

            var a1 = new BasicAction("Unfulfilled");
            a1.AddPrecondition(v1, new IntegerPrecondition(1));
            a1.AddCost(cost1);

            var a2 = new BasicAction("Fulfilled");
            a2.AddPrecondition(v2, new IntegerPrecondition(5));
            a2.AddCost(cost2);

            agent.AddIdleActions(a1, a2);

            var (actions, curNeed) = Plan(agent, snapshot);
            Assert.Single(actions);
            Assert.Equal("Fulfilled", (actions[0] as BasicAction).Tag);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Plan_IdleWithPreconditions_PlanEntirePath(bool snapshot)
        {
            Agent agent = new Agent();
            var v = new BooleanVariable();

            var a1 = new BasicAction("Idle");
            a1.AddPrecondition(v, new BooleanPrecondition(true));
            agent.AddIdleAction(a1);

            var a2 = new BasicAction("Required");
            a2.AddResult(v, new BooleanResult(true));
            agent.AddAction(a2);

            var (actions, curNeed) = Plan(agent, snapshot);
            Assert.Equal(2, actions.Count);
            Assert.Equal("Required", (actions[0] as BasicAction).Tag);
            Assert.Equal("Idle", (actions[1] as BasicAction).Tag);
        }

        private (Agent agent, INeed need) NeedVersusIdle_Setup(float costIdle, float costReq, float costNeed)
        {
            Agent agent = new Agent();
            var v1 = new BooleanVariable();
            var v2 = new IntegerVariable();
            var n = new LogNeed(v2, 5);

            var a1 = new BasicAction("Idle");
            a1.AddCost(costIdle);
            a1.AddPrecondition(v1, new BooleanPrecondition(true));

            var a2 = new BasicAction("Required");
            a2.AddCost(costReq);
            a2.AddResult(v1, new BooleanResult(true));

            var a3 = new BasicAction("Fulfill need");
            a3.AddCost(costNeed);
            a3.AddResult(v2, new IntegerAddResult(5));

            agent.AddNeed(n);
            agent.AddIdleAction(a1);
            agent.AddActions(a2, a3);

            return (agent, n);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Plan_NeedVersusIdle_NeedMoreExpensive_ChooseIdlePath(bool snapshot)
        {
            var (agent, _) = NeedVersusIdle_Setup(1, 1, 100);

            var (actions, curNeed) = Plan(agent, snapshot);
            Assert.Null(curNeed);
            Assert.Equal(2, actions.Count);
            Assert.Equal("Required", (actions[0] as BasicAction).Tag);
            Assert.Equal("Idle", (actions[1] as BasicAction).Tag);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Plan_NeedVersusIdle_IdleMoreExpensive_ChooseNeedPath(bool snapshot)
        {
            var (agent, need) = NeedVersusIdle_Setup(100, 100, 1);

            var (actions, curNeed) = Plan(agent, snapshot);
            Assert.Equal(need, curNeed);
            Assert.Single(actions);
            Assert.Equal("Fulfill need", (actions[0] as BasicAction).Tag);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Plan_CombiningPreconditions_FindCorrectPath(bool snapshot)
        {
            var vint = new IntegerVariable();
            var vbool = new BooleanVariable();
            var vneed = new BooleanVariable();
            var n = new BooleanNeed(vneed, true);

            var a1 = new BasicAction("Fulfills need");
            a1.AddPrecondition(vbool, new BooleanPrecondition(true));
            a1.AddPrecondition(vint, new IntegerPrecondition(2));
            a1.AddResult(vneed, new BooleanResult(true));

            var a2 = new BasicAction("Fulfills bool precondition");
            a2.AddPrecondition(vint, new IntegerPrecondition(3));
            a2.AddResult(vint, new IntegerAddResult(-3));
            a2.AddResult(vbool, new BooleanResult(true));
            
            var a3 = new BasicAction("Partially fulfills integer precondition");
            a3.AddPrecondition(vbool, new BooleanPrecondition(false));
            a3.AddResult(vint, new IntegerAddResult(1));

            var agent = new Agent();
            agent.AddActions(a1, a2, a3);
            agent.AddNeed(n);

            var (actions, curNeed) = Plan(agent, snapshot);
            Assert.Equal(7, actions.Count);
            Assert.Equal("Fulfills need", (actions[actions.Count - 1] as BasicAction).Tag);
            Assert.Equal("Fulfills bool precondition", (actions[actions.Count - 2] as BasicAction).Tag);
        }

        // helper functions
        private (List<Action> actions, INeed need) Plan(Agent agent, bool snapshot)
        {
            var planner = new ActionPlanner();
            return planner.Plan(agent, snapshot);
        }
    }
}
