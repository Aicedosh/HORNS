using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HORNS
{
    /// <summary>
    /// Klasa bazowa dla akcji możliwych do wykonania przez agenta.
    /// </summary>
    public abstract class Action
    {
        /// <summary>
        /// Ostatni obliczony koszt danej akcji.
        /// </summary>
        public float CachedCost { get; private set; }

        private ICollection<ActionResult> results = new List<ActionResult>();
        private ICollection<Precondition> preconditions = new List<Precondition>();
        private ICollection<IActionCostEvaluator> costEvaluators = new List<IActionCostEvaluator>();

        //TODO: Implement builder pattern to ensure all results, costs and precondidtions are added before adding to agent's possible actions
        //      This may also make it possible to ommit passing same solver in every call (first calling setup for solver and then adding results/preconditions)
        /// <summary>
        /// Dodaje rezultat do zbioru rezultatów akcji.
        /// </summary>
        /// <typeparam name="T">Typ danych przechowywanych w zmiennej \texttt{variable}.</typeparam>
        /// <typeparam name="RT">Typ dodawanego rezultatu.</typeparam>
        /// <typeparam name="ST">Typ solvera skojarzony z daną zmienną.</typeparam>
        /// <typeparam name="PT">Typ wymagania skojarzony z daną zmienną.</typeparam>
        /// <param name="variable">Zmienna, której dotyczy rezultat.</param>
        /// <param name="result">Nowy rezultat akcji.</param>
        public void AddResult<T, RT, ST, PT>(Variable<T, RT, ST, PT> variable, RT result)
            where ST : VariableSolver<T, RT, PT>, new()
            where RT : ActionResult<T>
            where PT : Precondition<T>
        {
            //TODO: Clone beforehand
            variable.Solver.Register(result);
            result.Variable = variable;
            result.Action = this;
            results.Add(result);
        }

        /// <summary>
        /// Dodaje wymaganie do zbioru wymagań akcji.
        /// </summary>
        /// <typeparam name="T">Typ danych przechowywanych w zmiennej \texttt{variable}.</typeparam>
        /// <typeparam name="RT">Typ rezultatu skojarzony z daną zmienną.</typeparam>
        /// <typeparam name="ST">Typ solvera skojarzony z daną zmienną.</typeparam>
        /// <typeparam name="PT">Typ dodawanego wymagania.</typeparam>
        /// <param name="variable">Zmienna, której dotyczy wymaganie.</param>
        /// <param name="precondition">Nowe wymaganie akcji.</param>
        public void AddPrecondition<T, RT, ST, PT>(Variable<T, RT, ST, PT> variable, PT precondition)
            where ST : VariableSolver<T, RT, PT>, new()
            where RT : ActionResult<T>
            where PT : Precondition<T>
        {
            //TODO: Clone beforehand
            preconditions.Add(precondition);
            
            precondition.Variable = variable;
        }

        /// <summary>
        /// Dodaje koszt zależny od wartości zmiennej do kosztu akcji.
        /// </summary>
        /// <typeparam name="T">Typ danych przechowywanych w zmiennej \texttt{variable}.</typeparam>
        /// <param name="variable">Zmienna, od której zależy koszt.</param>
        /// <param name="evaluationFunction">Funkcja wyznaczająca koszt dla danej wartości zmiennej.</param>
        public void AddCost<T>(Variable<T> variable, Func<T, float> evaluationFunction)
        {
            costEvaluators.Add(new VariableCostEvaluator<T>(variable, evaluationFunction));
        }

        /// <summary>
        /// Dodaje stały koszt do kosztu akcji.
        /// </summary>
        /// <param name="cost">Wartość kosztu.</param>
        public void AddCost(float cost)
        {
            costEvaluators.Add(new ConstantCostEvaluator(cost));
        }

        internal float GetCost(Agent agent, IdSet<Variable> variables = null)
        {
            float cost = 0f;

            foreach(IActionCostEvaluator evaluator in costEvaluators)
            {
                cost += evaluator.GetCost();
            }

            foreach(ActionResult result in results)
            {
                cost += result.GetCost(variables, agent);
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

        /// <summary>
        /// Wykonuje dowolne operacje związane z realizacją akcji.
        /// </summary>
        public abstract void Perform();

        /// <summary>
        /// \texttt{true}, jeżeli akcja może być wykonana w danym momencie.
        /// </summary>
        public bool CanExecute => preconditions.All(p => p.IsFulfilledByWorld());

        internal bool CanExecuteIn(IdSet<Variable> variables)
        {
            return preconditions.All(p => p.IsFulfilledBy(variables));
        }

        /// <summary>
        /// Wykonuje wszystkie rezultaty związane z akcją, jeżeli akcja może być wykonana.
        /// </summary>
        /// <returns>\texttt{true}, jeżeli akcja została wykonana poprawnie.</returns>
        public bool Apply()
        {
            if(CanExecute == false)
            {
                return false;
            }

            foreach (ActionResult result in results)
            {
                result.Apply();
            }

            return true;
        }

        internal void ApplyResults(IdSet<Variable> variables)
        {
            foreach (ActionResult result in results)
            {
                result.Apply(variables);
            }
        }

        internal void ApplyResults(Variable variable)
        {
            foreach (ActionResult result in results)
            {
                if (variable.Id == result.AbstractVariable.Id)
                {
                    result.Apply(variable);
                }
            }
        }

        internal IEnumerable<Precondition> GetPreconditions()
        {
            return preconditions;
        }
        
        internal void ApplyResults(PreconditionSet requirements)
        {
            foreach (var result in results)
            {
                result.Apply(requirements);
            }

        }
    }
}
