using System.Collections.Generic;

namespace HORNS
{
    /// <summary>
    /// Abstrakcyjna klasa bazowa dla wszystkich wymagań.
    /// </summary>
    public abstract class Precondition : IIdentifiable
    {
        internal int Id => GetVariable().Id;
        int IIdentifiable.Id => Id;

        /// <summary>
        /// Wykonuje kopię obiektu wymagania.
        /// </summary>
        /// <returns>Kopia wymagania.</returns>
        protected internal abstract Precondition Clone();
        IIdentifiable IIdentifiable.GetCopy()
        {
            return Clone();
        }

        //private protected bool Fulfilled { get; set; } = false;
        internal abstract bool IsFulfilled();
        internal abstract bool IsFulfilledBy(IdSet<Variable> variables);
        internal abstract bool IsFulfilledByWorld();

        /// <summary>
        /// Porównuje wymaganie z innym wymaganiem.
        /// </summary>
        /// <param name="precondition">Wymaganie do porównania.</param>
        /// <returns>\texttt{true}, jeżeli \texttt{other} jest w takim samym lub gorszym stanie; \texttt{false} w przeciwnym wypadku.</returns>
        protected internal abstract bool IsEqualOrWorse(Precondition precondition);
        /// <summary>
        /// Wyznacza akcje mogące spełnić wymaganie.
        /// </summary>
        /// <param name="agent">Agent, którego akcje będą rozważane.</param>
        /// <returns>Kolekcja akcji.</returns>
        protected internal abstract IEnumerable<Action> GetActions(Agent agent);
        /// <summary>
        /// Odejmuje rezultat akcji od wymagania.
        /// </summary>
        /// <param name="actionResult">Rezultat do odjęcia.</param>
        /// <returns>Nowe wymaganie będące wynikiem odjęcia rezultatu.</returns>
        protected internal abstract Precondition Subtract(ActionResult actionResult);
        /// <summary>
        /// Łączy wymaganie z innym wymaganiem.
        /// </summary>
        /// <param name="precondition">Wymaganie do połączenia.</param>
        /// <returns>Nowe wymaganie będące wynikiem połączenia.</returns>
        protected internal abstract Precondition Combine(Precondition precondition);

        internal abstract Variable GetVariable();
    }
}
