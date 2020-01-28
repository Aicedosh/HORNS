using System;
using System.Collections.Generic;
using System.Text;
using HORNS;

namespace HORNS_Performance
{
    static class Tester
    {
        private class LogNeed : Need<int>
        {
            public LogNeed(Variable<int> variable, int desired) : base(variable, desired, v => (float)(8 * System.Math.Log10(v + 1)))
            {
            }
        }

        private class BoolNeed : Need<bool>
        {
            public BoolNeed(Variable<bool> variable, bool desired) : base(variable, desired, v => v ? 1 : 0)
            {
            }
        }

        private class BasicAction : HORNS.Action
        {
            public override void Perform()
            {
                Apply();
            }
        }

        // branches options at every node, but picks only one and discards the rest
        public static void TestTreeWithCuts(int branches, int depth, int needCount, bool print = true)
        {
            TestTree(branches, depth, needCount, false, print);
        }

        public static void TestUniformTree(int branches, int depth, int needCount, bool print = true)
        {
            TestTree(branches, depth, needCount, true, print);
        }

        private static void TestTree(int branches, int depth, int needCount, bool nocut, bool print)
        {
            var agent = new Agent();

            var needVars = new BooleanVariable[needCount];
            var needs = new BoolNeed[needCount];
            var needActions = new BasicAction[needCount];
            var preVarReg = new IntegerVariable();
            var preVarNocut = new IntegerNocutVariable();

            for (int i = 0; i < needCount; i++)
            {
                needVars[i] = new BooleanVariable();
                needs[i] = new BoolNeed(needVars[i], true);
                agent.AddNeed(needs[i]);

                needActions[i] = new BasicAction();
                needActions[i].AddResult(needVars[i], new BooleanResult(true));
                if (nocut)
                {
                    needActions[i].AddPrecondition(preVarNocut, new IntegerNocutPrecondition(depth));
                }
                else
                {
                    needActions[i].AddPrecondition(preVarReg, new IntegerPrecondition(depth, false));
                }
                needActions[i].AddCost(1);
                agent.AddAction(needActions[i]);
            }

            var preActions = new BasicAction[branches];
            for (int i = 0; i < branches; i++)
            {
                preActions[i] = new BasicAction();
                if (nocut)
                {
                    preActions[i].AddResult(preVarNocut, new IntegerAddResult(1));
                }
                else
                {
                    preActions[i].AddResult(preVarReg, new IntegerAddResult(1));
                }
                agent.AddAction(preActions[i]);
            }

            agent.RecalculateActions();
            if (print)
            {
                Logger.Log(branches, depth, needCount, agent.LastPlanTime);
            }
        }
    }
}
