using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HORNS;

namespace HORNS_Sandbox
{
    class Example3
    {
        private class ExampleAgent : Agent
        {
            public string Name;
            public IntVariable Hunger;

            public ExampleAgent(string name, IntVariable commonTrees)
            {
                Name = name;

                var hasAxe = new BoolVariable();
                Hunger = new IntVariable(50);
                var energy = new IntVariable(100);
                var money = new IntVariable();
                var wood = new IntVariable();
                var chairs = new IntVariable();
                var rzodkiews = new IntVariable();
                var soups = new IntVariable();

                var pickAxe = new TimeAction($"{Name} picking up an axe", 3);
                //pickAxe.AddCost(10);
                pickAxe.AddPrecondition(hasAxe, new BooleanPrecondition(false));
                pickAxe.AddResult(hasAxe, new BooleanResult(true));

                var chopTree = new TimeAction($"{Name} chopping down a tree", 5);
                //chopTree.AddCost(50);
                chopTree.AddPrecondition(commonTrees, new IntegerPrecondition(1, IntegerPrecondition.Condition.AtLeast));
                chopTree.AddPrecondition(hasAxe, new BooleanPrecondition(true));
                chopTree.AddResult(commonTrees, new IntegerAddResult(-1));
                chopTree.AddResult(wood, new IntegerAddResult(1));
                chopTree.AddResult(energy, new IntegerAddResult(-10));

                var sellWood = new TimeAction($"{Name} selling a piece of wood", 3);
                //sellWood.AddCost(30);
                sellWood.AddPrecondition(wood, new IntegerPrecondition(1, IntegerPrecondition.Condition.AtLeast));
                sellWood.AddResult(wood, new IntegerAddResult(-1));
                sellWood.AddResult(money, new IntegerAddResult(1));

                var makeChair = new TimeAction($"{Name} making a chair", 5);
                //makeChair.AddCost(50);
                makeChair.AddPrecondition(wood, new IntegerPrecondition(1, IntegerPrecondition.Condition.AtLeast));
                makeChair.AddResult(wood, new IntegerAddResult(-1));
                makeChair.AddResult(chairs, new IntegerAddResult(1));
                makeChair.AddResult(energy, new IntegerAddResult(-5));

                var sellChair = new TimeAction($"{Name} selling a chair", 3);
                //sellChair.AddCost(30);
                sellChair.AddPrecondition(chairs, new IntegerPrecondition(1, IntegerPrecondition.Condition.AtLeast));
                sellChair.AddResult(chairs, new IntegerAddResult(-1));
                sellChair.AddResult(money, new IntegerAddResult(3));

                var buyRzodkiew = new TimeAction($"{Name} buying a rzodkiew", 3);
                //buyRzodkiew.AddCost(30);
                buyRzodkiew.AddPrecondition(money, new IntegerPrecondition(3, IntegerPrecondition.Condition.AtLeast));
                buyRzodkiew.AddResult(money, new IntegerAddResult(-3));
                buyRzodkiew.AddResult(rzodkiews, new IntegerAddResult(1));

                var eatRzodkiew = new TimeAction($"{Name} eating a rzodkiew", 3);
                //eatRzodkiew.AddCost(10);
                //eatRzodkiew.AddCost(feelingSoupy, x => x ? 1000 : 0);
                eatRzodkiew.AddPrecondition(rzodkiews, new IntegerPrecondition(1, IntegerPrecondition.Condition.AtLeast));
                eatRzodkiew.AddResult(rzodkiews, new IntegerAddResult(-1));
                eatRzodkiew.AddResult(Hunger, new IntegerAddResult(-20));

                var makeSoup = new TimeAction($"{Name} making some soup", 5);
                //makeSoup.AddCost(20);
                makeSoup.AddPrecondition(rzodkiews, new IntegerPrecondition(2, IntegerPrecondition.Condition.AtLeast));
                makeSoup.AddResult(rzodkiews, new IntegerAddResult(-2));
                makeSoup.AddResult(soups, new IntegerAddResult(1));
                makeSoup.AddResult(energy, new IntegerAddResult(-3));

                var eatSoup = new TimeAction($"{Name} eating some soup", 3);
                //eatSoup.AddCost(1);
                eatSoup.AddPrecondition(soups, new IntegerPrecondition(1, IntegerPrecondition.Condition.AtLeast));
                eatSoup.AddResult(soups, new IntegerAddResult(-1));
                eatSoup.AddResult(Hunger, new IntegerAddResult(-60));
                eatSoup.AddResult(energy, new IntegerAddResult(1));

                var sleep = new TimeAction($"{Name} sleeping", 10);
                sleep.AddCost(10);
                sleep.AddResult(energy, new IntegerAddResult(100));

                var idle = new TimeAction($"{Name} is bored", 1);
                idle.AddCost(100000);

                var hungerNeed = new Hunger(Hunger, 0);
                var energyNeed = new Energy(energy, int.MaxValue);

                AddNeed(hungerNeed);
                AddNeed(energyNeed);
                AddActions(pickAxe, chopTree, sellWood, buyRzodkiew, eatRzodkiew, makeChair, sellChair, makeSoup, eatSoup, sleep);
                AddIdleAction(idle);
            }
        }

        private class WoodObserver : IVariableObserver
        {
            private List<Agent> agents;

            public WoodObserver(List<Agent> agents)
            {
                this.agents = agents;
            }

            public void ValueChanged()
            {
                Console.WriteLine("!!! Agents have to recalculate!");
                foreach (var agent in agents)
                {
                    agent.Recalculate = true;
                }
            }
        }

        public static void Run()
        {
            var trees = new IntVariable(1);
            ExampleAgent agent1 = new ExampleAgent("Tom", trees), agent2 = new ExampleAgent("Matt", trees);
            var agents = new List<Agent>() { agent1, agent2 };

            trees.Observe(new WoodObserver(agents));

            // main loop
            Random random = new Random(123);
            while (true)
            {
                agent1.Hunger.Value += 5;
                agent2.Hunger.Value += 5;

                int generated = random.Next(10);
                if (generated == 0)
                {
                    trees.Value += 2;
                }

                foreach (var agent in agents)
                {
                    Console.WriteLine($"### Trees amount: {trees.Value}");
                    var nextAction = agent.GetNextAction();
                    if (nextAction == null)
                    {
                        Console.WriteLine("No plan was found!");
                    }
                    else
                    {
                        nextAction.Perform();
                        nextAction.Apply();
                    }
                }
            }
        }
    }
}
