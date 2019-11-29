using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HORNS;

namespace HORNS_Sandbox
{
    class Example2
    {
        private class MessageAction : HORNS.Action
        {
            public string Message { get; }

            public MessageAction(string message)
            {
                Message = message;
            }

            protected override void ActionResult()
            {
                Console.WriteLine(Message);
            }
        }

        private class Hunger : Need<int>
        {
            public Hunger(Variable<int> variable, int desired, VariableSolver<int> solver) : base(variable, desired, solver) { }

            public override float Evaluate(int value)
            {
                return 100 - value;
            }
        }

        private class Energy : Need<int>
        {
            public Energy(Variable<int> variable, int desired, VariableSolver<int> solver) : base(variable, desired, solver) { }

            public override float Evaluate(int value)
            {
                return (value > 100 ? 1000000 : 20) + value;
            }
        }

        public static void Run()
        {
            var hasAxe    = new Variable<bool>();
            var hunger    = new Variable<int>(50);
            var energy    = new Variable<int>(105);
            var money     = new Variable<int>();
            var wood      = new Variable<int>();
            var chairs    = new Variable<int>();
            var rzodkiews = new Variable<int>();
            var soups     = new Variable<int>();

            var feelingSoupy = new Variable<bool>();

            var hasAxeSolver    = new BooleanSolver();
            var hungerSolver    = new IntegerSolver();
            var energySolver    = new IntegerSolver();
            var moneySolver     = new IntegerSolver();
            var woodSolver      = new IntegerSolver();
            var chairsSolver    = new IntegerSolver();
            var rzodkiewsSolver = new IntegerSolver();
            var soupsSolver     = new IntegerSolver();

            var pickAxe = new MessageAction("Picked up an axe");
            pickAxe.AddCost(10);
            pickAxe.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(
                new BooleanPrecondition(hasAxe, false, hasAxeSolver));
            pickAxe.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(
                new BooleanResult(hasAxe, true), hasAxeSolver);

            var chopTree = new MessageAction("Chopped down a tree");
            chopTree.AddCost(50);
            chopTree.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(
                new BooleanPrecondition(hasAxe, true, hasAxeSolver));
            chopTree.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(wood, 1), woodSolver);
            chopTree.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(energy, -10), energySolver);

            var sellWood = new MessageAction("Sold a piece of wood");
            sellWood.AddCost(30);
            sellWood.AddPrecondition<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerPrecondition(wood, 1, IntegerPrecondition.Condition.AtLeast, woodSolver));
            sellWood.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(wood, -1), woodSolver);
            sellWood.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(money, 1), moneySolver);

            var makeChair = new MessageAction("Made a chair");
            makeChair.AddCost(50);
            makeChair.AddPrecondition<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerPrecondition(wood, 1, IntegerPrecondition.Condition.AtLeast, woodSolver));
            makeChair.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(wood, -1), woodSolver);
            makeChair.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(chairs, 1), chairsSolver);
            makeChair.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(energy, -5), energySolver);

            var sellChair = new MessageAction("Sold a chair");
            sellChair.AddCost(30);
            sellChair.AddPrecondition<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerPrecondition(chairs, 1, IntegerPrecondition.Condition.AtLeast, chairsSolver));
            sellChair.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(chairs, -1), chairsSolver);
            sellChair.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(money, 3), moneySolver);

            var buyRzodkiew = new MessageAction("Bought a rzodkiew");
            buyRzodkiew.AddCost(30);
            buyRzodkiew.AddPrecondition<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerPrecondition(money, 3, IntegerPrecondition.Condition.AtLeast, moneySolver));
            buyRzodkiew.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(money, -3), moneySolver);
            buyRzodkiew.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(rzodkiews, 1), rzodkiewsSolver);

            var eatRzodkiew = new MessageAction("Ate a rzodkiew");
            eatRzodkiew.AddCost(10);
            eatRzodkiew.AddCost(feelingSoupy, x => x ? 1000 : 0);
            eatRzodkiew.AddPrecondition<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerPrecondition(rzodkiews, 1, IntegerPrecondition.Condition.AtLeast, rzodkiewsSolver));
            eatRzodkiew.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(rzodkiews, -1), rzodkiewsSolver);
            eatRzodkiew.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(hunger, -20), hungerSolver);

            var makeSoup = new MessageAction("Made some soup");
            makeSoup.AddCost(20);
            makeSoup.AddPrecondition<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerPrecondition(rzodkiews, 2, IntegerPrecondition.Condition.AtLeast, rzodkiewsSolver));
            makeSoup.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(rzodkiews, -2), rzodkiewsSolver);
            makeSoup.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(soups, 1), soupsSolver);
            makeSoup.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(energy, -3), energySolver);

            var eatSoup = new MessageAction("Ate some soup");
            eatSoup.AddCost(1);
            eatSoup.AddPrecondition<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerPrecondition(soups, 1, IntegerPrecondition.Condition.AtLeast, soupsSolver));
            eatSoup.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(soups, -1), soupsSolver);
            eatSoup.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(hunger, -60), hungerSolver);
            eatSoup.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(energy, 1), energySolver);

            var sleep = new MessageAction("Went to sleep");
            sleep.AddCost(10);
            sleep.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(energy, 100), energySolver);

            var idle = new MessageAction("Is bored");
            idle.AddCost(100000);

            var hungerNeed = new Hunger(hunger, 0, hungerSolver);
            var energyNeed = new Energy(energy, int.MaxValue, energySolver);

            var agent = new Agent();
            agent.AddNeed(hungerNeed);
            agent.AddNeed(energyNeed);
            agent.AddActions(pickAxe, chopTree, sellWood, buyRzodkiew, eatRzodkiew, makeChair, sellChair, makeSoup, eatSoup, sleep);
            agent.AddIdleAction(idle);

            // main loop
            Random random = new Random();
            while (true)
            {
                energy.Value -= 1;
                hunger.Value += 5;
                feelingSoupy.Value = random.Next(2) == 0;

                var nextAction = agent.GetNextAction();
                if (nextAction == null)
                {
                    Console.WriteLine("No plan was found!");
                }
                else
                {
                    nextAction.Perform();
                }

                Thread.Sleep(100);
            }
        }
    }
}
