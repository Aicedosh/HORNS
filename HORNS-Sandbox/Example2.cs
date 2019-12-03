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
        private readonly static object lo = new object();
        private class MessageAction : HORNS.Action
        {
            private readonly string agentName;
            private readonly string[] messages;

            public MessageAction(string agentName, params string[] messages)
            {
                this.agentName = agentName;
                this.messages = messages;
            }

            public override void Perform()
            {
                Random rand = new Random();
                foreach (var m in messages)
                {
                    Thread.Sleep(rand.Next(700, 1300));
                    Console.WriteLine($"Agent {agentName}: {m}");
                }
                lock (lo)
                {
                    Apply();
                }
            }
        }

        private class HibernateAction : HORNS.Action, HORNS.IVariableObserver
        {
            private readonly EventWaitHandle e;
            private readonly string name;
            private bool sleeping;

            public HibernateAction(string name, params HORNS.Variable[] vars)
            {
                this.name = name;
                e = new EventWaitHandle(false, EventResetMode.AutoReset);
                foreach (var v in vars)
                {
                    v.Observe(this);
                }
            }

            public override void Perform()
            {
                Console.WriteLine($"Agent {name}: Went to sleep");
                sleeping = true;
                e.WaitOne();
                sleeping = false;
                Console.WriteLine($"Agent {name}: Woke up");
                lock (lo)
                {
                    Apply();
                }
            }

            public void ValueChanged()
            {
                if (sleeping)
                {
                    e.Set();
                }
            }
        }

        private class WaitForAction : HORNS.Action, IVariableObserver
        {
            private readonly string agentname;
            private readonly IntVariable v;
            private readonly string m;
            private readonly EventWaitHandle e;
            private bool sleeping;

            public WaitForAction(string agentname, IntVariable v, string m)
            {
                e = new EventWaitHandle(false, EventResetMode.AutoReset);
                v.Observe(this);
                this.agentname = agentname;
                this.v = v;
                this.m = m;
            }

            public override void Perform()
            {
                while(true)
                {
                    lock (lo)
                    {
                        if (v.Value > 1)
                        {
                            Console.WriteLine($"Agent {agentname}: {m}");
                            Apply();
                            break;
                        }
                    }
                    sleeping = true;
                    Console.WriteLine($"Agent {agentname}: Waiting...");
                    e.WaitOne();
                    sleeping = false;
                }
            }

            public void ValueChanged()
            {
                if (sleeping)
                {
                    e.Set();
                }
            }
        }

        private class Hunger : Need<int>
        {
            public Hunger(Variable<int> variable, int desired) : base(variable, desired, v =>
            v > 100 ? -100 :
            (float)(50*Math.Log10(-v+1+100))) { }
        }

        private class Energy : Need<int>
        {
            public Energy(Variable<int> variable, int desired) : base(variable, desired, v => 
            v < 0 ? 100 : (float)(50*Math.Log10(v + 1))) { }

            protected override bool IsSatisfied(int value)
            {
                return value >= 100;
            }
        }

        public static Agent CreateWoodcutter(string agentName, IntVariable radishesOnCounter, IntVariable chairDemand, IntVariable chairsInStock)
        {
            var hasAxe = new BoolVariable();
            var hunger = new IntVariable(100);
            var energy = new IntVariable(100);
            var money = new IntVariable();
            var wood = new IntVariable();
            var chairs = new IntVariable();
            var rzodkiews = new IntVariable();
            var soups = new IntVariable();

            var feelingSoupy = new BoolVariable();

            var pickAxe = new MessageAction(agentName, "Picked up an axe");
            pickAxe.AddPrecondition(hasAxe, new BooleanPrecondition(false));
            pickAxe.AddResult(hasAxe, new BooleanResult(true));

            var chopTree = new MessageAction(agentName, "Chop", "Chopped down a tree");
            chopTree.AddPrecondition(hasAxe, new BooleanPrecondition(true));
            chopTree.AddResult(wood, new IntegerAddResult(1));
            chopTree.AddResult(energy, new IntegerAddResult(-2));

            var sellWood = new MessageAction(agentName, "Sold a piece of wood");
            sellWood.AddPrecondition(wood, new IntegerPrecondition(1, IntegerPrecondition.Condition.AtLeast));
            sellWood.AddResult(wood, new IntegerAddResult(-1));
            sellWood.AddResult(money, new IntegerAddResult(1));

            var makeChair = new MessageAction(agentName, "Made a chair");
            makeChair.AddPrecondition(wood, new IntegerPrecondition(1, IntegerPrecondition.Condition.AtLeast));
            makeChair.AddResult(wood, new IntegerAddResult(-1));
            makeChair.AddResult(chairs, new IntegerAddResult(1));
            makeChair.AddResult(energy, new IntegerAddResult(-3));

            var sellChair = new MessageAction(agentName, "Sold a chair");
            sellChair.AddPrecondition(chairDemand, new IntegerPrecondition(1, IntegerPrecondition.Condition.AtLeast));
            sellChair.AddPrecondition(chairs, new IntegerPrecondition(1, IntegerPrecondition.Condition.AtLeast));
            sellChair.AddResult(chairs, new IntegerAddResult(-1));
            sellChair.AddResult(money, new IntegerAddResult(3));
            sellChair.AddResult(chairsInStock, new IntegerAddResult(1));

            var buyRzodkiew = new MessageAction(agentName, "Bought a rzodkiew");
            buyRzodkiew.AddPrecondition(radishesOnCounter, new IntegerPrecondition(1, IntegerPrecondition.Condition.AtLeast));
            buyRzodkiew.AddPrecondition(money, new IntegerPrecondition(3, IntegerPrecondition.Condition.AtLeast));
            buyRzodkiew.AddResult(money, new IntegerAddResult(-3));
            buyRzodkiew.AddResult(rzodkiews, new IntegerAddResult(1));
            buyRzodkiew.AddResult(radishesOnCounter, new IntegerAddResult(-1));

            var eatRzodkiew = new MessageAction(agentName, "Ate a rzodkiew");
            eatRzodkiew.AddPrecondition(rzodkiews, new IntegerPrecondition(1, IntegerPrecondition.Condition.AtLeast));
            eatRzodkiew.AddResult(rzodkiews, new IntegerAddResult(-1));
            eatRzodkiew.AddResult(hunger, new IntegerAddResult(-5));

            var makeSoup = new MessageAction(agentName, "Made some soup");
            makeSoup.AddPrecondition(rzodkiews, new IntegerPrecondition(2, IntegerPrecondition.Condition.AtLeast));
            makeSoup.AddResult(rzodkiews, new IntegerAddResult(-2));
            makeSoup.AddResult(soups, new IntegerAddResult(1));
            makeSoup.AddResult(energy, new IntegerAddResult(-11));

            var eatSoup = new MessageAction(agentName, "Ate some soup");
            eatSoup.AddPrecondition(soups, new IntegerPrecondition(1, IntegerPrecondition.Condition.AtLeast));
            eatSoup.AddResult(soups, new IntegerAddResult(-1));
            eatSoup.AddResult(hunger, new IntegerAddResult(-20));
            eatSoup.AddResult(energy, new IntegerAddResult(1));

            var sleep = new MessageAction(agentName, "Went to sleep");
            sleep.AddCost(10);
            sleep.AddResult(energy, new IntegerAddResult(25));

            var idle = new MessageAction(agentName, "Is bored");
            idle.AddCost(100000);

            var hungerNeed = new Hunger(hunger, 0);
            var energyNeed = new Energy(energy, int.MaxValue);

            var agent = new Agent();
            agent.AddNeed(hungerNeed);
            agent.AddNeed(energyNeed);
            agent.AddActions(pickAxe, chopTree, sellWood, buyRzodkiew, eatRzodkiew, makeChair, sellChair, makeSoup, eatSoup, sleep);
            agent.AddIdleAction(idle);

            return agent;
        }

        private static Agent CreateSeller(string agentName, IntVariable radishesOnCounter)
        {
            Need<int> sellRadishes = new Need<int>(radishesOnCounter, 10, v => v);
            MessageAction putRadish = new MessageAction(agentName, "Searching for radish", "Carrying radish", "Put radish on counter");
            putRadish.AddResult(radishesOnCounter, new IntegerAddResult(1));

            var sleep = new HibernateAction(agentName, radishesOnCounter);

            Agent agent = new Agent();
            agent.AddIdleAction(sleep);
            agent.AddAction(putRadish);
            agent.AddNeed(sellRadishes);

            return agent;
        }

        private static Agent CreateArtist(string agentName, IntVariable chairDemand, IntVariable chairsInStock)
        {
            IntVariable chairs = new IntVariable();

            MessageAction demandChair = new MessageAction(agentName, "Demanding chair");
            demandChair.AddResult(chairDemand, new IntegerAddResult(1));

            var buyChair = new WaitForAction(agentName, chairsInStock, "Bought a chair");
            buyChair.AddPrecondition(chairDemand, new IntegerPrecondition(1, IntegerPrecondition.Condition.AtLeast));
            buyChair.AddResult(chairs, new IntegerAddResult(1));
            buyChair.AddResult(chairDemand, new IntegerAddResult(-1));
            buyChair.AddResult(chairsInStock, new IntegerAddResult(-1));

            Need<int> idea = new Need<int>(chairs, 100, v => 10000 * v);

            Agent agent = new Agent();
            agent.AddActions(demandChair, buyChair);
            agent.AddNeed(idea);

            return agent;
        }

        private static void RunAgent(Agent agent, CancellationToken token)
        {
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var nextAction = await agent.GetNextActionAsync(token);
                        if (nextAction == null)
                        {
                            Console.WriteLine("No plan was found!");
                        }
                        else
                        {
                            nextAction.Perform();
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        Console.WriteLine("Canceled plan calculation");
                    }
                }
                Console.WriteLine("End agent loop");
            });
        }

        public static void Run()
        {
            IntVariable radishesOnCounter = new IntVariable(9);
            IntVariable chairDemand = new IntVariable(3);
            IntVariable chairsInStock = new IntVariable(3);

            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            RunAgent(CreateWoodcutter("woodcutter", radishesOnCounter, chairDemand, chairsInStock), token);
            RunAgent(CreateSeller("seller", radishesOnCounter), token);
            RunAgent(CreateArtist("artist 1", chairDemand, chairsInStock), token);
            RunAgent(CreateArtist("artist 2", chairDemand, chairsInStock), token);

            Console.ReadLine();
            source.Cancel();
            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
