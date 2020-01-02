using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Abstrakcyjna klasa bazowa dla zmiennych typu T powiązanych z typami rezultatu, solvera i wymagania.
    /// </summary>
    /// <typeparam name="T">Typ danych przechowywanych przez zmienną.</typeparam>
    /// <typeparam name="RT">Typ rezultatu związany ze zmienną.</typeparam>
    /// <typeparam name="ST">Typ solvera związany ze zmienną.</typeparam>
    /// <typeparam name="PT">Typ wymagania związany ze zmienną.</typeparam>
    public class Variable<T, RT, ST, PT> : Variable<T>
            where ST : VariableSolver<T, RT, PT>, new()
            where RT : ActionResult<T>
            where PT : Precondition<T>
    {
        /// <summary>
        /// Tworzy nową zmienną o określonej wartości początkowej.
        /// </summary>
        /// <param name="value">Wartość początkowa zmiennej.</param>
        protected Variable(T value = default) : base(value)
        {
            Solver = new ST();
            Solver.Variable = this;
        }

        internal Variable(Variable<T, RT, ST, PT> variable) : base(variable)
        {
            Solver = variable.Solver;
        }

        internal override Variable GetCopy()
        {
            return new Variable<T, RT, ST, PT>(this);
        }

        internal ST Solver { get; }

        internal override VariableSolver<T> GenericSolver => Solver;
    }
}
