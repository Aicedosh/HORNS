using System;

namespace HORNS_Performance
{
    class Program
    {
        // for cut
        static int maxDepthCut = 700;
        static int depthStepCut = 100;
        static int maxBranchesCut = 700;
        static int branchesStepCut = 100;

        // for full
        static int maxDepthFull = 7;
        static int depthStepFull = 1;
        static int[] branchesForDepth = { 10000, 1000, 120, 25, 11, 7, 5, 0, 0, 0 };
        static int[] stepsForDepth = { 500, 50, 5, 1, 1, 1, 1, 1, 1, 1 };

        static void Main(string[] args)
        {
            Tester.TestUniformTree(1, 1, 1, false); // first test is always slower

            // comment this line to output test logs to file
            //Logger.WriteToFile = false;

            for (int depth = depthStepCut; depth <= maxDepthCut; depth += depthStepCut)
            {
                for (int branches = branchesStepCut; branches <= maxBranchesCut; branches += branchesStepCut)
                {
                    Tester.TestTreeWithCuts(branches, depth, 1);
                }
            }

            Logger.NewLog();

            for (int depth = depthStepFull; depth <= maxDepthFull; depth += depthStepFull)
            {
                for (int branches = stepsForDepth[depth - 1]; branches <= branchesForDepth[depth - 1]; branches += stepsForDepth[depth - 1])
                {
                    Tester.TestUniformTree(branches, depth, 1);
                }
            }
        }
    }
}
