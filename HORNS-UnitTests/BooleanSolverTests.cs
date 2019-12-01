using HORNS;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HORNS_UnitTests
{
    public class BooleanSolverTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetActionsTowards_ShouldReturnRegisteredActionResult(bool value)
        {
            BasicAction a = new BasicAction("1");
            Variable<bool> v = new BoolVariable();
            BooleanSolver s = new BooleanSolver();
            BooleanResult result = new BooleanResult(value);

            result.Action = a;
            s.Register(result);

            Agent agent = new Agent();
            agent.AddAction(a);

            List<Action> actions = new List<Action>(s.GetActionsTowards(v, value, agent));
            Assert.Single(actions);
            Assert.Equal(a.Tag, (actions[0] as BasicAction).Tag);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetActionsTowards_TrueAndFalseResults_ShouldReturnOnlyOneValue(bool value)
        {
            BasicAction a = new BasicAction("1");
            BasicAction b = new BasicAction("2");

            Variable<bool> v = new BoolVariable();
            BooleanSolver s = new BooleanSolver();
            BooleanResult result = new BooleanResult(value);
            BooleanResult other_result = new BooleanResult(!value);
            result.Action = a;
            other_result.Action = b;

            s.Register(result);
            s.Register(other_result);

            Agent agent = new Agent();
            agent.AddAction(a);
            agent.AddAction(b);

            List<Action> actions = new List<Action>(s.GetActionsTowards(v, value, agent));

            Assert.Single(actions);
            Assert.Equal(a.Tag, (actions[0] as BasicAction).Tag);
        }

        [Theory]
        [InlineData(true, 5)]
        [InlineData(false, 7)]
        public void GetActionsTowards_MultipleResults_ShouldReturnAll(bool value, int num)
        {
            Variable<bool> v = new BoolVariable();
            BooleanSolver s = new BooleanSolver();

            List<BasicAction> actions = new List<BasicAction>();
            Agent agent = new Agent();

            for (int i = 0; i < num; i++)
            {
                BasicAction action = new BasicAction(i.ToString());
                BooleanResult br = new BooleanResult(value);

                actions.Add(action);
                br.Action = action;
                s.Register(br);

                agent.AddAction(action);
            }

            List<Action> got = new List<Action>(s.GetActionsTowards(v, value, agent));

            Assert.Equal(num, got.Count);
            Assert.Equal(actions.Select(a => a.Tag).OrderBy(n => n), got.Select(a => (a as BasicAction).Tag).OrderBy(n => n));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetActionsSatisfying_SingleMatchingResult_ShouldReturnAction(bool value)
        {
            Variable<bool> v = new BoolVariable();
            BooleanSolver s = new BooleanSolver();

            BasicAction a = new BasicAction("1");
            BooleanResult result = new BooleanResult(value);
            result.Action = a;

            s.Register(result);

            Agent agent = new Agent();
            agent.AddAction(a);

            List<Action> got = new List<Action>(s.GetActionsSatisfying(new BooleanPrecondition(value), agent));
            Assert.Single(got);
            Assert.Equal(a.Tag, (got[0] as BasicAction).Tag);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetActionsSatisfying_SingleNonMatchingResult_ShouldReturnNoActions(bool value)
        {
            Variable<bool> v = new BoolVariable();
            BooleanSolver s = new BooleanSolver();

            BasicAction a = new BasicAction("1");
            BooleanResult result = new BooleanResult(!value);
            result.Action = a;

            s.Register(result);

            Agent agent = new Agent();
            agent.AddAction(a);

            List<Action> got = new List<Action>(s.GetActionsSatisfying(new BooleanPrecondition(value), agent));
            Assert.Empty(got);
        }
    }
}
