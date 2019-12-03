using HORNS;

namespace HORNS_UnitTests
{
    // ACTIONS

    // BasicAction
    class BasicAction : Action
    {
        public BasicAction(string tag = "")
        {
            Tag = tag;
        }

        public string Tag { get; }

        public override void Perform()
        {
            Apply();
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
        public override void Perform()
        {
            State.Value = true;
            Apply();
        }
    }

    // NEEDS

    // BooleanNeed
    class BooleanNeed : Need<bool>
    {
        public BooleanNeed(Variable<bool> variable, bool desired)
            : base(variable, desired, v => v ? 100 : 1)
        {
        }
    }

    class LinearIntegerNeed : Need<int>
    {
        public LinearIntegerNeed(Variable<int> variable, int desired)
            : base(variable, desired, v => v)
        {
        }
    }
}
