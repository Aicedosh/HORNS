using System;

namespace HORNS_Performance
{
    class Program
    {
        const int MAX_DEPTH = 10;
        const int DEPTH_STEP = 1;
        const int MAX_BRANCHES = 10;
        const int BRANCHES_STEP = 1;

        static void Main(string[] args)
        {
            // comment this line to output test logs to file
            Logger.WriteToFile = false;

            Logger.Log("START TEST");

            //for (int depth = DEPTH_STEP; depth <= MAX_DEPTH; depth += DEPTH_STEP)
            //{
            //    for (int branches = BRANCHES_STEP; branches <= MAX_BRANCHES; branches += BRANCHES_STEP)
            //    {
            //        Tester.TestTreeWithCuts(branches, depth, 1);
            //    }
            //}

            for (int depth = DEPTH_STEP; depth <= MAX_DEPTH; depth += DEPTH_STEP)
            {
                for (int branches = BRANCHES_STEP; branches <= MAX_BRANCHES; branches += BRANCHES_STEP)
                {
                    Tester.TestUniformTree(branches, depth, 1);
                }
            }
        }
    }
}
