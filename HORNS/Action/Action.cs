using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HORNS
{
    public abstract class Action
    {
        public float CachedCost { get; private set; }

        private ICollection<ActionResult> results = new List<ActionResult>();
        private ICollection<Precondition> preconditions = new List<Precondition>();
        private ICollection<IActionCostEvaluator> costEvaluators = new List<IActionCostEvaluator>();

        //TODO: Implement builder pattern to ensure all results, costs and precondidtions are added before adding to agent's possible actions
        //      This may also make it possible to ommit passing same solver in every call (first calling setup for solver and then adding results/preconditions)
        public void AddResult<T, RT, ST, PT>(Variable<T, RT, ST, PT> variable, RT result)
            where ST : VariableSolver<T, RT, PT>, new()
            where RT : ActionResult<T, ST>
            where PT : Precondition<T, ST>
        {
            //TODO: Clone beforehand
            variable.Solver.Register(result);
            result.Variable = variable;
            result.Action = this;
            results.Add(result);
        }

        public void AddPrecondition<T, RT, ST, PT>(Variable<T, RT, ST, PT> variable, PT precondition)
            where ST : VariableSolver<T, RT, PT>, new()
            where RT : ActionResult<T, ST>
            where PT : Precondition<T, ST>
        {
            //TODO: Clone beforehand
            preconditions.Add(precondition);

            precondition.SetSolver(variable.Solver);
            precondition.Variable = variable;
        }

        public void AddCost<T>(Variable<T> variable, Func<T, float> evaluationFunction)
        {
            costEvaluators.Add(new VariableCostEvaluator<T>(variable, evaluationFunction));
        }

        public void AddCost(float cost)
        {
            costEvaluators.Add(new ConstantCostEvaluator(cost));
        }

        internal float GetCost(IdSet<Variable> variablesPatch = null)
        {
            float cost = 0f;

            foreach(IActionCostEvaluator evaluator in costEvaluators)
            {
                cost += evaluator.GetCost();
            }

            foreach(ActionResult result in results)
            {
                cost += result.GetCost(variablesPatch);
            }

            CachedCost = cost;
            return cost;
        }

        internal IEnumerable<Variable> GetVariables()
        {
            List<Variable> variables = new List<Variable>();
            variables.AddRange(results.Select(r => r.AbstractVariable));
            //TODO: making this not null check seems unnecessary, maybe separate cost evaluators (the interface is probably unnecessary as well)
            variables.AddRange(costEvaluators.Select(e => e.GetVariable()).Where(v => v != null));
            variables.AddRange(preconditions.Select(p => p.GetVariable()));
            return variables;
        }

        protected abstract void ActionResult();

        public void Perform()
        {
            ActionResult();
            foreach(ActionResult result in results)
            {
                result.Apply();
            }
        }

        internal void ApplyResults(IdSet<Variable> variables)
        {
            foreach (ActionResult result in results)
            {
                result.Apply(variables);
            }
        }

        internal IEnumerable<Precondition> GetPreconditions()
        {
            return preconditions;
        }

        //internal void SubtractResults(RequirementSet requirements)
        internal void SubtractResults(PreconditionSet requirements)
        {
            foreach (var result in results)
            {
                result.SubtractFrom(requirements);
            }

        }
    }
}
