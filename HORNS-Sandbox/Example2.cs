using System;
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
            public Hunger(Variable<int> variable, int desired) : base(variable, desired, v =>
            v > 100 ? -100 :
            (float)(50 * Math.Log10(-v + 1 + 100)))
            { }
        }

        private class Energy : Need<int>
        {
            public Energy(Variable<int> variable, int desired) : base(variable, desired, v =>
            v < 0 ? 100 : (float)(50 * Math.Log10(v + 1)))
            { }

            protected override bool IsSatisfied(int value)
            {
                return value >= 100;
            }
        }

        public static void Run()
        {
            var hasAxe = new BooleanVariable();
            var hunger = new IntegerConsumeVariable(100);
            var energy = new IntegerConsumeVariable(100);
            var money = new IntegerConsumeVariable();
            var wood = new IntegerConsumeVariable();
            var chairs = new IntegerConsumeVariable();
            var rzodkiews = new IntegerConsumeVariable();
            var soups = new IntegerConsumeVariable();

            var feelingSoupy = new BooleanVariable();

            var pickAxe = new MessageAction("Picked up an axe");
            //pickAxe.AddCost(10);
            pickAxe.AddPrecondition(hasAxe, new BooleanPrecondition(false));
            pickAxe.AddResult(hasAxe, new BooleanResult(true));

            var chopTree = new MessageAction("Chopped down a tree");
            //chopTree.AddCost(50);
            chopTree.AddPrecondition(hasAxe, new BooleanPrecondition(true));
            chopTree.AddResult(wood, new IntegerAddResult(1));
            chopTree.AddResult(energy, new IntegerAddResult(-2));

            var sellWood = new MessageAction("Sold a piece of wood");
            //sellWood.AddCost(30);
            sellWood.AddPrecondition(wood, new IntegerConsumePrecondition(1));
            sellWood.AddResult(wood, new IntegerAddResult(-1));
            sellWood.AddResult(money, new IntegerAddResult(1));

            var makeChair = new MessageAction("Made a chair");
            //makeChair.AddCost(5);
            makeChair.AddPrecondition(wood, new IntegerConsumePrecondition(1));
            makeChair.AddResult(wood, new IntegerAddResult(-1));
            makeChair.AddResult(chairs, new IntegerAddResult(1));
            makeChair.AddResult(energy, new IntegerAddResult(-3));

            var sellChair = new MessageAction("Sold a chair");
            //sellChair.AddCost(30);
            sellChair.AddPrecondition(chairs, new IntegerConsumePrecondition(1));
            sellChair.AddResult(chairs, new IntegerAddResult(-1));
            sellChair.AddResult(money, new IntegerAddResult(3));

            var buyRzodkiew = new MessageAction("Bought a rzodkiew");
            //buyRzodkiew.AddCost(30);
            buyRzodkiew.AddPrecondition(money, new IntegerConsumePrecondition(3));
            buyRzodkiew.AddResult(money, new IntegerAddResult(-3));
            buyRzodkiew.AddResult(rzodkiews, new IntegerAddResult(1));

            var eatRzodkiew = new MessageAction("Ate a rzodkiew");
            //eatRzodkiew.AddCost(20);
            eatRzodkiew.AddPrecondition(rzodkiews, new IntegerConsumePrecondition(1));
            eatRzodkiew.AddResult(rzodkiews, new IntegerAddResult(-1));
            eatRzodkiew.AddResult(hunger, new IntegerAddResult(-5));

            var makeSoup = new MessageAction("Made some soup");
            //makeSoup.AddCost(5);
            makeSoup.AddPrecondition(rzodkiews, new IntegerConsumePrecondition(2));
            makeSoup.AddResult(rzodkiews, new IntegerAddResult(-2));
            makeSoup.AddResult(soups, new IntegerAddResult(1));
            makeSoup.AddResult(energy, new IntegerAddResult(-11));

            var eatSoup = new MessageAction("Ate some soup");
            //eatSoup.AddCost(1);
            eatSoup.AddPrecondition(soups, new IntegerConsumePrecondition(1));
            eatSoup.AddResult(soups, new IntegerAddResult(-1));
            eatSoup.AddResult(hunger, new IntegerAddResult(-20));
            eatSoup.AddResult(energy, new IntegerAddResult(1));

            var sleep = new MessageAction("Went to sleep");
            sleep.AddCost(10);
            sleep.AddResult(energy, new IntegerAddResult(25));

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
                //energy.Value -= 1;
                hunger.Value += random.Next(3);

                Console.WriteLine($"    Energy: {energy.Value} Hunger: {hunger.Value}");

                var nextAction = agent.GetNextAction();
                if (nextAction == null)
                {
                    Console.WriteLine("No plan was found!");
                }
                else
                {
                    nextAction.Perform();
                }

                Console.ReadLine();
            }
        }
    }
}