using HORNS;

class SimpleAction : Action
{
    private string name;

    public SimpleAction(string name)
    {
        this.name = name;
    }

    public override void Perform()
    {
        System.Console.WriteLine($"Performing Action '{name}'");
        Apply(); //zakończ akcję i zaaplikuj efekty
    }
}

class Example_doc
{
    public static void Run_old()
    {
        //Domyślna wartość zmiennej 'required' to 'false'
        BooleanVariable required = new BooleanVariable(false);

        //Potrzeba dotycząca zimennej 'variable' do zaspokojenia wymaga, by zmienna miała wartość 10
        IntegerVariable variable = new IntegerVariable(0);
        Need<int> need = new Need<int>(variable, 10, v => v);

        //Akcja zwiększenia zmiennej wymaga, by zmienna 'required' miała wartość 'true'
        Action increaseVariable = new SimpleAction("Increase Variable");
        increaseVariable.AddPrecondition(required, new BooleanPrecondition(true));
        increaseVariable.AddResult(variable, new IntegerAddResult(1));

        //Akcja 'setRequired' ustawia wartość zmiennej 'required' na 'true'
        Action setRequired = new SimpleAction("Set Required");
        setRequired.AddResult(required, new BooleanResult(true));

        //Agent 'agent' posiada 2 możliwe akcje do wykonania oraz jedną potrzebę
        Agent agent = new Agent();
        agent.AddNeed(need);
        agent.AddAction(increaseVariable);
        agent.AddAction(setRequired);

        System.Console.WriteLine($"Starting variable value: {variable.Value}");
        while(variable.Value < 10)
        {
            Action action = agent.GetNextAction();
            action.Perform();
            System.Console.WriteLine($"Variable value: {variable.Value}");
        }
    }

    public static void Run()
    {
        IntegerVariable v = new IntegerVariable(0);
        IntegerVariable n = new IntegerVariable(0);
        BooleanVariable b = new BooleanVariable(false);

        Need<int> need = new Need<int>(n, 10, x => x);

        Action a1 = new SimpleAction("1");
        a1.AddPrecondition(v, new IntegerPrecondition(4));
        a1.AddPrecondition(b, new BooleanPrecondition(true));
        a1.AddResult(n, new IntegerAddResult(1));
        a1.AddResult(v, new IntegerAddResult(-3));

        Action a2 = new SimpleAction("2");
        a2.AddResult(v, new IntegerAddResult(1));

        Action a3 = new SimpleAction("3");
        a3.AddPrecondition(v, new IntegerPrecondition(20));
        a3.AddResult(v, new IntegerAddResult(-20));
        a3.AddResult(b, new BooleanResult(true));

        Agent a = new Agent();
        a.AddActions(a1, a2, a3);
        a.AddNeed(need);

        while(n.Value < 10)
        {
            var ag = a.GetNextAction();
            ag.Perform();
            System.Console.WriteLine($"n: {n.Value}");
            System.Console.WriteLine($"v: {v.Value}");
        }
    }
}