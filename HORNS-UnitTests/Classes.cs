using HORNS;

namespace HORNS_UnitTests
{
    // ACTIONS

    // BasicAction
    class BasicAction : Action
    {
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
}
