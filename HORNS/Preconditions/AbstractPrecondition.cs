using System.Collections.Generic;

namespace HORNS
{
    public enum ComparisonResult
    {
        NotComparable, Better, EqualWorse
    }
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
        
        internal abstract bool IsFulfilled();
        internal abstract bool IsFulfilledBy(IdSet<Variable> variables);
        internal abstract bool IsFulfilledByWorld();

        /// <summary>
        /// Porównuje wymaganie z innym wymaganiem.
        /// </summary>
        /// <param name="precondition">Wymaganie do porównania.</param>
        /// <returns>\texttt{true}, jeżeli obecne wymaganie jest w lepszym (łatwiejszym do spełnienia) stanie niż \texttt{precondition}; \texttt{false} w przeciwnym wypadku.</returns>
        protected internal abstract ComparisonResult IsBetterThan(Precondition precondition);
        /// <summary>
        /// Wyznacza akcje mogące spełnić wymaganie.
        /// </summary>
        /// <param name="agent">Agent, którego akcje będą rozważane.</param>
        /// <returns>Kolekcja akcji.</returns>
        protected internal abstract IEnumerable<Action> GetActions(Agent agent);
        /// <summary>
        /// Łączy wymaganie z innym wymaganiem.
        /// </summary>
        /// <param name="precondition">Wymaganie do połączenia.</param>
        /// <returns>Nowe wymaganie będące wynikiem połączenia.</returns>
        protected internal abstract Precondition Combine(Precondition precondition);

        internal abstract Variable GetVariable();
        internal abstract Precondition Apply(ActionResult actionResult);
    }
}
