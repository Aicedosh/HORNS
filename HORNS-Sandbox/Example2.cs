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

            public override void Perform()
            {
                Console.WriteLine(Message);
                Apply();
            }
        }

        private class Hunger : Need<int>
        {
            public Hunger(Variable<int> variable, int desired) : base(variable, desired) { }

            public override float Evaluate(int value)
            {
                return 100 - value;
            }
        }

        private class Energy : Need<int>
        {
            public Energy(Variable<int> variable, int desired) : base(variable, desired) { }

            public override float Evaluate(int value)
            {
                return (value > 100 ? 1000000 : 20) + value;
            }
        }
        
        public static void Run()
        {
            var hasAxe    = new BoolVariable();
            var hunger    = new IntVariable(50);
            var energy    = new IntVariable(105);
            var money     = new IntVariable();
            var wood      = new IntVariable();
            var chairs    = new IntVariable();
            var rzodkiews = new IntVariable();
            var soups     = new IntVariable();

            var feelingSoupy = new BoolVariable();

            var pickAxe = new MessageAction("Picked up an axe");
            pickAxe.AddCost(10);
            pickAxe.AddPrecondition(hasAxe, new BooleanPrecondition(false));
            pickAxe.AddResult(hasAxe, new BooleanResult(true));

            var chopTree = new MessageAction("Chopped down a tree");
            chopTree.AddCost(50);
            chopTree.AddPrecondition(hasAxe, new BooleanPrecondition(true));
            chopTree.AddResult(wood, new IntegerAddResult(1));
            chopTree.AddResult(energy, new IntegerAddResult(-10));

            var sellWood = new MessageAction("Sold a piece of wood");
            sellWood.AddCost(30);
            sellWood.AddPrecondition(wood, new IntegerPrecondition(1, IntegerPrecondition.Condition.AtLeast));
            sellWood.AddResult(wood, new IntegerAddResult(-1));
            sellWood.AddResult(money, new IntegerAddResult(1));

            var makeChair = new MessageAction("Made a chair");
            makeChair.AddCost(50);
            makeChair.AddPrecondition(wood, new IntegerPrecondition(1, IntegerPrecondition.Condition.AtLeast));
            makeChair.AddResult(wood, new IntegerAddResult(-1));
            makeChair.AddResult(chairs, new IntegerAddResult(1));
            makeChair.AddResult(energy, new IntegerAddResult(-5));

            var sellChair = new MessageAction("Sold a chair");
            sellChair.AddCost(30);
            sellChair.AddPrecondition(chairs, new IntegerPrecondition(1, IntegerPrecondition.Condition.AtLeast));
            sellChair.AddResult(chairs, new IntegerAddResult(-1));
            sellChair.AddResult(money, new IntegerAddResult(3));

            var buyRzodkiew = new MessageAction("Bought a rzodkiew");
            buyRzodkiew.AddCost(30);
            buyRzodkiew.AddPrecondition(money, new IntegerPrecondition(3, IntegerPrecondition.Condition.AtLeast));
            buyRzodkiew.AddResult(money, new IntegerAddResult(-3));
            buyRzodkiew.AddResult(rzodkiews, new IntegerAddResult(1));

            var eatRzodkiew = new MessageAction("Ate a rzodkiew");
            eatRzodkiew.AddCost(10);
            eatRzodkiew.AddCost(feelingSoupy, x => x ? 1000 : 0);
            eatRzodkiew.AddPrecondition(rzodkiews, new IntegerPrecondition(1, IntegerPrecondition.Condition.AtLeast));
            eatRzodkiew.AddResult(rzodkiews, new IntegerAddResult(-1));
            eatRzodkiew.AddResult(hunger, new IntegerAddResult(-20));

            var makeSoup = new MessageAction("Made some soup");
            makeSoup.AddCost(20);
            makeSoup.AddPrecondition(rzodkiews, new IntegerPrecondition(2, IntegerPrecondition.Condition.AtLeast));
            makeSoup.AddResult(rzodkiews, new IntegerAddResult(-2));
            makeSoup.AddResult(soups, new IntegerAddResult(1));
            makeSoup.AddResult(energy, new IntegerAddResult(-3));

            var eatSoup = new MessageAction("Ate some soup");
            eatSoup.AddCost(1);
            eatSoup.AddPrecondition(soups, new IntegerPrecondition(1, IntegerPrecondition.Condition.AtLeast));
            eatSoup.AddResult(soups, new IntegerAddResult(-1));
            eatSoup.AddResult(hunger, new IntegerAddResult(-60));
            eatSoup.AddResult(energy, new IntegerAddResult(1));

            var sleep = new MessageAction("Went to sleep");
            sleep.AddCost(10);
            sleep.AddResult(energy, new IntegerAddResult(100));

            var idle = new MessageAction("Is bored");
            idle.AddCost(100000);

            var hungerNeed = new Hunger(hunger, 0);
            var energyNeed = new Energy(energy, int.MaxValue);

            var agent = new Agent();
            agent.AddNeed(hungerNeed);
            agent.AddNeed(energyNeed);
            agent.AddActions(pickAxe, chopTree, sellWood, buyRzodkiew, eatRzodkiew, makeChair, sellChair, makeSoup, eatSoup, sleep);
            agent.AddIdleAction(idle);

            // main loop
            Random random = new Random(42);
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

                Thread.Sleep(500);
            }
        }
    }
}
