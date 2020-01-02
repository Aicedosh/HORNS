using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Abstrakcyjna klasa bazowa dla wymagań dla zmiennych typu T powiązanych z solverem typu ST.
    /// </summary>
    /// <typeparam name="T">Typ danych przechowywanych w zmiennej związanej z wymaganiem.</typeparam>
    /// <typeparam name="ST">Typ solvera związanego z wymaganiem.</typeparam>
    //public abstract class Precondition<T, ST> : Precondition<T> where ST : VariableSolver<T>
    //{
    //    private ST solver;

    //    /// <summary>
    //    /// Tworzy nowe wymaganie o określonej wartości docelowej.
    //    /// </summary>
    //    /// <param name="value">Wartość docelowa wymagania.</param>
    //    public Precondition(T value) : base(value)
    //    {

    //    }

    //    /// <summary>
    //    /// Tworzy nowe wymaganie o określonej wartości docelowej i zmiennej pochodzącej z innego wymagania.
    //    /// </summary>
    //    /// <param name="value"></param>
    //    /// <param name="precondition"></param>
    //    protected Precondition(T value, T state, Precondition<T, ST> precondition) : this(value)
    //    {
    //        this.Variable = precondition.Variable;
    //        this.solver = precondition.solver;
    //        State = state;
    //    }

    //    /// <summary>
    //    /// Tworzy nowe wymaganie bedące kopią innego wymagania.
    //    /// </summary>
    //    /// <param name="precondition">Wymaganie do skopiowania.</param>
    //    public Precondition(Precondition<T, ST> precondition) : base(precondition)
    //    {
    //        solver = precondition.solver;
    //    }

    //    internal void SetSolver(ST solver)
    //    {
    //        this.solver = solver;
    //    }

    //    /// <summary>
    //    /// Wyznacza akcje mogące spełnić wymaganie.
    //    /// </summary>
    //    /// <param name="agent">Agent, którego akcje będą rozważane.</param>
    //    /// <returns>Kolekcja akcji.</returns>
    //    protected internal override IEnumerable<Action> GetActions(Agent agent)
    //    {
    //        return solver.GetActionsSatisfying(this, agent);
    //    }
    //}
}
