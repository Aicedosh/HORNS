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
        private readonly static object lo = new object();
        private readonly static object wo = new object();

        private static void Write(string s, ConsoleColor color)
        {
            lock(wo)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(s);
                Console.ResetColor();
            }
        }

        private class MessageAction : HORNS.Action
        {
            private readonly string agentName;
            private readonly ConsoleColor color;
            private readonly string[] messages;

            public MessageAction(string agentName, ConsoleColor color, params string[] messages)
            {
                this.agentName = agentName;
                this.color = color;
                this.messages = messages;
            }

            public override void Perform()
            {
                Random rand = new Random();
                foreach (var m in messages)
                {
                    Thread.Sleep(rand.Next(700, 1300));
                    Write($"Agent {agentName}: {m}", color);
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
            private readonly ConsoleColor color;
            private bool sleeping;

            public HibernateAction(string name, ConsoleColor color, params HORNS.Variable[] vars)
            {
                this.name = name;
                this.color = color;
                e = new EventWaitHandle(false, EventResetMode.AutoReset);
                foreach (var v in vars)
                {
                    v.Observe(this);
                }
            }

            public override void Perform()
            {
                Write($"Agent {name}: Went to sleep", color);
                sleeping = true;
                e.WaitOne();
                sleeping = false;
                Write($"Agent {name}: Woke up", color);
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
            private readonly ConsoleColor color;
            private readonly IntegerVariable v;
            private readonly string m;
            private readonly EventWaitHandle e;
            private bool sleeping;

            public WaitForAction(string agentname, ConsoleColor color, IntegerVariable v, string m)
            {
                e = new EventWaitHandle(false, EventResetMode.AutoReset);
                v.Observe(this);
                this.agentname = agentname;
                this.color = color;
                this.v = v;
                this.m = m;
            }

            public override void Perform()
            {
                while(true)
                {
                    lock (lo)
                    {
                        if (v.Value >= 1)
                        {
                            Write($"Agent {agentname}: {m}", color);
                            Apply();
                            break;
                        }
                    }
                    sleeping = true;
                    Write($"Agent {agentname}: Waiting...", color);
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
            public Hunger(Variable<int> variable, int desired) : base(variable, desired,
                v => v > 100 ? -100 : (float)(50*Math.Log10(-v+1+100))) { }
        }

        private class Energy : Need<int>
        {
            public Energy(Variable<int> variable, int desired) : base(variable, desired,
                v => v < 0 ? 100 : (float)(50*Math.Log10(v + 1)),
                v => v >= 100) { }
        }

        public static Agent CreateWoodcutter(string agentName, ConsoleColor color, IntegerVariable radishesOnCounter, IntegerVariable chairDemand, IntegerVariable chairsInStock)
        {
            var hasAxe = new BooleanVariable();
            var hunger = new IntegerVariable(100);
            var energy = new IntegerVariable(100);
            var money = new IntegerVariable();
            var wood = new IntegerVariable();
            var chairs = new IntegerVariable();
            var rzodkiews = new IntegerVariable();
            var soups = new IntegerVariable();

            var pickAxe = new MessageAction(agentName, color, "Picked up an axe");
            pickAxe.AddPrecondition(hasAxe, new BooleanPrecondition(false));
            pickAxe.AddResult(hasAxe, new BooleanResult(true));

            var chopTree = new MessageAction(agentName, color, "Chop", "Chopped down a tree");
            chopTree.AddPrecondition(hasAxe, new BooleanPrecondition(true));
            chopTree.AddResult(wood, new IntegerAddResult(1));
            chopTree.AddResult(energy, new IntegerAddResult(-2));

            var sellWood = new MessageAction(agentName, color, "Sold a piece of wood");
            sellWood.AddPrecondition(wood, new IntegerPrecondition(1, true));
            sellWood.AddResult(wood, new IntegerAddResult(-1));
            sellWood.AddResult(money, new IntegerAddResult(1));

            var makeChair = new MessageAction(agentName, color, "Made a chair");
            makeChair.AddPrecondition(wood, new IntegerPrecondition(1, true));
            makeChair.AddResult(wood, new IntegerAddResult(-1));
            makeChair.AddResult(chairs, new IntegerAddResult(1));
            makeChair.AddResult(energy, new IntegerAddResult(-3));

            var sellChair = new MessageAction(agentName, color, "Sold a chair");
            sellChair.AddPrecondition(chairDemand, new IntegerPrecondition(1, false));
            sellChair.AddPrecondition(chairs, new IntegerPrecondition(1, true));
            sellChair.AddResult(chairs, new IntegerAddResult(-1));
            sellChair.AddResult(money, new IntegerAddResult(3));
            sellChair.AddResult(chairsInStock, new IntegerAddResult(1));

            var buyRzodkiew = new MessageAction(agentName, color, "Bought a rzodkiew");
            buyRzodkiew.AddPrecondition(radishesOnCounter, new IntegerPrecondition(1, true));
            buyRzodkiew.AddPrecondition(money, new IntegerPrecondition(3, true));
            buyRzodkiew.AddResult(money, new IntegerAddResult(-3));
            buyRzodkiew.AddResult(rzodkiews, new IntegerAddResult(1));
            buyRzodkiew.AddResult(radishesOnCounter, new IntegerAddResult(-1));

            var eatRzodkiew = new MessageAction(agentName, color, "Ate a rzodkiew");
            eatRzodkiew.AddPrecondition(rzodkiews, new IntegerPrecondition(1, true));
            eatRzodkiew.AddResult(rzodkiews, new IntegerAddResult(-1));
            eatRzodkiew.AddResult(hunger, new IntegerAddResult(-5));

            var makeSoup = new MessageAction(agentName, color, "Made some soup");
            makeSoup.AddPrecondition(rzodkiews, new IntegerPrecondition(2, true));
            makeSoup.AddResult(rzodkiews, new IntegerAddResult(-2));
            makeSoup.AddResult(soups, new IntegerAddResult(1));
            makeSoup.AddResult(energy, new IntegerAddResult(-11));

            var eatSoup = new MessageAction(agentName, color, "Ate some soup");
            eatSoup.AddPrecondition(soups, new IntegerPrecondition(1, true));
            eatSoup.AddResult(soups, new IntegerAddResult(-1));
            eatSoup.AddResult(hunger, new IntegerAddResult(-20));
            eatSoup.AddResult(energy, new IntegerAddResult(1));

            var sleep = new MessageAction(agentName, color, "Went to sleep");
            sleep.AddCost(10);
            sleep.AddResult(energy, new IntegerAddResult(25));

            var idle = new MessageAction(agentName, color, "Is bored");
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

        private static Agent CreateSeller(string agentName, ConsoleColor color, IntegerVariable radishesOnCounter)
        {
            Need<int> sellRadishes = new Need<int>(radishesOnCounter, 10, v => v);
            MessageAction putRadish = new MessageAction(agentName, color, "Searching for radish", "Carrying radish", "Put radish on counter");
            putRadish.AddResult(radishesOnCounter, new IntegerAddResult(1));

            var sleep = new HibernateAction(agentName, color, radishesOnCounter);

            Agent agent = new Agent();
            agent.AddIdleAction(sleep);
            agent.AddAction(putRadish);
            agent.AddNeed(sellRadishes);

            return agent;
        }

        private static Agent CreateArtist(string agentName, ConsoleColor color, IntegerVariable chairDemand, IntegerVariable chairsInStock)
        {
            IntegerVariable chairs = new IntegerVariable();

            MessageAction demandChair = new MessageAction(agentName, color, "Demanding chair");
            demandChair.AddResult(chairDemand, new IntegerAddResult(1));

            var buyChair = new WaitForAction(agentName, color, chairsInStock, "Bought a chair");
            buyChair.AddPrecondition(chairDemand, new IntegerPrecondition(1, true));
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
            IntegerVariable radishesOnCounter = new IntegerVariable(9);
            IntegerVariable chairDemand = new IntegerVariable(0);
            IntegerVariable chairsInStock = new IntegerVariable(0);

            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            RunAgent(CreateWoodcutter("woodcutter", ConsoleColor.Cyan, radishesOnCounter, chairDemand, chairsInStock), token);
            RunAgent(CreateSeller("seller", ConsoleColor.Yellow, radishesOnCounter), token);
            RunAgent(CreateArtist("artist 1", ConsoleColor.White, chairDemand, chairsInStock), token);
            RunAgent(CreateArtist("artist 2", ConsoleColor.Magenta, chairDemand, chairsInStock), token);

            Console.ReadLine();
            source.Cancel();
            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
