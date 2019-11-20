using HORNS;

namespace HORNS_UnitTests
{
    // ACTIONS

    // BasicAction
    class BasicAction : Action
    {
        public BasicAction(int n = 0)
        {
            N = n;
        }

        public int N { get; }

        protected override void ActionResult()
        {
        }
    }

    // ChangeStateAction
    class State
    {
        public bool Value { get; set; } = false;
    }

    class ChangeStateAction : Action
    {
        public State State { get; private set; } = new State();
        protected override void ActionResult()
        {
            State.Value = true;
        }
    }

    // NEEDS

    // BooleanNeed
    class BooleanNeed : Need<bool>
    {
        public BooleanNeed(Variable<bool> variable, bool desired, VariableSolver<bool> solver)
            : base(variable, desired, solver)
        {
        }

        public override float Evaluate(bool value)
        {
            return value ? 100 : 1;
        }
    }

    class LinearIntegerNeed : Need<int>
    {
        public LinearIntegerNeed(Variable<int> variable, int desired, VariableSolver<int> solver)
            : base(variable, desired, solver)
        {
        }

        public override float Evaluate(int value)
        {
            return value;
        }
    }
}
