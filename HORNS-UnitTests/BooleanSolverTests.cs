﻿using HORNS;
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
        public void GetActionsTowards_ShouldBeReturned_ByGetTowards(bool value)
        {
            BasicAction a = new BasicAction(1);
            Variable<bool> v = new Variable<bool>();
            BooleanSolver s = new BooleanSolver();
            BooleanResult result = new BooleanResult(v, value);

            result.Action = a;
            s.Register(result);

            List<Action> actions = new List<Action>(s.GetActionsTowards(v, value));
            Assert.Single(actions);
            Assert.Equal(a.N, (actions[0] as BasicAction).N);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetActionsTowards_TrueAndFalseResults_ShouldReturnOnlyOneValue_True(bool value)
        {
            BasicAction a = new BasicAction(1);
            BasicAction b = new BasicAction(2);

            Variable<bool> v = new Variable<bool>();
            BooleanSolver s = new BooleanSolver();
            BooleanResult result = new BooleanResult(v, value);
            BooleanResult other_result = new BooleanResult(v, !value);
            result.Action = a;
            other_result.Action = b;

            s.Register(result);
            s.Register(other_result);

            List<Action> actions = new List<Action>(s.GetActionsTowards(v, value));

            Assert.Single(actions);
            Assert.Equal(a.N, (actions[0] as BasicAction).N);
        }

        [Theory]
        [InlineData(true, 5)]
        [InlineData(false, 7)]
        public void GetActionsTowards_MultipleResults_ShouldReturnAll(bool value, int num)
        {
            Variable<bool> v = new Variable<bool>();
            BooleanSolver s = new BooleanSolver();

            List<BasicAction> actions = new List<BasicAction>();

            for (int i = 0; i < num; i++)
            {
                BasicAction action = new BasicAction(i);
                BooleanResult br = new BooleanResult(v, value);

                actions.Add(action);
                br.Action = action;
                s.Register(br);
            }

            List<Action> got = new List<Action>(s.GetActionsTowards(v, value));

            Assert.Equal(num, got.Count);
            Assert.Equal(actions.Select(a => a.N).OrderBy(n => n), got.Select(a => (a as BasicAction).N).OrderBy(n => n));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetActionsSatisfying_SingleMatchingResult_ShouldReturnAction(bool value)
        {
            Variable<bool> v = new Variable<bool>();
            BooleanSolver s = new BooleanSolver();

            BasicAction a = new BasicAction(1);
            BooleanResult result = new BooleanResult(v, value);
            result.Action = a;

            s.Register(result);

            List<Action> got = new List<Action>(s.GetActionsSatisfying(new BooleanPrecondition(v, value, s)));
            Assert.Single(got);
            Assert.Equal(a.N, (got[0] as BasicAction).N);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetActionsSatisfying_SingleNonMatchingResult_ShouldReturnNoActions(bool value)
        {
            Variable<bool> v = new Variable<bool>();
            BooleanSolver s = new BooleanSolver();

            BasicAction a = new BasicAction(1);
            BooleanResult result = new BooleanResult(v, !value);
            result.Action = a;

            s.Register(result);

            List<Action> got = new List<Action>(s.GetActionsSatisfying(new BooleanPrecondition(v, value, s)));
            Assert.Empty(got);
        }
    }
}