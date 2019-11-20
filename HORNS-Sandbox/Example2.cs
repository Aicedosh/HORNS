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
                return (value > 100 ? 100000 : 20) + value;
            }
        }

        public static void Run()
        {
            var hasAxe    = new Variable<bool>();
            var hunger    = new Variable<int>(50);
            var energy    = new Variable<int>(105);
            var money     = new Variable<int>();
            var wood      = new Variable<int>();
            var rzodkiews = new Variable<int>();

            var hasAxeSolver    = new BooleanSolver();
            var hungerSolver    = new IntegerSolver();
            var energySolver    = new IntegerSolver();
            var moneySolver     = new IntegerSolver();
            var woodSolver      = new IntegerSolver();
            var rzodkiewsSolver = new IntegerSolver();

            var pickAxe = new MessageAction("Picked up an axe");
            pickAxe.AddCost(1);
            pickAxe.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(
                new BooleanPrecondition(hasAxe, false, hasAxeSolver));
            pickAxe.AddResult<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(
                new BooleanResult(hasAxe, true), hasAxeSolver);

            var chopTree = new MessageAction("Chopped down a tree");
            chopTree.AddCost(5);
            chopTree.AddPrecondition<bool, BooleanResult, BooleanSolver, BooleanPrecondition>(
                new BooleanPrecondition(hasAxe, true, hasAxeSolver));
            chopTree.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(wood, 1), woodSolver);
            chopTree.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(energy, -10), energySolver);

            var sellWood = new MessageAction("Sold a piece of wood");
            sellWood.AddCost(3);
            sellWood.AddPrecondition<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerPrecondition(wood, 1, IntegerPrecondition.Condition.AtLeast, woodSolver));
            sellWood.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(wood, -1), woodSolver);
            sellWood.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(money, 1), moneySolver);

            var buyRzodkiew = new MessageAction("Bought a rzodkiew");
            buyRzodkiew.AddCost(3);
            buyRzodkiew.AddPrecondition<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerPrecondition(money, 3, IntegerPrecondition.Condition.AtLeast, moneySolver));
            buyRzodkiew.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(money, -3), moneySolver);
            buyRzodkiew.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(rzodkiews, 1), rzodkiewsSolver);

            var eatRzodkiew = new MessageAction("Ate a rzodkiew");
            eatRzodkiew.AddCost(1);
            eatRzodkiew.AddPrecondition<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerPrecondition(rzodkiews, 1, IntegerPrecondition.Condition.AtLeast, rzodkiewsSolver));
            eatRzodkiew.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(rzodkiews, -1), rzodkiewsSolver);
            eatRzodkiew.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(hunger, -20), hungerSolver);

            var sleep = new MessageAction("Went to sleep");
            sleep.AddCost(1);
            sleep.AddResult<int, IntegerAddResult, IntegerSolver, IntegerPrecondition>(
                new IntegerAddResult(energy, 100), energySolver);

            var idle = new MessageAction("Is bored");
            idle.AddCost(2000);

            var hungerNeed = new Hunger(hunger, 0, hungerSolver);
            var energyNeed = new Energy(energy, int.MaxValue, energySolver);

            var agent = new Agent();
            agent.AddNeed(hungerNeed);
            agent.AddNeed(energyNeed);
            agent.AddActions(pickAxe, chopTree, sellWood, buyRzodkiew, eatRzodkiew);
            agent.AddIdleAction(idle);
            
            // main loop
            while (true)
            {
                var nextAction = agent.GetNextAction();
                if (nextAction == null)
                {
                    Console.WriteLine("No plan was found!");
                }
                else
                {
                    nextAction.Perform();
                }
                energy.Value -= 1;
                hunger.Value += 1;
                Thread.Sleep(1000);
            }
        }
    }
}
